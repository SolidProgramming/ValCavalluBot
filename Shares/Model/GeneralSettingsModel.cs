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
        public int WaitTimeFrom { get; set; } = 100;
        public int WaitTimeTo { get; set; } = 1100;
    }
}
