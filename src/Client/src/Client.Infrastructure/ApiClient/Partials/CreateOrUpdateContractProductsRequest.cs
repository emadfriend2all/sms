using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Showmatics.Blazor.Client.Infrastructure.ApiClient;
public partial class CreateOrUpdateContractItemsRequest
{
    public bool ReadOnly { get; set; }
    public int? AddedByFieldId { get; set; }
}
