using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CluelessCrosswords.PDF
{
    public static class Produce
    {
        public static byte[] GamesPDF(Games games, byte[] fontData)
        {
            using System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            using PdfWriter pdfWriter = new PdfWriter(memoryStream);
            using PdfDocument pdfDocument = new PdfDocument(pdfWriter);
            using Document document = new Document(pdfDocument, PageSize.LETTER.Rotate());
            document.SetMargins(16.0f, 16.0f, 16.0f, 16.0f);

            PdfFont font = PdfFontFactory.CreateFont(fontData, true);
            int fontSize = 20;

            for (int i = 0; i < games.Puzzles.Length; i++)
            {
                Paragraph header = new Paragraph($"Puzzle {i + 1}")
                    .SetFont(font)
                    .SetFontSize(12);
                Table game = MakeGamePage(games.Puzzles[i])
                    .SetFont(font)
                    .SetFontSize(fontSize);
                document.Add(header);
                document.Add(game);
                document.Add(new AreaBreak());

                header = new Paragraph($"Solution {i + 1}")
                    .SetFont(font)
                    .SetFontSize(12);
                Table solution = MakeSolutionPage(games.Puzzles[i])
                    .SetFont(font)
                    .SetFontSize(fontSize);
                document.Add(header);
                document.Add(solution);
                if (i < games.Puzzles.Length - 1) { document.Add(new AreaBreak()); }
            }

            document.Close();
            return memoryStream.ToArray();
        }

        private static Table MakeGamePage(Puzzle puzzle)
        {
            Table table = new Table(new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            for (int i = 0; i < Games.ROWS; i++)
            {
                table.AddCell(AddHint(puzzle.Hints[i * 2]));
                table.AddCell(AddHint(puzzle.Hints[i * 2 + 1]));

                Cell cell = new Cell()
                    .Add(new Paragraph("\u00A0"))
                    .SetBorder(Border.NO_BORDER)
                    .SetBorderTop(new SolidBorder(ColorConstants.WHITE, 10.0f));
                table.AddCell(cell);

                for (int j = 0; j < Games.COLS; j++)
                {
                    table.AddCell(AddGameCell(puzzle.GameBoard[i, j]));
                }

                table.AddCell(cell.Clone(true));

                table.AddCell(AddLetter(puzzle, GamesData.WordData.Letters[i * 2]));
                table.AddCell(AddLetter(puzzle, GamesData.WordData.Letters[i * 2 + 1]));
            }

            SetTablePadding(table);

            return table;
        }

        private static Cell AddHint(string letter)
        {
            Paragraph p = new Paragraph();
            Cell cell = new Cell();
            if (Int32.TryParse(letter, out _))
            {
                p.Add("\u00A0" + letter);
                cell.SetFontSize(14.0f);
                cell.SetFontColor(new DeviceCmyk(40, 20, 0, 10));
            }
            else { p.Add(letter); }
            cell.Add(p);
            return cell;
        }

        private static Cell AddGameCell(string letter)
        {
            Paragraph p = new Paragraph();
            Cell cell = new Cell();
            if (letter == Games.EMPTYCHAR)
            {
                p.Add("\u00A0\u00A0");
                cell.SetBackgroundColor(new DeviceGray(0.6f));
            }
            else if (Int32.TryParse(letter, out _))
            {
                p.Add("\u00A0" + letter);
                cell.SetFontSize(14.0f);
                cell.SetFontColor(new DeviceCmyk(40, 20, 0, 10));
            }
            else { p.Add(letter); }

            cell.Add(p);
            return cell;
        }

        private static void SetTablePadding(Table table)
        {
            for (int i = 0; i < table.GetNumberOfRows(); i++)
            {
                for (int j = 0; j < table.GetNumberOfColumns(); j++)
                {
                    table.GetCell(i, j).SetPaddings(-6, 5, -6, 5);
                }
            }
        }

        private static Table MakeSolutionPage(Puzzle puzzle)
        {
            Table table = new Table(new float[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
            for (int i = 0; i < Games.ROWS; i++)
            {//TODO: Add Key and Solution
                table.AddCell(AddKey(i * 2, puzzle.Key[i * 2]));
                table.AddCell(AddKey(i * 2 + 1, puzzle.Key[i * 2 + 1]));

                Cell cell = new Cell()
                    .Add(new Paragraph("\u00A0"))
                    .SetBorder(Border.NO_BORDER)
                    .SetBorderTop(new SolidBorder(ColorConstants.WHITE, 10.0f));
                table.AddCell(cell);

                for (int j = 0; j < Games.COLS; j++)
                {
                    table.AddCell(AddGameCell(puzzle.Solution[i, j]));
                }
            }

            SetTablePadding(table);
            return table;
        }

        private static Cell AddKey(int index, string letter)
        {
            Cell cell = new Cell();
            cell.Add(new Paragraph($"{index + 1:d02}:{letter}"));
            return cell;
        }

        private static Paragraph AddLetter(Puzzle puzzle, string letter)
        {
            Paragraph p = new Paragraph($"\u00A0{letter}\u00A0");
            if (puzzle.Hints.Contains(" " + letter))
            {
                p.SetFontColor(ColorConstants.LIGHT_GRAY);
                p.SetLineThrough();
            }
            return p;
        }

        public static byte[] ShortWords(byte[] fontData)
        {
            using System.IO.MemoryStream memoryStream = new System.IO.MemoryStream();
            using PdfWriter pdfWriter = new PdfWriter(memoryStream);
            using PdfDocument pdfDocument = new PdfDocument(pdfWriter);
            using Document document = new Document(pdfDocument, PageSize.LETTER);
            document.SetMargins(16.0f, 16.0f, 16.0f, 16.0f);

            PdfFont font = PdfFontFactory.CreateFont(fontData, true);
            int fontSize = 16;

            StringBuilder twoLetter = new StringBuilder();
            List<StringBuilder> threeLetter = new List<StringBuilder>();
            string previousWord = " ";
            foreach (string word in GamesData.WordData.ShortWords)
            {
                if (word.Length == 2)
                {
                    twoLetter.Append(word + " ");
                }
                if (word.Length == 3)
                {
                    if (word.StartsWith(previousWord[0])) { threeLetter[^1].Append(word + " "); }
                    else { threeLetter.Add(new StringBuilder(word + " ")); }
                    previousWord = word;
                }
            }

            Paragraph twoLetterParagraph = new Paragraph(twoLetter.ToString())
                .SetFontSize(fontSize)
                .SetFont(font)
                .SetMultipliedLeading(0.8f);

            Paragraph threeLetterParagraph = new Paragraph()
                .SetFontSize(fontSize)
                .SetFont(font)
                .SetMultipliedLeading(0.8f);

            foreach (StringBuilder line in threeLetter)
            {
                string temp = line.ToString();
                Text text = new Text($"\u00A0{temp[0]}\u00A0\u00A0").SetFontColor(ColorConstants.RED);
                threeLetterParagraph.Add(text);
                threeLetterParagraph.Add(temp);
            }

            document.Add(twoLetterParagraph);
            document.Add(threeLetterParagraph);
            document.Close();
            return memoryStream.ToArray();
        }
    }
}
