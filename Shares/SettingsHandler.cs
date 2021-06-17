using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Shares.Enum;
using Shares.Model;

namespace Shares
{
    public static class SettingsHandler
    {
        private static readonly string SavePathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "ValCavalluBot");
        private static readonly string GeneralSettingsFilePath = Path.Combine(SavePathDirectory, "GeneralSettings");
        private static readonly string BotSettingsFilePath = Path.Combine(SavePathDirectory, "BotSettings");
        private static readonly string BotExitConditionSettingsFilePath = Path.Combine(SavePathDirectory, "BotExitConditionSettings");

        private static Dictionary<FileType, string> FilesPath = new()
        {
            { FileType.GeneralSettings, GeneralSettingsFilePath },
            { FileType.BotSettings, BotSettingsFilePath },
            { FileType.BotExitConditionSettings, BotExitConditionSettingsFilePath }
        };

        public static T LoadSettings<T>(FileType fileType)
        {
            CheckDirectoryExists();

            string filePath = FilesPath[fileType];

            if (!File.Exists(filePath)) return default;

            try
            {
                string xmlData = Encryption.Decrypt(File.ReadAllText(filePath));

                var serializer = new XmlSerializer(typeof(T));
                var rdr = new StringReader(xmlData);

                return (T)Convert.ChangeType(serializer.Deserialize(rdr), typeof(T), System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return default;
            }
        }
        public static void SaveSettings(dynamic settings, FileType fileType)
        {
            CheckDirectoryExists();

            string filePath = FilesPath[fileType];

            try
            {
                XmlSerializer xmlserializer = new(settings.GetType());
                StringWriter stringWriter = new();

                using var writer = XmlWriter.Create(stringWriter);

                xmlserializer.Serialize(writer, settings);

                string data = Encryption.Encrypt(stringWriter.ToString());

                File.WriteAllText(filePath, data);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private static void CheckDirectoryExists()
        {
            if (!Directory.Exists(SavePathDirectory)) Directory.CreateDirectory(SavePathDirectory);
        }
    }
}
