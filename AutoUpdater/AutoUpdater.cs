using AutoUpdaterClient.Model;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO.Compression;
using System.Net;
using System.ComponentModel;

namespace AutoUpdaterClient
{
    public class AutoUpdater
    {
        public static event Action<DownloadProgressChangedEventArgs> OnDownloadProgressChanged;
        public static event Action<AsyncCompletedEventArgs> OnDownloadCompleted;
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
            string tempPathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "ValCavalluBot", "Temp");
            string extractDest = Path.Combine(tempPathDirectory, "update");
            string archivePath = Path.Combine(tempPathDirectory, "download");
            string assemlbyPath = Path.Combine(archivePath, "valcavallubot.zip");

            CheckDirectoryExists(tempPathDirectory);
            CheckDirectoryExists(extractDest);
            CheckDirectoryExists(archivePath);

            using WebClient wc = new();

            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadFileAsync(new Uri(updateDetails.AssemblyUrl), assemlbyPath);
        }

        private static void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            OnDownloadCompleted(e);
        }

        private static void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage % 3 == 1)
            {
                OnDownloadProgressChanged(e);
            }
        }

        private static void CheckDirectoryExists(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

    }
}
