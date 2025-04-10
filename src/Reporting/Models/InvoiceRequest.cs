using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reporting.Models;
public class InvoiceItemRequest
{
    public string? Id { get; set; }
    public string? Description { get; set; }
    public string? Quantity { get; set; }
    public string? ItemDiscount { get; set; }
    public string? UnitPrice { get; set; }
    public string? Total { get; set; }
}
