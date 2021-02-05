using GamesData;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CluelessCrossword
{
    public enum Difficulty { Easy = 8, Normal = 4, Hard = 2 };

    public class CluelessCrosswords : IEnumerable
    {
        public Puzzle[] Puzzles { get; private set; }
        public List<string> usedWords;
        public readonly List<string> wordSource;
        public const string EMPTYCHAR = ".";
        public const int ROWS = 13;
        public const int COLS = ROWS;
        private const int TriesToAddWord = 96;
        private readonly Random random;

        // TODO: Verify that only 1 solution exists?

        public CluelessCrosswords(int numberOfPuzzles, Difficulty difficulty)
        {
            Puzzles = new Puzzle[numberOfPuzzles];
            wordSource = GamesData.WordData.AllWords;
            usedWords = new List<string>();
            random = new Random();

            for (int i = 0; i < numberOfPuzzles; i++)
            {
                Puzzles[i].Key = new string[WordData.Letters.Count];
                Puzzles[i].Solution = new string[ROWS, COLS];
                Puzzles[i].Hints = new string[WordData.Letters.Count];
                Puzzles[i].GameBoard = new string[ROWS, COLS];

                MakeKey(Puzzles[i].Key);
                MakeSolution(Puzzles[i], false);
                MakeHints(Puzzles[i], difficulty);
                MakeGameBoard(Puzzles[i]);
            }
        }

        private static void MakeGameBoard(Puzzle puzzle)
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    string letter = puzzle.Solution[row, col];
                    if (letter == EMPTYCHAR) { puzzle.GameBoard[row, col] = EMPTYCHAR; }
                    else
                    {
                        puzzle.GameBoard[row, col] = puzzle.Hints[Array.IndexOf(puzzle.Key, letter)];
                    }
                }
            }
        }

        private void MakeHints(Puzzle puzzle, Difficulty difficulty)
        {
            for (int i = 0; i < puzzle.Hints.Length; i++)
            {
                puzzle.Hints[i] = $"{i + 1:D2}";
            }
            List<string> unUsedLetters = new List<string>(WordData.Letters);
            List<string> usedLetters = new List<string> { Capacity = unUsedLetters.Count };
            foreach (string letter in puzzle.Solution)
            {
                if (letter == EMPTYCHAR) { continue; }
                if (unUsedLetters.Contains(letter))
                {
                    unUsedLetters.Remove(letter);
                    usedLetters.Add(letter);
                }
            }
            foreach (string letter in unUsedLetters)
            {
                puzzle.Hints[Array.IndexOf(puzzle.Key, letter)] = " " + letter;
            }
            string[] vowels = { "A", "E", "I", "O", "U" };
            string vowel = vowels[random.Next(vowels.Length)];
            puzzle.Hints[Array.IndexOf(puzzle.Key, vowel)] = " " + vowel;

            foreach (string v in vowels)
            {
                usedLetters.Remove(v);
            }

            for (int i = 0; i < (int)difficulty; i++)
            {
                string letter = usedLetters[random.Next(usedLetters.Count)];
                usedLetters.Remove(letter);
                puzzle.Hints[Array.IndexOf(puzzle.Key, letter)] = " " + letter;
            }
        }

        private void MakeSolution(Puzzle puzzle, bool customWords)
        {
            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    puzzle.Solution[row, col] = EMPTYCHAR;
                }
            }

            if (customWords)
            {
                AddRandomWords(puzzle, wordSource, wordSource.Count);
            }
            else
            {
                AddRandomWords(puzzle, WordData.InterestingWords, 6);
                AddRandomWords(puzzle, wordSource, TriesToAddWord);
                AddRandomWords(puzzle, WordData.ShortWords, TriesToAddWord * 2);
            }
        }

        private void AddRandomWords(Puzzle puzzle, List<string> wordsToAdd, int tries)
        {
            List<(int rowStart, int colStart, bool addToRow)> locations = new List<(int rowStart, int colStart, bool addToRow)>();
            for (int i = 0; i < tries; i++)
            {
                for (int row = 0; row < ROWS; row++)
                {
                    for (int col = 0; col < COLS; col++)
                    {
                        locations.Add((row, col, true));
                        locations.Add((row, col, false));
                    }
                }
                string wordToAdd = GetWord(wordsToAdd);
                while (locations.Count > 0)
                {
                    int randomLocation = random.Next(locations.Count);
                    if (IsValidLocation(locations[randomLocation].rowStart, locations[randomLocation].colStart, locations[randomLocation].addToRow, wordToAdd, puzzle))
                    {
                        AddWordToPuzzle(locations[randomLocation].rowStart, locations[randomLocation].colStart, locations[randomLocation].addToRow, wordToAdd, puzzle);
                        locations.Clear();
                        break;
                    }

                    locations.RemoveAt(randomLocation);
                }
            }
        }

        private void AddWordToPuzzle(int rowStart, int colStart, bool addToRow, string wordToAdd, Puzzle puzzle)
        {
            if (addToRow)
            {
                for (int i = 0; i < wordToAdd.Length; i++)
                {
                    puzzle.Solution[rowStart, colStart + i] = wordToAdd[i].ToString();
                }
            }
            else
            {
                for (int i = 0; i < wordToAdd.Length; i++)
                {
                    puzzle.Solution[rowStart + i, colStart] = wordToAdd[i].ToString();
                }
            }
            if (wordToAdd.Length > 3) { usedWords.Add(wordToAdd); }
        }

        private bool IsValidLocation(int rowStart, int colStart, bool addToRow, string wordToAdd, Puzzle puzzle)
        {
            if (addToRow) { if (colStart + wordToAdd.Length > COLS) { return false; } }
            else { if (rowStart + wordToAdd.Length > ROWS) { return false; } }

            Puzzle testPuzzle = new Puzzle(puzzle);
            bool usedEmptyChar = false;
            if (addToRow)
            {
                for (int i = 0; i < wordToAdd.Length; i++)
                {
                    string letter = puzzle.Solution[rowStart, colStart + i];
                    if (letter == EMPTYCHAR)
                    {
                        testPuzzle.Solution[rowStart, colStart + i] = wordToAdd[i].ToString();
                        usedEmptyChar = true;
                    }
                    else if (letter == wordToAdd[i].ToString()) { continue; }
                    else { return false; }
                }
            }
            else
            {
                for (int i = 0; i < wordToAdd.Length; i++)
                {
                    string letter = puzzle.Solution[rowStart + i, colStart];
                    if (letter == EMPTYCHAR)
                    {
                        testPuzzle.Solution[rowStart + i, colStart] = wordToAdd[i].ToString();
                        usedEmptyChar = true;
                    }
                    else if (letter == wordToAdd[i].ToString()) { continue; }
                    else { return false; }
                }
            }

            return usedEmptyChar && IsAllWordsValid(testPuzzle);
        }

        private bool IsAllWordsValid(Puzzle testPuzzle)
        {
            // Get all rows
            for (int row = 0; row < ROWS; row++)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int col = 0; col < COLS; col++)
                {
                    stringBuilder.Append(testPuzzle.Solution[row, col]);
                }
                if (!AreAllWordsInLineValid(stringBuilder.ToString().Split(EMPTYCHAR))) { return false; }
            }
            // Get all cols
            for (int col = 0; col < COLS; col++)
            {
                StringBuilder stringBuilder = new StringBuilder();
                for (int row = 0; row < ROWS; row++)
                {
                    stringBuilder.Append(testPuzzle.Solution[row, col]);
                }
                if (!AreAllWordsInLineValid(stringBuilder.ToString().Split(EMPTYCHAR))) { return false; }
            }
            return true;
        }

        private bool AreAllWordsInLineValid(string[] testWords)
        {
            foreach (string word in testWords)
            {
                if (word.Length <= 1) { continue; }
                if (wordSource.BinarySearch(word) < 0) { return false; }
            }
            return true;
        }

        private string GetWord(List<string> wordsList)
        {
            int index;
            while (true)
            {
                index = random.Next(0, wordsList.Count);
                if (usedWords.Count < (wordsList.Count / 2) || !usedWords.Contains(wordsList[index])) { return wordsList[index]; }
            }
        }

        private void MakeKey(string[] key)
        {
            for (int i = 0; i < key.Length; i++)
            {
                key[i] = WordData.Letters[i];
            }
            for (int i = 0; i < key.Length; i++)
            {
                int tempIndex = random.Next(0, key.Length);
                string temp = key[tempIndex];
                key[tempIndex] = key[i];
                key[i] = temp;
            }
        }

        public IEnumerator GetEnumerator()
        {
            return Puzzles.GetEnumerator();
        }
    }
}
