using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shares.Model
{
    public class GeneralSettingsModel
    {
        public GeneralCredentialsModel GeneralCredentials { get; set; } = new();
    }
}
