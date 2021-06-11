using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public struct HowrseServerLoginResponseModel
    {
        public FieldsErrors fieldsErrors { get; set; }
        public string errorsText { get; set; }
        public string messageText { get; set; }
        public List<string> errors { get; set; }
        public object message { get; set; }
    }
}
