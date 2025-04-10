using FastReport;
using ShowMatic.Server.Application.Extensions;
using System.Globalization;
using Reporting.Templates;
using ShowMatic.Server.Application.Catalog.Contracts.GetDetails;
using System.Collections.Generic;
using FastReport.Data;
using ShowMatic.Server.Domain.Catalog;
using System.Diagnostics.Contracts;
using Showmatics.Application.Catalog.Labels.Print;

namespace Reporting;
public static class ContractReport
{
    public static Stream DynamicReport(PrintContractDetailsResponse contract)
    {
        using (Report report = new Report())
        {
            string mainDataSourceName = nameof(contract);
            FixFieldsEmptySpace(contract);

            var datasource = report.Init(contract, mainDataSourceName);
            datasource.Enabled = true;

            var page = report.CreatePage();
            var databand = page.CreateBand();
            databand.PrintIfDatasourceEmpty = true;
            databand.PrintIfDetailEmpty = true;

            databand.CreateContractHeaderBand(new Models.ContractHeaderRequest
            {
                DocumentNumberTxt = contract.SerialNumber,
                DocumentNameTxt = contract.IsOffer ? contract.Type.DisplayNameOnOffer : contract.Type.DisplayNameOnContract,
                FirstPartyNameTxt = contract.TenantInfo.Name,
                FirstPartyAddressTxt = contract.TenantInfo.Address,
                FirstPartyRegistrationNoTxt = contract.TenantInfo.RegistrationNo,
                SecondPartyNameTxt = contract.Customer?.Name,
                SecondPartyAddressTxt = contract.Address?.FullAddress,
                SecondPartyIdTxt = contract.Customer?.IdentityNumber,
                SecondPartyIdLabelTxt = string.IsNullOrEmpty(contract.Customer?.IdentityNumber) ? string.Empty : contract.Customer.IsCompany == true ? ": سجل تجاري" : ": الهوية",
                SecondPartyPhoneTxt = contract.Customer?.PhoneNumber,
                FirstPartyOwnerTitleTxt = contract.TenantInfo.Title,
                OwnerNameTxt = contract.TenantInfo.OwnerName,
                Id = contract.SerialNumber,
                CustomerId = contract.CustomerId?.ToString(),
                IncludeParties = contract.Type.ContractSetting.ShowParties,
                ReferenceSerialNumber = contract.Reference?.SerialNumber,

            });

            if (contract.Type.ContractSetting.ShowFields)
            {
                // Contract Fields
                string fieldsDataSourceName = Shared.GetPath(() => contract.ContractFields);
                var fieldsDataSource = report.GetDataSource(fieldsDataSourceName);
                if (contract.Type.ContractSetting.ShouldGroupFields)
                {
                    page.CreateFormFieldsDataBandBand(
                        fieldsDataSource,
                        $"[{fieldsDataSourceName}.FormName]",
                        $"[{fieldsDataSourceName}.FieldText]",
                        $"[{fieldsDataSourceName}.FieldValue]");
                }
                else
                {
                    page.CreateFieldsDataBandBand(
                        fieldsDataSource,
                        $"[{fieldsDataSourceName}.FieldText]",
                        $"[{fieldsDataSourceName}.FieldValue]").CreateBasicDataHeaderBand("المواصفات الأساسية لتنفيذ الأتفاقية");
                }
            }

            if (contract.Type.ContractSetting.ShowItemsInfo)
            {
                // Items
                string productsDataSourceName = Shared.GetPath(() => contract.Items);
                var productsDataSource = report.GetDataSource(productsDataSourceName);
                page.CreateSpecificationsDataBandBand(
                    productsDataSource,
                    $"[{productsDataSourceName}.Item.Name]",
                    $"[{productsDataSourceName}.Item.Description]").CreateBasicDataHeaderBand("المواصفات الفنية");
            }

            if (contract.Type.ContractSetting.ShowInvoice)
            {
                // Invoice

                if (contract.Items != null)
                {
                    decimal invoiceTotalVatExc = contract.Total;
                    decimal vatRate = 15;
                    decimal vatAmount = contract.TotalVatInc - contract.TotalAfterDiscount;
                    decimal totalItems = contract.Items.Sum(x => x.Total);
                    decimal discountAmount = contract.Total - contract.TotalAfterDiscount;

                    string invoiceDataSourceName = "InvoiceData";
                    var combinedInvoiceData = contract.Items.ToList();
                    report.RegisterData(combinedInvoiceData.AsEnumerable(), invoiceDataSourceName, 2);

                    string servicesDataSourceName = Shared.GetPath(() => contract.ContractServices);
                    var servicesDataSource = report.GetDataSource(servicesDataSourceName);

                    // Set the data source
                    var invoiceDataSource = report.GetDataSource(invoiceDataSourceName);
                    invoiceDataSource.Enabled = true;
                    page.CreateInvoiceDataBandBand(
                        invoiceDataSource, new Models.InvoiceItemRequest
                        {
                            Id = $"[{invoiceDataSourceName}.Label]",
                            Description = $"[{invoiceDataSourceName}.Item.Name]",
                            UnitPrice = $"[{invoiceDataSourceName}.UnitPrice]",
                            Quantity = $"[{invoiceDataSourceName}.Quantity]",
                            Total = $"[{invoiceDataSourceName}.Total]",
                            ItemDiscount = $"[{invoiceDataSourceName}.Discount]",
                        }).CreateInvoiceHeaderBand()
                        .CreateInvoiceItemsFooterBand(totalItems, contract.DiscountPercentage, discountAmount);
                    page.CreateInvoiceServicesDataBandBand(
                        servicesDataSource,
                        $"[{servicesDataSourceName}.Description] [{servicesDataSourceName}.Percentage]",
                        $"[{servicesDataSourceName}.Price]")
                        .CreateInvoiceFooterBand(vatRate, vatAmount, contract.TotalVatInc, $"رقم {contract.TenantInfo.BankAccount} في {contract.TenantInfo.BankName}");

                }
            }

            if (contract.Type.ContractSetting.ShowInstalments)
            {
                // Installments
                string installmentDataSourceName = Shared.GetPath(() => contract.InstallmentsPlan);
                var installmentDataSource = report.GetDataSource(installmentDataSourceName);
                page.CreateInstallmentsDataBandBand(
                    installmentDataSource,
                    $"[{installmentDataSourceName}.Name]",
                    $"[{installmentDataSourceName}.Percentage]%",
                    $"[{installmentDataSourceName}.Amount]",
                    $"[{installmentDataSourceName}.Description]")
                    .CreateBasicDataHeaderBand("طريقة الدفع")
                    .CreateAccountInfoDataFooterBand($"حساب {contract.TenantInfo.Name} {contract.TenantInfo.BankAccount} {contract.TenantInfo.BankName}");

            }

            if (contract.Type.ContractSetting.ShowTerms)
            {
                // Terms
                // SetTermsParameters(contract);
                string termsDataSourceName = Shared.GetPath(() => contract.ContractTerms);
                var termsDataSource = report.GetDataSource(termsDataSourceName);
                var termsChildrenDataSource = report.GetDataSource($"{termsDataSourceName}.term.children");

                page.CreateStaredParagraphTextGroupHeaderBand(
                    termsDataSource,
                    $"[{termsDataSourceName}.Term.Group.Name]",
                    $"[{termsDataSourceName}.TermText]",
                    termsChildrenDataSource,
                    $"[{termsDataSourceName}.Term.ParentId]",
                    $"[{termsDataSourceName}.Term.children.name]");
            }

            if (contract.Type.ContractSetting.ShowSignature)
            {
                // Signature
                page.CreateSignatureBand(
                    firstParty: contract.TenantInfo.Name,
                    secondParty: contract.Customer?.Name ?? string.Empty,
                    description: "حرر هذا العقد من نسختين أصليتين لكل طرف نسخة للعمل بموجبها وعلى هذا تم الاتفاق وجرى التوقيع");
            }

            if (contract.Type.ContractSetting.SetHeaderImage)
            {
                page.CreateHeader(contract.TenantInfo.HeaderUrl);
            }
            else
            {
                page.CreateHeader();
            }

            page.CreateFooter();

            report.Prepare();
            report.Save("Reports/Prepared11.frx");
            return report.ToPdf();
        }
    }

