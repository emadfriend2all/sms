using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Xml.Linq;
using FastReport;
using FastReport.Barcode;
using FastReport.Data;
using FastReport.Data.JsonConnection;
using FastReport.Export.Image;
using FastReport.Format;
using FastReport.Table;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema.Generation;
using Reporting.Constants;
using Reporting.Models;
using ShowMatic.Server.Application.Extensions;
using static FastReport.Utils.FileSize;
using Units = FastReport.Utils.Units;

namespace Reporting.Templates;
public static class ReportGenerator
{
    private static readonly string TemplateReportPath = "Reports/Templates/Templates.frx";
    private static readonly string QRCodeTemplateReportPath = "Reports/Templates/QRCodeTemplate.frx";

    public static ReportPage CreatePage(this Base report, string? name = null)
    {
        ReportPage page = new ReportPage();
        page.RawPaperSize = 9;
        page.SetObjectName(name);
        page.Parent = report;
        return page;
    }

    public static PageHeaderBand CreateHeader(this Base page, string? imageUrl = null, int? heigh = null, string? name = null)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        PageHeaderBand pageHeader = (PageHeaderBand)templateReport.FindObject(ReportIdentifiers.ReportPageHeader);
        pageHeader.Height = heigh ?? Units.Centimeters * 3;
        pageHeader.SetObjectName(name);
        pageHeader.Parent = page;

        if (imageUrl != null)
        {
            PictureObject pictureObject = (PictureObject)pageHeader.FindObject(ReportIdentifiers.HeaderImage);
            pictureObject.ImageLocation = imageUrl;
        }

