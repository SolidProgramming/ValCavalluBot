using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using AutoUpdaterClient;
using AutoUpdaterClient.Model;

namespace ValCavalluBot.Services
{
    public class AutoUpdaterService
    {
        public async Task<(bool, UpdateModel)> CheckForUpdates(string AssemblyVersion)
        {
            return await AutoUpdater.CheckForUpdates(AssemblyVersion);
        }
        public void DownloadUpdate(UpdateModel updateDetails)
        {
            AutoUpdater.DownloadUpdate(updateDetails);
        }
        public void UnpackUpdate()
        {
            AutoUpdater.UnpackUpdate();
        }
    }
}
