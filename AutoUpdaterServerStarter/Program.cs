using System;
using System.IO;

namespace AutoUpdaterServerStarter
{
    class Program
    {
        private static readonly string TempPathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "ValCavalluBot", "Temp");
        private static readonly string newAssemblyPath = Path.Combine(TempPathDirectory, "update");

        static void Main(string[] args)
        {
            MoveFiles();
            Console.ReadKey();
        }

        private static void MoveFiles()
        {
            string test = Directory.GetCurrentDirectory();

            DirectoryInfo di = new(test);

            foreach (FileInfo file in new DirectoryInfo(newAssemblyPath).GetFiles())
            {
                file.MoveTo(di.FullName);
            }


        }
    }
}
