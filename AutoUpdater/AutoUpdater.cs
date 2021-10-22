using AutoUpdaterClient.Model;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO.Compression;
using System.Net;
using System.ComponentModel;
using System.Diagnostics;

namespace AutoUpdaterClient
{
    public static class AutoUpdater
    {
        public static event Action<DownloadProgressChangedEventArgs> OnDownloadProgressChanged;
        public static event Action<AsyncCompletedEventArgs> OnDownloadCompleted;
        private static string TempPathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "ValCavalluBot", "Temp");
        private static readonly string archivePath = Path.Combine(TempPathDirectory, "download");
        private static readonly string assemlbyPath = Path.Combine(archivePath, "valcavallubot.zip");
        private static readonly string extractDest = Path.Combine(TempPathDirectory, "update");

        public static async Task<(bool, UpdateModel)> CheckForUpdates(string AssemblyVersion)
        {
            using HttpClient client = new();

            string result = await client.GetStringAsync("http://95.216.164.108/valcavallubot/latest.xml");

            if (result.Length > 0)
            {
                UpdateModel updateDetails = ParseUpdateModel(result);

                if (new Version(updateDetails.Version) > new Version(AssemblyVersion))
                {
                    return (true, updateDetails);
                }
            }

            return default;
        }
        private static UpdateModel ParseUpdateModel(string xmlData)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(UpdateModel));
                var rdr = new StringReader(xmlData);

                return (UpdateModel)Convert.ChangeType(serializer.Deserialize(rdr), typeof(UpdateModel), System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static void DownloadUpdate(UpdateModel updateDetails)
        {
            CheckAllDirectorysExists();
            using WebClient wc = new();

            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadFileAsync(new Uri(updateDetails.AssemblyUrl), assemlbyPath);
        }
        public static void UnpackUpdate()
        {
            CheckAllDirectorysExists();

            ZipFile.ExtractToDirectory(assemlbyPath, extractDest, true);
        }        
        private static void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            OnDownloadCompleted(e);
        }
        private static void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage % 5 == 1)
            {
                OnDownloadProgressChanged(e);
            }
        }
        private static void CheckAllDirectorysExists()
        {
            CheckDirectoryExists(TempPathDirectory);
            CheckDirectoryExists(archivePath);
            CheckDirectoryExists(extractDest);
        }
        private static void CheckDirectoryExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

    }
}
