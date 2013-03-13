using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orchard.WebApi.Common
{
    public interface IDto<RecordType> where RecordType:class 
    {
        RecordType ToEntity();
        object RecordId { get; set; }
    }
}
