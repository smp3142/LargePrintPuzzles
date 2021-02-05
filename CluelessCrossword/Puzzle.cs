using System.Text;

namespace CluelessCrossword
{
    public struct Puzzle
    {
        public string[] Key;
        public string[] Hints;
        public string[,] Solution;
        public string[,] GameBoard;

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("--- Key ---");
            for (int i = 0; i < Key.Length; i++)
            {
                stringBuilder.Append($"{i + 1:D2}: {Key[i]} ");
                if ((i + 1) % 13 == 0) { stringBuilder.Append('\n'); }
            }

            stringBuilder.AppendLine("\n--- Hints ---");
            for (int i = 0; i < Hints.Length; i++)
            {
                stringBuilder.Append($"{i + 1:D2}:{Hints[i]} ");
                if ((i + 1) % 13 == 0) { stringBuilder.Append('\n'); }
            }

            stringBuilder.AppendLine("\n--- Solution ---");
            for (int row = 0; row < Solution.GetLength(0); row++)
            {
                for (int col = 0; col < Solution.GetLength(1); col++)
                {
                    stringBuilder.Append($"{Solution[row, col]}");
                }
                stringBuilder.AppendLine();
            }

            stringBuilder.AppendLine("\n--- GameBoard ---");
            for (int row = 0; row < GameBoard.GetLength(0); row++)
            {
                for (int col = 0; col < GameBoard.GetLength(1); col++)
                {
                    stringBuilder.AppendFormat("{0,3} ", GameBoard[row, col]);
                }
                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public Puzzle(Puzzle puzzle)
        {
            if (!(puzzle.Key == null))
            {
                Key = new string[puzzle.Key.Length];
                for (int i = 0; i < puzzle.Key.Length; i++)
                {
                    Key[i] = puzzle.Key[i];
                }
            }
            else { Key = null; }

            if (!(puzzle.Hints == null))
            {
                Hints = new string[puzzle.Hints.Length];
                for (int i = 0; i < puzzle.Hints.Length; i++)
                {
                    Hints[i] = puzzle.Hints[i];
                }
            }
            else { Hints = null; }

            if (!(puzzle.Solution == null))
            {
                Solution = new string[puzzle.Solution.GetLength(0), puzzle.Solution.GetLength(1)];
                for (int row = 0; row < puzzle.Solution.GetLength(0); row++)
                {
                    for (int col = 0; col < puzzle.Solution.GetLength(1); col++)
                    {
                        Solution[row, col] = puzzle.Solution[row, col];
                    }
                }
            }
            else { Solution = null; }

            if (!(puzzle.GameBoard == null))
            {
                GameBoard = new string[puzzle.GameBoard.GetLength(0), puzzle.GameBoard.GetLength(1)];
                for (int row = 0; row < puzzle.GameBoard.GetLength(0); row++)
                {
                    for (int col = 0; col < puzzle.GameBoard.GetLength(1); col++)
                    {
                        GameBoard[row, col] = puzzle.GameBoard[row, col];
                    }
                }
            }
            else { GameBoard = null; }
        }

        public static Puzzle GetInstance(Puzzle puzzle)
        {
            return new Puzzle(puzzle);
        }
    }
}
