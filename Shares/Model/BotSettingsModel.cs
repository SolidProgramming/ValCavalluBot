using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class BotSettingsModel
    {
        public CredentialsModel Credentials = new();
        public string Server { get; set; }
    }
}
