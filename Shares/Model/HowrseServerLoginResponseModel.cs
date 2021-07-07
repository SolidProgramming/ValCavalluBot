using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public struct HowrseServerLoginResponseModel
    {
        public FieldsErrors FieldsErrors { get; set; }
        public string ErrorsText { get; set; }
        public string MessageText { get; set; }
        public List<string> Errors { get; set; }
        public object Message { get; set; }
    }
}
