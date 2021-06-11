using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shares.Enum;

namespace Shares.Model
{
    public class BotSettingsModel
    {
        public HowrseCredentialsModel Credentials = new();
        public string Server { get; set; }
        public HowrseAccountType HowrseAccountType { get; set; }
    }
}
