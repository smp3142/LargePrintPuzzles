using iText.Html2pdf;
using iText.Layout.Font;

using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CluelessCrossword.PDF
{
    public static class Produce
    {
        private const int baseFontSize = 32;

        public static byte[] PDF(CluelessCrosswords cluelessCrosswords)
        {
            using MemoryStream memoryStream = new MemoryStream();

            FontProvider fontProvider = new FontProvider();
            fontProvider.AddFont(Properties.Resources.notomono);
            ConverterProperties properties = new ConverterProperties();
            properties.SetFontProvider(fontProvider);

            int counter = 0;
            StringBuilder html = new StringBuilder();

            html.Append("<!DOCTYPE html>");
            html.Append("<html>");
            html.Append("<head>");
            html.Append(@"<meta name=""description"" content = ""Large Print Clueless Crosswords."" />");
            html.Append("<style type=\"text / css\"> ");
            html.Append("@page { size: letter landscape; margin: 0.6cm 2cm 1cm 2cm; }");
            html.AppendLine(@$"table {{width: 100 %; font-size:{baseFontSize}px; border-collapse: collapse;}}");
            html.AppendLine(@"td {border: 1px solid black; border-collapse: collapse; text-align: center}");
            html.AppendLine(@".spacer {border: }");
            html.AppendLine(@".solidGray {background:#9C9C9C; }");
            html.AppendLine(@"p {font-size:16px}");
            html.AppendLine(@".number {color: #575757; position: relative; top:-0.5em; left:0.5em; font-size:60%; }");
            html.AppendLine(@".usedLetter {color: #707070; text-decoration: line-through}");
            html.Append("</style>");
            html.Append("</head>");
            html.Append("<body>");

            foreach (Puzzle puzzle in cluelessCrosswords)
            {
                counter++;
                AddGameBoard(puzzle, html, counter);
                AddSolution(puzzle, html, counter);
            }
            html.Append("</body>");
            html.Append("</html>");

            HtmlConverter.ConvertToPdf(html.ToString(), memoryStream, properties);
            return memoryStream.ToArray();
        }

        public static byte[] ShortWords()
        {
            using MemoryStream memoryStream = new MemoryStream();
            FontProvider fontProvider = new FontProvider();
            fontProvider.AddFont(Properties.Resources.notomono);
            ConverterProperties properties = new ConverterProperties();
            properties.SetFontProvider(fontProvider);

            StringBuilder html = new StringBuilder();
            html.Append("<!DOCTYPE html>");
            html.Append("<html>");
            html.Append("<head>");
            html.Append(@"<meta name=""description"" content = ""Large Print Clueless Crosswords."" />");
            html.Append("<style type=\"text / css\"> ");
            html.Append("@page { size: letter; margin: 0.8cm; }");
            html.Append(@"body {font-size: 21px; }");
            html.Append("</style>");
            html.Append("</head>");
            html.Append("<body>");

            string previousWord = " ";
            foreach (string word in GamesData.WordData.ShortWords)
            {
                if (word.Length == 2)
                {
                    html.Append(word + " ");
                }
            }
            html.Append("</br></br>");
            foreach (string word in GamesData.WordData.ShortWords)
            {
                if (word.Length == 3)
                {
                    if (word[0] != previousWord[0])
                    {
                        previousWord = word;
                        html.Append(@$"<span style=""color: red"">&nbsp;{word[0]}&nbsp;&nbsp;</span>");
                    }
                    html.Append(word + " ");
                }
            }

            html.Append("</body>");
            html.Append("</html>");
            HtmlConverter.ConvertToPdf(html.ToString(), memoryStream, properties);
            return memoryStream.ToArray();
        }

        private static void AddSolution(Puzzle puzzle, StringBuilder html, int counter)
        {
            html.AppendLine(@"<div style=""page-break-after: always;"" >");
            html.Append(@$"<p>Solution {counter}</p>");
            html.AppendLine(@"<table>");
            for (int row = 0; row < CluelessCrosswords.ROWS; row++)
            {
                html.Append("<tr>");
                html.Append(TableAddKey(row * 2, puzzle));
                html.Append(TableAddKey(row * 2 + 1, puzzle));
                html.Append(@"<td class=""spacer"">&nbsp;</td>");
                for (int col = 0; col < CluelessCrosswords.COLS; col++)
                {
                    html.Append(TableAddSolution(row, col, puzzle));
                }
                html.Append(@"<td class=""spacer"">&nbsp;&nbsp;&nbsp;&nbsp;</td>");
                html.Append("</tr>");
            }
            html.AppendLine("</table>");
            html.AppendLine(@"</div>");
        }

        private static string TableAddKey(int index, Puzzle puzzle)
        {
            return $@"<td style=""font-size:{baseFontSize / 2 + 5}px;"">{index + 1:D2}: {puzzle.Key[index]}</td>";
        }

        private static string TableAddSolution(int row, int col, Puzzle puzzle)
        {
            if (puzzle.Solution[row, col] == CluelessCrosswords.EMPTYCHAR)
            {
                return @"<td class=""solidGray"">&nbsp;</td>";
            }
            else { return $"<td>{puzzle.Solution[row, col]}</td>"; }
        }

        private static void AddGameBoard(Puzzle puzzle, StringBuilder html, int counter)
        {
            html.AppendLine(@"<div style=""page-break-after: always;"" >");
            html.Append(@$"<div class=""puzzleHead""><p>Puzzle {counter}</p></div>");
            html.AppendLine(@"<table>");
            for (int row = 0; row < CluelessCrosswords.ROWS; row++)
            {
                html.Append("<tr>");
                html.Append(TableAddHint(row * 2, puzzle));
                html.Append(TableAddHint(row * 2 + 1, puzzle));
                html.Append(@"<td class=""spacer"">&nbsp;</td>");
                for (int col = 0; col < CluelessCrosswords.COLS; col++)
                {
                    html.Append(TableAddGameBoard(row, col, puzzle));
                }
                html.Append(@"<td class=""spacer"">&nbsp;</td>");
                html.Append(TableAddLetter(row * 2, puzzle));
                html.Append(TableAddLetter(row * 2 + 1, puzzle));
                html.Append("</tr>");
            }
            html.AppendLine("</table>");
            html.AppendLine("<p>Notes</p>");
            html.AppendLine(@"</div>");
        }

        private static string TableAddLetter(int index, Puzzle puzzle)
        {
            if (puzzle.Hints.Contains(" " + GamesData.WordData.Letters[index]))
            {
                return @$"<td class=""usedLetter"">&nbsp;{GamesData.WordData.Letters[index]}&nbsp;</td>";
            }
            else { return $"<td>&nbsp;{GamesData.WordData.Letters[index]}&nbsp;</td>"; }
        }

        private static string TableAddGameBoard(int row, int col, Puzzle puzzle)
        {
            if (Int32.TryParse(puzzle.GameBoard[row, col], out _))
            {
                return @$"<td class=""number"">{puzzle.GameBoard[row, col]}</td>";
            }
            else if (puzzle.GameBoard[row, col] == CluelessCrosswords.EMPTYCHAR)
            {
                return @"<td class=""solidGray"">&nbsp;&nbsp;</td>";
            }
            else
            {
                return $"<td>{puzzle.GameBoard[row, col]}&nbsp;</td>";
            }
        }

        private static string TableAddHint(int index, Puzzle puzzle)
        {
            if (Int32.TryParse(puzzle.Hints[index], out _))
            {
                return @$"<td class=""number"">{puzzle.Hints[index]}&nbsp;</td>";
            }
            else { return $"<td>{puzzle.Hints[index]}&nbsp;</td>"; }
        }
    }
}