        return pageHeader;
    }

    public static PageFooterBand CreateFooter(this Base page, int? heigh = null, bool setPaheNumber = true)
    {
        PageFooterBand pageFooter = new PageFooterBand();
        pageFooter.Height = heigh ?? Units.Centimeters * 3;
        pageFooter.SetObjectName();
        pageFooter.Parent = page;
        pageFooter.SetPageNumber();
        return pageFooter;
    }

    public static ReportPage CreateTitle(this ReportPage page, string title, string? name = null, int? heigh = null)
    {
        page.ReportTitle = new ReportTitleBand();
        page.ReportTitle.Height = Units.Centimeters * heigh ?? 1;
        page.ReportTitle.SetObjectName($"title_{name}");

        // create title text
        TextObject titleText = new TextObject();
        titleText.Parent = page.ReportTitle;
        titleText.SetObjectName(name);
        titleText.Bounds = new RectangleF(Units.Centimeters * 5, 0, Units.Centimeters * 10, Units.Centimeters * heigh ?? 1);
        titleText.Font = new Font("Arial", 14, FontStyle.Bold);
        titleText.Text = title;
        titleText.HorzAlign = HorzAlign.Center;
        return page;
    }

    public static DataBand CreateBand(this ReportPage page, DataSourceBase? datasource = null, string? name = null)
    {
        DataBand dataBand = CreateBand(datasource);
        page.Bands.Add(dataBand);
        return dataBand;
    }

    public static DataBand CreateBand(this GroupHeaderBand groupHeaderBand, float heigh, DataSourceBase? datasource = null, string? name = null)
    {
        DataBand dataBand = CreateBand(datasource, name = null);
        groupHeaderBand.Data = dataBand;
        return dataBand;
    }

    public static ReportPage CreateHirarchyDataBand(this ReportPage page, DataSourceBase? datasource, string text, string? columnId = null, string? parentId = null)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataBand hirarchyBand = (DataBand)templateReport.FindObject(ReportIdentifiers.HirarchyDataBand);
        hirarchyBand.SetObjectName();
        hirarchyBand.DataSource = datasource;
        hirarchyBand.IdColumn = columnId;
        hirarchyBand.ParentIdColumn = parentId;
        hirarchyBand.Indent = 1.5f;
        var textObj = (TextObject)hirarchyBand.FindObject(ReportIdentifiers.HirarchyContentText);
        textObj!.Text = text;

        //textObj.BeforePrint += (sender, e) =>
        //{
        //    bool isParent = Convert.ToBoolean((sender as TextObject).Report.GetColumnValue("IsGroup"));

        //    if (!isParent)
        //    {
        //        (sender as TextObject).Style = "ParagraphBold";
        //    }
        //};
        //textObj.Parent = dataBand;
        hirarchyBand.Parent = page;
        return (ReportPage)page;
    }

    public static DataBand CreateBand(DataSourceBase? datasource = null, string? name = null)
    {
        DataBand dataBand = new DataBand();
        dataBand.PrintIfDatasourceEmpty = true;
        dataBand.PrintIfDetailEmpty = true;
        if (datasource != null)
            dataBand.DataSource = datasource;
        dataBand.CanGrow = true;
        dataBand.SetObjectName(name);

        return dataBand;
    }

    public static ReportSummaryBand CreateSignatureBand(this Base page, string firstParty, string secondParty, string? description = null)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        ReportSummaryBand reportSummaryBand = (ReportSummaryBand)templateReport.FindObject(ReportIdentifiers.SignatureBand);

        if (description != null)
        {
            TextObject descriptionTxt = (TextObject)reportSummaryBand.FindObject("DescriptionText");
            descriptionTxt.Text = description;
        }

        TextObject firstPartyTxt = (TextObject)reportSummaryBand.FindObject("FirstPartyName");
        firstPartyTxt.Text = firstParty;

        TextObject secondPartyTxt = (TextObject)reportSummaryBand.FindObject("SecondPartyName");
        secondPartyTxt.Text = secondParty;
        reportSummaryBand.Printable = true;
        reportSummaryBand.Parent = page;
        return reportSummaryBand;
    }

    public static DataHeaderBand CreateContractHeaderBand(this Base databand, ContractHeaderRequest request)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataHeaderBand dataHeaderBand = (DataHeaderBand)templateReport.FindObject(ReportIdentifiers.ContractHeader);

        DateTime higriDate = DateTime.ParseExact(request.DateInHigriTxt ?? DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
        string higri = higriDate.ToString("dd-MM-yyyy", new CultureInfo("ar-SA"));
        var qrcodeInfo = new { request.Id, request.CustomerId };

        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.PrintDateTxt, request.PrintDateTxt ?? DateTime.Now.ToString("dd-MM-yyyy"));
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.DateInHigriTxt, higri);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.DocumentNumberTxt, request.DocumentNumberTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.DocumentNameTxt, request.DocumentNameTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.FirstPartyNameTxt, request.FirstPartyNameTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.FirstPartyAddressTxt, request.FirstPartyAddressTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.FirstPartyRegistrationNoTxt, request.FirstPartyRegistrationNoTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.SecondPartyNameTxt, request.SecondPartyNameTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.SecondPartyAddressTxt, request.SecondPartyAddressTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.SecondPartyIdTxt, request.SecondPartyIdTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.SecondPartyPhoneTxt, request.SecondPartyPhoneTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.ReferenceSerialNumberTxt, request.ReferenceSerialNumber);


        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.OwnerNameTxt, request.OwnerNameTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.FirstPartyOwnerTitleTxt, request.FirstPartyOwnerTitleTxt);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.SecondPartyIdLableTxt, request.SecondPartyIdLabelTxt);

        TextObject referenceLabelTxt = (TextObject)dataHeaderBand.FindObject(ReportIdentifiers.ReferenceLabelTxt);

        if (string.IsNullOrEmpty(request.ReferenceSerialNumber))
        {
            referenceLabelTxt.Visible = false;
        }

        ChildBand customerNameHeaderChildBand = (ChildBand)dataHeaderBand.FindObject(ReportIdentifiers.CustomerNameHeaderChildBand);
        ChildBand contractHeaderPariesChildBand = (ChildBand)customerNameHeaderChildBand.FindObject(ReportIdentifiers.ContractHeaderPariesChildBand);
        if (request.IncludeParties)
        {
            customerNameHeaderChildBand.Visible = false;
            contractHeaderPariesChildBand.Visible = true;
            customerNameHeaderChildBand.Height = 0;
        }
        else
        {
            customerNameHeaderChildBand.SetTextObjectValue(ReportIdentifiers.SecondPartyNameInHeaderTxt, request.SecondPartyNameTxt);
            contractHeaderPariesChildBand.Visible = false;
            customerNameHeaderChildBand.Visible = true;
            contractHeaderPariesChildBand.Height = 0;

        }
        BarcodeObject barcodeObject = (BarcodeObject)templateReport.FindObject(ReportIdentifiers.QRCodeObject);
        barcodeObject.Text = JsonConvert.SerializeObject(qrcodeInfo);
        dataHeaderBand.Parent = databand;
        return dataHeaderBand;
    }

    public static void SetTextObjectValue(this Base container, string? objectNameInReport, string? value, bool addToCurrentTxt = false)
    {
        if (objectNameInReport == null || value == null) { return; }
        try
        {
            if (addToCurrentTxt)
            {
                ((TextObject)container.FindObject(objectNameInReport))!.Text += $" {value}";
                return;
            }

            ((TextObject)container.FindObject(objectNameInReport))!.Text = value;
        }
        catch (Exception)
        {
            string camelCase = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(objectNameInReport);
            ((TextObject)container.FindObject(camelCase)).Text = value;
        }
    }

    public static GroupHeaderBand CreateGroupHeaderBand(this ReportPage page, string condition, string? name = null, int? heigh = null)
    {
        GroupHeaderBand groupHeaderBand = new GroupHeaderBand();
        page.Bands.Add(groupHeaderBand);
        groupHeaderBand.Height = Units.Centimeters * heigh ?? 1;
        groupHeaderBand.Condition = condition;
        groupHeaderBand.SortOrder = SortOrder.Ascending;
        groupHeaderBand.SetObjectName(name);

        return groupHeaderBand;
    }

    public static ReportPage CreateStaredParagraphTextGroupHeaderBand(this Base page, DataSourceBase? datasource, string groupTitle, string? contentText, DataSourceBase? childDatasource = null, string? childTitleText = null, string? childContentText = null)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        GroupHeaderBand groupHeaderBand = (GroupHeaderBand)templateReport.FindObject(ReportIdentifiers.TermGroupHeader);
        groupHeaderBand.Condition = groupTitle;
        groupHeaderBand.SetTextObjectValue(ReportIdentifiers.TermGroupTitleTxt, groupTitle);
        groupHeaderBand.SetTextObjectValue(ReportIdentifiers.OuterTermText, contentText);
        groupHeaderBand.SetObjectName();
        groupHeaderBand.Data.DataSource = datasource;
        groupHeaderBand.KeepTogether = true;
        groupHeaderBand.SortOrder = SortOrder.None;

        int groupNumber = 0;
        int termNumber = 0;
        groupHeaderBand.BeforePrint += (sender, e) =>
        {
            termNumber = 0;
        };

        var groupNumberTextObj = (TextObject)groupHeaderBand.FindObject(ReportIdentifiers.GroupNumberTxt);
        groupNumberTextObj.BeforePrint += (sender, e) =>
        {
            groupNumber++;
            (sender as TextObject).Text = $"{groupNumber}";
        };

        var termNumberTextObj = (TextObject)groupHeaderBand.FindObject(ReportIdentifiers.OuterTermNumberTxt);
        termNumberTextObj.BeforePrint += (sender, e) =>
        {
            termNumber++;
            (sender as TextObject).Text = $"{termNumber}.";
        };

        groupHeaderBand.Parent = page;
        return (ReportPage)page;
    }

    public static DataBand CreateBasicDataHeaderBand(this Base databand, string? title)
    {
        if (databand is not DataBand && databand is not GroupHeaderBand)
            throw new Exception("You can only add data header band in databand or data header band");

        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataHeaderBand dataHeaderBand = (DataHeaderBand)templateReport.FindObject("BasicDataHeaderBand");
        dataHeaderBand.SetTextObjectValue("DataHeaderTitle", title);
        dataHeaderBand.SetObjectName();
        dataHeaderBand.Parent = databand;
        dataHeaderBand.KeepWithData = true;
        return (DataBand)databand;
    }

    public static DataBand CreateBasicDataTitleBand(this Base databand, string? dataHeaderTitle)
    {
        if (databand is not DataBand && databand is not GroupHeaderBand)
            throw new Exception("You can only add data header band in databand or data header band");

        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataHeaderBand dataHeaderBand = (DataHeaderBand)templateReport.FindObject(ReportIdentifiers.TitleDataHeaderBand);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.TitleTxt, dataHeaderTitle);
        dataHeaderBand.SetObjectName();
        dataHeaderBand.Parent = databand;
        return (DataBand)databand;
    }

    public static DataBand CreateAccountInfoDataFooterBand(this Base databand, string? dataFooterText = null)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataFooterBand dataFooterBand = (DataFooterBand)templateReport.FindObject("AccountInfoDataFooter");
        dataFooterBand.SetTextObjectValue("DataFooterText", dataFooterText);
        dataFooterBand.SetObjectName();
        dataFooterBand.Parent = databand;
        return (DataBand)databand;
    }

    public static DataBand CreateFieldsDataBandBand(this Base page, DataSourceBase datasource, string fieldText, string fieldValue)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataBand dataBand = (DataBand)templateReport.FindObject(ReportIdentifiers.FieldsDataBand);
        dataBand.PrintIfDatasourceEmpty = false;
        dataBand.DataSource = datasource;
        dataBand.SetTextObjectValue(ReportIdentifiers.FieldNameTxt, fieldText);
        dataBand.SetTextObjectValue(ReportIdentifiers.FieldValueTxt, fieldValue);
        dataBand.SetObjectName();
        dataBand.Parent = page;
        return dataBand;
    }

    public static ReportPage CreateFormFieldsDataBandBand(this Base page, DataSourceBase datasource, string formName, string fieldText, string fieldValue)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        GroupHeaderBand groupHeaderBand = (GroupHeaderBand)templateReport.FindObject(ReportIdentifiers.FormGroupHeader);
        groupHeaderBand.Condition = formName;
        groupHeaderBand.SetTextObjectValue(ReportIdentifiers.FormNameTxt, formName);
        groupHeaderBand.SetTextObjectValue(ReportIdentifiers.FormFieldNameTxt, fieldText);
        groupHeaderBand.SetTextObjectValue(ReportIdentifiers.FormFieldValueTxt, fieldValue);
        groupHeaderBand.SetObjectName();
        groupHeaderBand.Data.DataSource = datasource;
        groupHeaderBand.KeepTogether = true;

        groupHeaderBand.SortOrder = SortOrder.Ascending;
        groupHeaderBand.Parent = page;
        return (ReportPage)page;
    }

    public static ReportPage CreateLabelsDataBand(this Base page, DataSourceBase datasource, string companyName, string qrCode, string serialNumber, string? primaryColor, string? secondaryColor)
    {
        Report templateReport = new Report();
        templateReport.Load(QRCodeTemplateReportPath);
        DataBand qrCodeDataband = (DataBand)templateReport.FindObject(ReportIdentifiers.QRCodeBand);
        qrCodeDataband.SetTextObjectValue(ReportIdentifiers.QRCompanyNameTxt, companyName);
        qrCodeDataband.SetTextObjectValue(ReportIdentifiers.QRSerialNumberTxt, serialNumber);
        qrCodeDataband.SetObjectName();
        qrCodeDataband.DataSource = datasource;

        BarcodeObject barcodeObject = (BarcodeObject)templateReport.FindObject(ReportIdentifiers.QRCodeObject);
        ShapeObject upperQRShape = (ShapeObject)templateReport.FindObject(ReportIdentifiers.Shape1);
        ShapeObject lowerQRShape = (ShapeObject)templateReport.FindObject(ReportIdentifiers.Shape2);

        //upperQRShape.FillColor = ColorTranslator.FromHtml(secondaryColor ?? "#000");
        //lowerQRShape.FillColor = ColorTranslator.FromHtml(secondaryColor ?? "#000");

        //barcodeObject.Barcode.Color = ColorTranslator.FromHtml(primaryColor ?? "#000");
        barcodeObject.Text = qrCode;
        qrCodeDataband.Parent = page;
        return (ReportPage)page;
    }

    public static DataBand CreateSpecificationsDataBandBand(this Base page, DataSourceBase datasource, string name, string description)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataBand dataBand = (DataBand)templateReport.FindObject("SpecificationBand");
        dataBand.PrintIfDatasourceEmpty = true;
        dataBand.DataSource = datasource;
        dataBand.SetTextObjectValue(ReportIdentifiers.NameTxt, name);
        dataBand.SetTextObjectValue(ReportIdentifiers.DescriptionTxt, description);
        dataBand.SetObjectName();
        dataBand.Parent = page;
        return dataBand;
    }

    public static DataBand CreateInstallmentsDataBandBand(this Base page, DataSourceBase datasource, string name, string percentage, string amount, string description)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);

        DataBand dataBand = (DataBand)templateReport.FindObject(ReportIdentifiers.InstallmentsBand);
        dataBand.PrintIfDatasourceEmpty = false;
        dataBand.DataSource = datasource;
        dataBand.SetObjectName();
        dataBand.Parent = page;
        dataBand.KeepTogether = true;
        dataBand.SetTextObjectValue(ReportIdentifiers.InstallmentsPlanName, name);
        dataBand.SetTextObjectValue(ReportIdentifiers.InstallmentsPlanPercentage, percentage);
        dataBand.SetTextObjectValue(ReportIdentifiers.InstallmentsPlanAmount, amount);
        dataBand.SetTextObjectValue(ReportIdentifiers.InstallmentsPlanDescription, description);

        return dataBand;
    }

    public static DataBand CreateInvoiceDataBandBand(this Base page, DataSourceBase datasource, InvoiceItemRequest request)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);

        DataBand dataBand = (DataBand)templateReport.FindObject(ReportIdentifiers.InvoiceDataBand);
        dataBand.PrintIfDatasourceEmpty = false;
        dataBand.DataSource = datasource;
        dataBand.SetObjectName();
        dataBand.Parent = page;
        dataBand.KeepTogether = true;
        dataBand.SetTextObjectValue(ReportIdentifiers.ItemIdTxt, request.Id);
        dataBand.SetTextObjectValue(ReportIdentifiers.PriceDescriptionTxt, request.Description);
        dataBand.SetTextObjectValue(ReportIdentifiers.QuantityTxt, request.Quantity);
        dataBand.SetTextObjectValue(ReportIdentifiers.UnitPriceTxt, request.UnitPrice);
        dataBand.SetTextObjectValue(ReportIdentifiers.ItemTotalTxt, request.Total);
        dataBand.SetTextObjectValue(ReportIdentifiers.ItemDiscountTxt, request.ItemDiscount);

        return dataBand;
    }

    public static DataBand CreateInvoiceServicesDataBandBand(this Base page, DataSourceBase datasource, string serviceName, string servicePrice)
    {
        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);

        DataBand dataBand = (DataBand)templateReport.FindObject(ReportIdentifiers.InvoiceٍServiceDataBand);
        dataBand.PrintIfDatasourceEmpty = true;
        dataBand.DataSource = datasource;
        dataBand.SetObjectName();
        dataBand.Parent = page;
        dataBand.KeepTogether = true;
        if (datasource.ChildObjects.Count > 0)
        {
            dataBand.Height = Units.Centimeters * 0.0f;
        }

        dataBand.SetTextObjectValue(ReportIdentifiers.ServicePriceTxt, servicePrice);
        dataBand.SetTextObjectValue(ReportIdentifiers.ServiceNameTxt, serviceName);

        return dataBand;
    }

    public static DataBand CreateInvoiceHeaderBand(this Base databand, string? dataHeaderTitle = null)
    {
        if (databand is not DataBand && databand is not GroupHeaderBand)
            throw new Exception("You can only add data header band in databand or data header band");

        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataHeaderBand dataHeaderBand = (DataHeaderBand)templateReport.FindObject(ReportIdentifiers.InvoiceDataHeaderBand);
        dataHeaderBand.SetTextObjectValue(ReportIdentifiers.InvoiceTitleTxt, dataHeaderTitle);
        dataHeaderBand.SetObjectName();
        dataHeaderBand.Parent = databand;
        return (DataBand)databand;
    }

    public static DataBand CreateInvoiceFooterBand(this Base databand, decimal vatPercentage, decimal vat, decimal total, string footerTxt)
    {
        if (databand is not DataBand && databand is not DataFooterBand)
            throw new Exception("You can only add data footer band in databand or data header band");

        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataFooterBand dataFooterBand = (DataFooterBand)templateReport.FindObject(ReportIdentifiers.InvoiceDataFooterBand);
        dataFooterBand.SetTextObjectValue(ReportIdentifiers.AmountInWordsTxt, $"( {decimal.ToInt32(total).ToWords(new CultureInfo("ar-SA"))} )");
        dataFooterBand.SetTextObjectValue(ReportIdentifiers.VatDescriptionTxt, $"({vatPercentage}%)", true);
        dataFooterBand.SetTextObjectValue(ReportIdentifiers.VatTxt, vat.ToString());
        dataFooterBand.SetTextObjectValue(ReportIdentifiers.TotalInvoiceTxt, total.ToString());
        dataFooterBand.SetTextObjectValue(ReportIdentifiers.InvoiceAccountInfoTxt, footerTxt);
        dataFooterBand.SetObjectName();
        dataFooterBand.Parent = databand;
        return (DataBand)databand;
    }

    public static DataBand CreateInvoiceItemsFooterBand(this Base databand, decimal totalVatEx, int discountPercentageTxt, decimal discountAmountTxt)
    {
        if (databand is not DataBand && databand is not DataFooterBand)
            throw new Exception("You can only add data footer band in databand or data header band");

        Report templateReport = new Report();
        templateReport.Load(TemplateReportPath);
        DataFooterBand dataFooterBand = (DataFooterBand)templateReport.FindObject(ReportIdentifiers.InvoiceItemsFooterBand);
        dataFooterBand.SetTextObjectValue(ReportIdentifiers.TotalVatExTxt, totalVatEx.ToString());
        dataFooterBand.SetTextObjectValue(ReportIdentifiers.DiscountDescriptionTxt, $"({discountPercentageTxt}%)", true);
        dataFooterBand.SetTextObjectValue(ReportIdentifiers.DiscountTxt, discountAmountTxt.ToString());
        dataFooterBand.SetObjectName();
        if (discountPercentageTxt == default)
        {
            TableObject dicountTable = (TableObject)dataFooterBand.FindObject(ReportIdentifiers.DiscountTable);
            float tableheigh = dicountTable.Rows[0].Height;
            dataFooterBand.Height -= tableheigh;
            dataFooterBand.RemoveChild(dicountTable);
        }

        dataFooterBand.Parent = databand;
        return (DataBand)databand;
    }

    public static void SetObjectName(this Base reportObject, string? name = null)
    {
        if (name != null)
        {
            reportObject.Name = name;
        }
        else
        {
            reportObject.CreateUniqueName();
        }
    }

    public static TextObject CreateText(this Base parent, string text, RectangleF? locationAndSize = null, string? dateformat = null, string? name = null)
    {
        TextObject textObj = new TextObject();
        textObj.Parent = parent;
        textObj.SetObjectName(name);
        textObj.Bounds = locationAndSize == null ? new RectangleF(0, 0, Units.Centimeters * 5, Units.Centimeters * 0.5f) :
            new RectangleF(
                Units.Centimeters * locationAndSize.GetValueOrDefault().X,
                Units.Centimeters * locationAndSize.GetValueOrDefault().Y,
                Units.Centimeters * locationAndSize.GetValueOrDefault().Width,
                Units.Centimeters * locationAndSize.GetValueOrDefault().Height);
        textObj.Text = text;
        if (dateformat != null)
        {
            DateFormat format = new DateFormat();
            format.Format = dateformat;
            textObj.Format = format;
        }

        return textObj;
    }

    public static TextObject SetPageNumber(this Base reportObject)
    {
        TextObject pageNumberTxt = new TextObject();
        pageNumberTxt.SetObjectName();
        pageNumberTxt.Width = 718.2f;
        pageNumberTxt.Height = 18.9f;
        pageNumberTxt.Text = "[Page#]/[TotalPages#]";
        pageNumberTxt.HorzAlign = HorzAlign.Center;
        pageNumberTxt.RightToLeft = true;
        pageNumberTxt.Font = new Font("Arial", 10);

        GeneralFormat generalFormat1 = new GeneralFormat();
        GeneralFormat generalFormat2 = new GeneralFormat();
        pageNumberTxt.Formats.Add(generalFormat1);
        pageNumberTxt.Formats.Add(generalFormat2);

        pageNumberTxt.Parent = reportObject;

        return pageNumberTxt;
    }

    public static TextObject SetFont(this TextObject textObj, string fontFamily, int size, FontStyle style)
    {
        textObj.Font = new Font(fontFamily, size, style);
        return textObj;
    }
    public static TextObject Center(this TextObject textObj, bool isHorizonal = true, bool isVertical = true)
    {
        if (isHorizonal)
            textObj.HorzAlign = HorzAlign.Center;

        if (isVertical)
            textObj.VertAlign = VertAlign.Center;

        return textObj;
    }

    public static TableObject CreateTable<T>(this T container, DataTable data, string? name = null)
        where T : BandBase
    {
        TableObject table = new TableObject();
        int rows = data.Rows.Count;
        int cols = data.Columns.Count;

        table.Anchor = System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Bottom;
        table.SetObjectName(name);
        table.RowCount = rows;
        table.ColumnCount = cols;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                table[j, i].Text = data.Rows[i][j].ToString();
                table[j, i].Border.Lines = BorderLines.All;
            }
        }

        container.Objects.Add(table);
        return table;
    }

    private static Report CreateJsonConnection(this Report report, object data)
    {
        var builder = BuildJsonConnection(data);
        var conn = new JsonDataSourceConnection();
        conn.ConnectionString = builder.ConnectionString;

        report.Dictionary.Connections.Add(conn);
        return report;
    }

    public static Report UpdateJsonConnection(this Report report, object data)
    {
        var builder = BuildJsonConnection(data);
        report.Dictionary.Connections[0].ConnectionString = builder.ConnectionString;
        return report;
    }

    private static JsonDataSourceConnectionStringBuilder BuildJsonConnection(object data)
    {
        var builder = new JsonDataSourceConnectionStringBuilder();
        builder.Json = JsonConvert.SerializeObject(data);
        builder.JsonSchema = new JSchemaGenerator().Generate(data.GetType()).ToString();
        builder.Encoding = "utf-8";
        builder.SimpleStructure = true;
        return builder;
    }

    public static DataSourceBase Init(this Report report, object data, string? dataSourceName)
    {
        var list = new List<object>
        {
            data
        };
        report.Init(list, dataSourceName);

        return report.GetDataSource(dataSourceName);
    }

    public static DataSourceBase Init(this Report report, ICollection data, string? dataSourceName)
    {
        report.RegisterStyles();
        if (data != null)
        {
            report.RegisterData(data, dataSourceName, 10);
            report.GetDataSource(dataSourceName).Enabled = true;
        }

        return report.GetDataSource(dataSourceName);
    }

    public static void RegisterAllDataSources(this Report report, object response)
    {
        if (response == null)
            throw new ArgumentNullException(nameof(response));

        Type responseType = response.GetType();
        var properties = responseType.GetProperties();

        foreach (var property in properties)
        {
            object? propertyValue = property.GetValue(response);

            if (propertyValue is IEnumerable collection)
            {
                string dataSourceName = property.Name;
                report.RegisterData(collection, dataSourceName);
                report.GetDataSource(dataSourceName).Enabled = true;
            }
        }
    }

    public static Report RegisterStyles(this Report report)
    {
        List<Style> styleDictionary = new List<Style>();

        // Create styles
        Style titleStyle = new Style();
        titleStyle.Name = "Title";
        titleStyle.Font = new Font("Calibri", 12, FontStyle.Bold);
        styleDictionary.Add(titleStyle);

        Style subtitleStyle = new Style();
        subtitleStyle.Name = "Subtitle";
        subtitleStyle.Font = new Font("Calibri", 11, FontStyle.Bold);
        styleDictionary.Add(subtitleStyle);

        Style paragraphStyle = new Style();
        paragraphStyle.Name = "Paragraph";
        paragraphStyle.Font = new Font("Calibri", 11);
        styleDictionary.Add(paragraphStyle);

        Style tableCellNormalStyle = new Style();
        tableCellNormalStyle.Name = "TableCellNormal";
        tableCellNormalStyle.Border.Lines = BorderLines.All;
        tableCellNormalStyle.Border.Color = Color.LightGray;
        tableCellNormalStyle.Font = new Font("Calibri", 11);
        styleDictionary.Add(tableCellNormalStyle);

        Style tableCellBoldStyle = new Style();
        tableCellBoldStyle.Name = "TableCellBold";
        tableCellBoldStyle.Border.Lines = BorderLines.All;
        tableCellBoldStyle.Border.Color = Color.LightGray;
        tableCellBoldStyle.Font = new Font("Calibri", 11, FontStyle.Bold);
        styleDictionary.Add(tableCellBoldStyle);

        Style tableCellEvenNormalStyle = new Style();
        tableCellEvenNormalStyle.Name = "TableCellEvenNormal";
        tableCellEvenNormalStyle.Fill = new SolidFill(Color.WhiteSmoke);
        tableCellEvenNormalStyle.Font = new Font("Calibri", 11);
        styleDictionary.Add(tableCellEvenNormalStyle);

        Style tableCellEvenBoldStyle = new Style();
        tableCellEvenBoldStyle.Name = "TableCellEvenBold";
        tableCellEvenBoldStyle.Fill = new SolidFill(Color.WhiteSmoke);
        tableCellEvenBoldStyle.Font = new Font("Calibri", 11, FontStyle.Bold);
        styleDictionary.Add(tableCellEvenBoldStyle);

        // Add styles to the report's style dictionary
        report.Styles.AddRange(styleDictionary.ToArray());
        return report;
    }

    public static Stream ToPdf(this Report report)
    {
        MemoryStream pdfStream = new MemoryStream();
        var pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport();
        pdfExport.Export(report, pdfStream);
        pdfStream.Seek(0, SeekOrigin.Begin);
        return pdfStream;
    }

    public static Stream ToImage(this Report report)
    {
        MemoryStream imageStream = new MemoryStream();
        ImageExport image = new ImageExport();
        image.ImageFormat = ImageExportFormat.Jpeg;
        report.Export(image, imageStream);
        imageStream.Seek(0, SeekOrigin.Begin);
        return imageStream;
    }

    // Sample Reports
    public static Report GetSimpleListReport(this Report report, IEnumerable data, string title)
    {
        report.RegisterData(data, "contract");
        var page = report.CreatePage().CreateTitle(title);

        var datasource = report.GetDataSource(title);
        datasource.Enabled = true;
        page.CreateBand(datasource)
            .CreateTable(data.ConvertToDataTable());

        return report;
    }

    public static Report GetMasterDetailReport()
    {
        Report report = new Report();

        // register all data tables and relations
        //report.RegisterData(dataSet);

        // enable the "Categories" and "Items" tables to use it in the report
        report.GetDataSource("Categories").Enabled = true;
        report.GetDataSource("Items").Enabled = true;
        // enable relation between two tables
        report.Dictionary.UpdateRelations();

        // add report page
        ReportPage page = new ReportPage();
        report.Pages.Add(page);
        // always give names to objects you create. You can use CreateUniqueName method to do this;
        // call it after the object is added to a report.
        page.CreateUniqueName();

        // create master data band
        DataBand masterDataBand = new DataBand();
        page.Bands.Add(masterDataBand);
        masterDataBand.CreateUniqueName();
        masterDataBand.DataSource = report.GetDataSource("Categories");
        masterDataBand.Height = FastReport.Utils.Units.Centimeters * 0.5f;

        // create category name text
        TextObject categoryText = new TextObject();
        categoryText.Parent = masterDataBand;
        categoryText.CreateUniqueName();
        categoryText.Bounds = new RectangleF(0, 0, Units.Centimeters * 5, Units.Centimeters * 0.5f);
        categoryText.Font = new Font("Arial", 10, FontStyle.Bold);
        categoryText.Text = "[Categories.CategoryName]";

        // create detail data band
        DataBand detailDataBand = new DataBand();
        masterDataBand.Bands.Add(detailDataBand);
        detailDataBand.CreateUniqueName();
        detailDataBand.DataSource = report.GetDataSource("Items");
        detailDataBand.Height = Units.Centimeters * 0.5f;
        // set sort by product name
        detailDataBand.Sort.Add(new Sort("[Items.ItemName]"));

        // create product name text
        TextObject productText = new TextObject();
        productText.Parent = detailDataBand;
        productText.CreateUniqueName();
        productText.Bounds = new RectangleF(Units.Centimeters * 1, 0, Units.Centimeters * 10, Units.Centimeters * 0.5f);
        productText.Text = "[Items.ItemName]";

        return report;
    }

    public static Report GetGroupReport()
    {
        Report report = new Report();
        //report.RegisterData(dataSet);
        report.GetDataSource("Items").Enabled = true;

        // create group header
        var groupHeaderBand = report.CreatePage().CreateGroupHeaderBand("[Items.ItemName].Substring(0,1)");

        groupHeaderBand.CreateText("[[Items.ItemName].Substring(0,1)]", new RectangleF(0, 0, 10, 1))
        .SetFont("Arial", 14, FontStyle.Bold).Center()
        .Fill = new LinearGradientFill(Color.OldLace, Color.Moccasin, 90, 0.5f, 1);

        var dataBand = groupHeaderBand.CreateBand(0.5f, report.GetDataSource("Items"));
        dataBand.CreateText("[Items.ItemName]", new RectangleF(0, 0, 10, 0.5f));

        // create group footer
        groupHeaderBand.GroupFooter = new GroupFooterBand();
        groupHeaderBand.GroupFooter.CreateUniqueName();
        groupHeaderBand.GroupFooter.Height = Units.Centimeters * 1;

        // create total
        Total groupTotal = new Total();
        groupTotal.Name = "TotalRows";
        groupTotal.TotalType = TotalType.Count;
        groupTotal.Evaluator = dataBand;
        groupTotal.PrintOn = groupHeaderBand.GroupFooter;
        report.Dictionary.Totals.Add(groupTotal);

        TextObject totalText = groupHeaderBand.GroupFooter.CreateText("Rows: [TotalRows]", new RectangleF(0, 0, 10, 0.5f));
        totalText.HorzAlign = HorzAlign.Right;
        totalText.Border.Lines = BorderLines.Top;

        return report;
    }

    public static Report GetNestedGroupsReport()
    {
        Report report = new Report();

        // register all data tables and relations
        //report.RegisterData(dataSet);

        // enable the "Items" table to use it in the report
        report.GetDataSource("Items").Enabled = true;

        // add report page
        ReportPage page = new ReportPage();
        report.Pages.Add(page);
        // always give names to objects you create. You can use CreateUniqueName method to do this;
        // call it after the object is added to a report.
        page.CreateUniqueName();

        // create group header
        GroupHeaderBand groupHeaderBand = new GroupHeaderBand();
        page.Bands.Add(groupHeaderBand);
        groupHeaderBand.Height = Units.Centimeters * 1;
        groupHeaderBand.Condition = "[Items.ItemName].Substring(0,1)";

        // create group text
        TextObject groupText = new TextObject();
        groupText.Parent = groupHeaderBand;
        groupText.CreateUniqueName();
        groupText.Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 1);
        groupText.Font = new Font("Arial", 14, FontStyle.Bold);
        groupText.Text = "[[Items.ItemName].Substring(0,1)]";
        groupText.VertAlign = VertAlign.Center;
        groupText.Fill = new LinearGradientFill(Color.OldLace, Color.Moccasin, 90, 0.5f, 1);

        // create nested group header
        GroupHeaderBand nestedGroupBand = new GroupHeaderBand();
        groupHeaderBand.NestedGroup = nestedGroupBand;
        nestedGroupBand.Height = Units.Centimeters * 0.5f;
        nestedGroupBand.Condition = "[Items.ItemName].Substring(0,2)";

        // create nested group text
        TextObject nestedText = new TextObject();
        nestedText.Parent = nestedGroupBand;
        nestedText.CreateUniqueName();
        nestedText.Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 0.5f);
        nestedText.Font = new Font("Arial", 10, FontStyle.Bold);
        nestedText.Text = "[[Items.ItemName].Substring(0,2)]";

        // create data band
        DataBand dataBand = new DataBand();
        // connect it to inner group
        nestedGroupBand.Data = dataBand;
        dataBand.CreateUniqueName();
        dataBand.DataSource = report.GetDataSource("Items");
        dataBand.Height = Units.Centimeters * 0.5f;
        // set sort by product name
        dataBand.Sort.Add(new Sort("[Items.ItemName]"));

        // create product name text
        TextObject productText = new TextObject();
        productText.Parent = dataBand;
        productText.CreateUniqueName();
        productText.Bounds = new RectangleF(Units.Centimeters * 0.5f, 0, Units.Centimeters * 9.5f, Units.Centimeters * 0.5f);
        productText.Text = "[Items.ItemName]";

        // create group footer for outer group
        groupHeaderBand.GroupFooter = new GroupFooterBand();
        groupHeaderBand.GroupFooter.CreateUniqueName();
        groupHeaderBand.GroupFooter.Height = Units.Centimeters * 1;

        // create total
        Total groupTotal = new Total();
        groupTotal.Name = "TotalRows";
        groupTotal.TotalType = TotalType.Count;
        groupTotal.Evaluator = dataBand;
        groupTotal.PrintOn = groupHeaderBand.GroupFooter;
        report.Dictionary.Totals.Add(groupTotal);

        // show total in the group footer
        TextObject totalText = new TextObject();
        totalText.Parent = groupHeaderBand.GroupFooter;
        totalText.CreateUniqueName();
        totalText.Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 0.5f);
        totalText.Text = "Rows: [TotalRows]";
        totalText.HorzAlign = HorzAlign.Right;
        totalText.Border.Lines = BorderLines.Top;

        return report;
    }

    public static Report GetSubreportReport()
    {
        Report report = new Report();

        // register all data tables and relations
        //report.RegisterData(dataSet);

        // enable the "Items" and "Suppliers" tables to use it in the report
        report.GetDataSource("Items").Enabled = true;
        report.GetDataSource("Suppliers").Enabled = true;

        // add report page
        ReportPage page = new ReportPage();
        report.Pages.Add(page);
        // always give names to objects you create. You can use CreateUniqueName method to do this;
        // call it after the object is added to a report.
        page.CreateUniqueName();

        // create title band
        page.ReportTitle = new ReportTitleBand();
        // native FastReport unit is screen pixel, use conversion 
        page.ReportTitle.Height = Units.Centimeters * 1;
        page.ReportTitle.CreateUniqueName();

        // create two title text objects
        TextObject titleText1 = new TextObject();
        titleText1.Parent = page.ReportTitle;
        titleText1.CreateUniqueName();
        titleText1.Bounds = new RectangleF(0, 0, Units.Centimeters * 8, Units.Centimeters * 1);
        titleText1.Font = new Font("Arial", 14, FontStyle.Bold);
        titleText1.Text = "Items";
        titleText1.HorzAlign = HorzAlign.Center;

        TextObject titleText2 = new TextObject();
        titleText2.Parent = page.ReportTitle;
        titleText2.CreateUniqueName();
        titleText2.Bounds = new RectangleF(Units.Centimeters * 9, 0, Units.Centimeters * 8, Units.Centimeters * 1);
        titleText2.Font = new Font("Arial", 14, FontStyle.Bold);
        titleText2.Text = "Suppliers";
        titleText2.HorzAlign = HorzAlign.Center;

        // create report title's child band that will contain subreports
        ChildBand childBand = new ChildBand();
        page.ReportTitle.Child = childBand;
        childBand.CreateUniqueName();
        childBand.Height = Units.Centimeters * 0.5f;

        // create the first subreport
        SubreportObject subreport1 = new SubreportObject();
        subreport1.Parent = childBand;
        subreport1.CreateUniqueName();
        subreport1.Bounds = new RectangleF(0, 0, Units.Centimeters * 8, Units.Centimeters * 0.5f);

        // create subreport's page
        ReportPage subreportPage1 = new ReportPage();
        report.Pages.Add(subreportPage1);
        // connect subreport to page
        subreport1.ReportPage = subreportPage1;

        // create report on the subreport's page
        DataBand dataBand = new DataBand();
        subreportPage1.Bands.Add(dataBand);
        dataBand.CreateUniqueName();
        dataBand.DataSource = report.GetDataSource("Items");
        dataBand.Height = Units.Centimeters * 0.5f;

        TextObject productText = new TextObject();
        productText.Parent = dataBand;
        productText.CreateUniqueName();
        productText.Bounds = new RectangleF(0, 0, Units.Centimeters * 8, Units.Centimeters * 0.5f);
        productText.Text = "[Items.ItemName]";


        // create the second subreport
        SubreportObject subreport2 = new SubreportObject();
        subreport2.Parent = childBand;
        subreport2.CreateUniqueName();
        subreport2.Bounds = new RectangleF(Units.Centimeters * 9, 0, Units.Centimeters * 8, Units.Centimeters * 0.5f);

        // create subreport's page
        ReportPage subreportPage2 = new ReportPage();
        report.Pages.Add(subreportPage2);
        // connect subreport to page
        subreport2.ReportPage = subreportPage2;

        // create report on the subreport's page
        DataBand dataBand2 = new DataBand();
        subreportPage2.Bands.Add(dataBand2);
        dataBand2.CreateUniqueName();
        dataBand2.DataSource = report.GetDataSource("Suppliers");
        dataBand2.Height = Units.Centimeters * 0.5f;

        // create supplier name text
        TextObject supplierText = new TextObject();
        supplierText.Parent = dataBand2;
        supplierText.CreateUniqueName();
        supplierText.Bounds = new RectangleF(0, 0, Units.Centimeters * 8, Units.Centimeters * 0.5f);
        supplierText.Text = "[Suppliers.CompanyName]";

        return report;
    }

    public static Report GetTableReport(object data)
    {
        Report report = new Report();
        report.CreatePage().CreateBand().CreateTable(data.ConvertToDataTable());
        return report;
    }

    //static void Main(string[] args)
    //{

    //    Console.WriteLine("Welcome! Choose report type, please:\n" +
    //        "1 - SimpleList\n" +
    //        "2 - MasterDetail\n" +
    //        "3 - Group\n" +
    //        "4 - NestedGroups\n" +
    //        "5 - Subreport\n" +
    //        "6 - Table");
    //    Report report;
    //    //while 1,2,3,4 is not pressed
    //    char key;
    //    do
    //    {
    //        key = Console.ReadKey().KeyChar;
    //    }
    //    while ((int)key < 49 || (int)key > 54);

    //    // create report instance
    //    switch (key)
    //    {
    //        case '1': report = GetSimpleListReport(); break;
    //        case '2': report = GetMasterDetailReport(); break;
    //        case '3': report = GetGroupReport(); break;
    //        case '4': report = GetNestedGroupsReport(); break;
    //        case '5': report = GetSubreportReport(); break;
    //        case '6': report = GetTableReport(); break;
    //        default: report = GetSimpleListReport(); break;
    //    }

    //    // prepare the report
    //    report.Prepare();

    //    // save prepared report
    //    if (!Directory.Exists(outFolder))
    //        Directory.CreateDirectory(outFolder);
    //    report.SavePrepared(Path.Combine(outFolder, "Prepared Report.fpx"));

    //    // export to image
    //    ImageExport image = new ImageExport();
    //    image.ImageFormat = ImageExportFormat.Jpeg;
    //    report.Export(image, Path.Combine(outFolder, "report.jpg"));

    //    // free resources used by report
    //    report.Dispose();

    //    Console.WriteLine("\nPrepared report and report exported as image have been saved into the 'out' folder.");
    //    Console.WriteLine("Press any key to exit...");
    //    Console.ReadKey();
    //}
}
