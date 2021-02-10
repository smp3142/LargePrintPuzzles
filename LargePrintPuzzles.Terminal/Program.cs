using CluelessCrossword;

using System;
using System.IO;

namespace LargePrintPuzzles.Terminal
{
    internal class Program
    {
        private static void Main()
        {
            CluelessCrosswords clueless1 = new CluelessCrosswords(1, Difficulty.Normal);
            foreach (var item in clueless1)
            {
                Console.WriteLine(item);
            }

            //CluelessCrosswords clueless2 = new CluelessCrosswords(20, Difficulty.Normal);
            //foreach (var item in clueless2)
            //{
            //    Console.WriteLine(item);
            //}

            var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var cluelessPDF = CluelessCrossword.PDF.Produce.PDF(clueless1);
            File.WriteAllBytes(Path.Combine(homeDir, "./clueless.pdf"), cluelessPDF);

            var shortPDF = CluelessCrossword.PDF.Produce.ShortWords();
            File.WriteAllBytes(Path.Combine(homeDir, "./short.pdf"), shortPDF);
        }
    }
}
