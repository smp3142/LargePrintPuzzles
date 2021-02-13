using CluelessCrosswords;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass]
    public class CluelessCrosswordTests
    {
        private static Games SingleCluelessCrosswordsNormal;
        private static Games SingleCluelessCrosswordsEasy;
        private static Games SingleCluelessCrosswordsHard;
        private static Games cluelessCrosswordsNormal;
        private static Games cluelessCrosswordsEasy;
        private static Games cluelessCrosswordsHard;
        private static Games[] cluelessesGroup;

        [AssemblyInitialize]
#pragma warning disable IDE0060 // Remove unused parameter - Required signature.
        public static void Initialize(TestContext t)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            SingleCluelessCrosswordsEasy = new Games(1, Difficulty.Normal);
            SingleCluelessCrosswordsNormal = new Games(1, Difficulty.Easy);
            SingleCluelessCrosswordsHard = new Games(1, Difficulty.Hard);
            cluelessCrosswordsEasy = new Games(10, Difficulty.Normal);
            cluelessCrosswordsNormal = new Games(10, Difficulty.Easy);
            cluelessCrosswordsHard = new Games(10, Difficulty.Hard);
            cluelessesGroup = new Games[] { SingleCluelessCrosswordsEasy,
                SingleCluelessCrosswordsNormal,
                SingleCluelessCrosswordsHard,
                cluelessCrosswordsEasy,
                cluelessCrosswordsNormal,
                cluelessCrosswordsHard };
        }

        [TestMethod]
        public void KeysAreRightLength()
        {
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    Assert.IsTrue(puzzle.Key.Length == GamesData.WordData.Letters.Count);
                }
            }
        }

        [TestMethod]
        public void KeysHaveAllLetters()
        {
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    foreach (string letter in GamesData.WordData.Letters)
                    {
                        Assert.IsTrue(puzzle.Key.Contains(letter));
                    }
                }
            }
        }

        [TestMethod]
        public void KeysHaveNoRepeats()
        {
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    foreach (string letter in GamesData.WordData.Letters)
                    {
                        Assert.IsTrue(puzzle.Key.Count(p => p == letter) == 1);
                    }
                }
            }
        }

        [TestMethod]
        public void KeysAreShuffled()
        {
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    int totalShuffled = 0;
                    int minShuffled = puzzle.Key.Length * 2 / 3;
                    for (int i = 0; i < puzzle.Key.Length; i++)
                    {
                        if (puzzle.Key[i] != GamesData.WordData.Letters[i]) { totalShuffled++; }
                    }
                    Assert.IsTrue(totalShuffled >= minShuffled);
                }
            }
        }

        [TestMethod]
        public void ManyLettersAreUsed()
        {
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    List<string> uniqueLetters = new List<string>();
                    foreach (string letter in puzzle.Solution)
                    {
                        if (!uniqueLetters.Contains(letter)) { uniqueLetters.Add(letter); }
                    }
                    Assert.IsTrue(uniqueLetters.Count > 17, $"Only {uniqueLetters.Count} letters were used.");
                }
            }
        }

        [TestMethod]
        public void BoardMostlyFilled()
        {
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    int empty = 0;
                    foreach (string item in puzzle.Solution)
                    {
                        if (item == Games.EMPTYCHAR) { empty++; }
                    }
                    Assert.IsTrue(empty < (int)(Games.ROWS * Games.COLS / 2.0), $"There are {empty} cells empty.");
                }
            }
        }

        [TestMethod]
        public void LettersAreInKey()
        {
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    foreach (string letter in puzzle.Solution)
                    {
                        Assert.IsTrue(letter == Games.EMPTYCHAR || puzzle.Key.Contains(letter),
                            $"{letter} not in key nor is it {Games.EMPTYCHAR}.");
                    }
                }
            }
        }

        [TestMethod]
        public void HintsHaveVowel()
        {
            int total;
            List<string> vowels = new List<string>() { " A", " E", " I", " O", " U" };
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    total = 0;
                    foreach (string letter in puzzle.Hints)
                    {
                        if (vowels.Contains(letter)) { total++; }
                    }
                    Assert.IsTrue(total > 0);
                }
            }
        }

        [TestMethod]
        public void HintsAreUnique()
        {
            List<string> used = new List<string>();
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    used.Clear();
                    foreach (string item in puzzle.Hints)
                    {
                        Assert.IsFalse(used.Contains(item));
                        used.Add(item);
                    }
                }
            }
        }

        [TestMethod]
        public void OnlyHintsInGameBoard()
        {
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    List<string> used = new List<string>();
                    foreach (string item in puzzle.Hints)
                    {
                        if (char.IsLetter(item[1])) { used.Add(item); }
                    }
                    foreach (string item in puzzle.GameBoard)
                    {
                        if (!(item == Games.EMPTYCHAR) && char.IsLetter(item[1])) { Assert.IsTrue(used.Contains(item)); }
                    }
                }
            }
        }

        [TestMethod]
        public void GameBoardLettersMatchSolution()
        {
            foreach (Games clueless in cluelessesGroup)
            {
                foreach (Puzzle puzzle in clueless)
                {
                    for (int row = 0; row < Games.ROWS; row++)
                    {
                        for (int col = 0; col < Games.COLS; col++)
                        {
                            if (puzzle.GameBoard[row, col].Trim() == Games.EMPTYCHAR)
                            {
                                Assert.IsTrue(puzzle.Solution[row, col] == Games.EMPTYCHAR);
                            }
                            else if (char.IsLetter(puzzle.GameBoard[row, col][1]))
                            {
                                Assert.IsTrue(" " + puzzle.Solution[row, col] == puzzle.GameBoard[row, col]);
                            }
                        }
                    }
                }
            }
        }
    }
}
