using System;
using System.IO;

namespace AutoUpdaterServerStarter
{
    class Program
    {
        private static string TempPathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "ValCavalluBot", "Temp");
        private static readonly string extractDest = Path.Combine(TempPathDirectory, "update");

        static void Main(string[] args)
        {
            MoveFiles();
            Console.ReadKey();
        }

        private static void MoveFiles()
        {
            var test = Directory.GetCurrentDirectory();
            Directory.Delete(Directory.GetCurrentDirectory());
            Directory.Move(extractDest, Directory.GetCurrentDirectory());
        }
    }
}
