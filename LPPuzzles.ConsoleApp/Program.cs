using CluelessCrosswords;

using System;
using System.Diagnostics;
using System.IO;

namespace LPPuzzles.ConsoleApp
{
    internal class Program
    {
        private static void Main()
        {
            Games clueless1 = new Games(1, Difficulty.Normal);
            foreach (var item in clueless1)
            {
                Console.WriteLine(item);
            }

            //Games clueless2 = new Games(20, Difficulty.Normal);
            //foreach (var item in clueless2)
            //{
            //    Console.WriteLine(item);
            //}

            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var cluelessPDF = CluelessCrosswords.PDF.Produce.GamesPDF(clueless1, Properties.Resources.notomono);
            File.WriteAllBytes(Path.Combine(homeDir, "./LPP-CluelessCrosswords.pdf"), cluelessPDF);

            var shortPDF = CluelessCrosswords.PDF.Produce.ShortWords(Properties.Resources.notomono);
            File.WriteAllBytes(Path.Combine(homeDir, "./LPP-ShortWords.pdf"), shortPDF);

            Process process = new Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = Path.Combine(homeDir, "./LPP-ShortWords.pdf");
            process.Start();

            process.StartInfo.FileName = Path.Combine(homeDir, "./LPP-CluelessCrosswords.pdf");
            process.Start();
        }
    }
}