    private static void FixFieldsEmptySpace(PrintContractDetailsResponse contract)
    {
        List<PrintContractDetailsFieldResponse> fields = new();
        if (contract.Type.ContractSetting.ShouldGroupFields)
        {
            foreach (var group in contract.ContractFields.GroupBy(x => x.FormName))
            {
                var groupFields = group.ToList();
                fields.AddRange(FixFieldsEmptySpace(groupFields, group.Key));
            }
        }
        else
        {
            fields.AddRange(FixFieldsEmptySpace(contract.ContractFields.ToList()));
        }

        contract.ContractFields = fields.OrderByDescending(x => x.FieldId).ToList();
    }

    private static List<PrintContractDetailsFieldResponse> FixFieldsEmptySpace(List<PrintContractDetailsFieldResponse> fields, string? groupName = null)
    {
        int fieldPlaceHolder = fields.Count % 3;
        if (fieldPlaceHolder != 0)
        {
            for (int i = 0; i < 3 - fieldPlaceHolder; i++)
            {
                fields.Add(new PrintContractDetailsFieldResponse() { FormName = groupName ?? string.Empty });
            }
        }

        return fields;
    }

    public static Stream PrintLabels(PrinLabelReportResponse response)
    {
        using (Report report = new Report())
        {
            var page = report.CreatePage();
            string mainDataSourceName = nameof(response);
            var datasource = report.Init(response, mainDataSourceName);

            string labelsDataSourceName = Shared.GetPath(() => response.Labels);
            var labelsDataSource = report.GetDataSource(labelsDataSourceName);

            page.CreateLabelsDataBand(
                labelsDataSource,
                response.TenantInfo.Name,
                $"[{labelsDataSourceName}.QRCodeInfo]",
                $"[{labelsDataSourceName}.Code]",
                response.TenantInfo.PrimaryColor,
                response.TenantInfo.SecondaryColor);

            report.Prepare();
            return report.ToPdf();
        }
    }
}
