using System;
using System.Diagnostics;
using System.IO;

namespace AutoUpdaterServerStarter
{
    class Program
    {
        private static readonly string TempPathDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SolidProgramming", "ValCavalluBot", "Temp");
        private static readonly string newAssemblyPath = Path.Combine(TempPathDirectory, "update");

        static void Main(string[] args)
        {
#if DEBUG
            Console.WriteLine("Taste drücken um zu starten");
            Console.ReadKey();
#endif

            MoveFiles();            
        }

        private static void MoveFiles()
        {
            string currentDirectory = Directory.GetCurrentDirectory();

            DirectoryInfo di = new(currentDirectory);

            foreach (FileInfo file in new DirectoryInfo(newAssemblyPath).GetFiles())
            {
                file.MoveTo(Path.Combine(di.FullName, file.Name), true);
            }

            foreach (DirectoryInfo directory in new DirectoryInfo(newAssemblyPath).GetDirectories())
            {
                directory.MoveTo(di.FullName);
            }

            Process.Start("ValCavalluBot.exe");
        }
    }
}
