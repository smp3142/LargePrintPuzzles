using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GamesDataWordTests
    {
        [TestMethod]
        public void WordsAreSorted()
        {
            for (int i = 0; i < GamesData.WordData.AllWords.Count - 1; i++)
            {
                Assert.IsTrue(string.Compare(GamesData.WordData.AllWords[i], GamesData.WordData.AllWords[i + 1]) < 0);
            }
        }

        [TestMethod]
        public void WordsAreOnlyLetters()
        {
            for (int i = 0; i < GamesData.WordData.AllWords.Count; i++)
            {
                foreach (char letter in GamesData.WordData.AllWords[i])
                {
                    Assert.IsTrue(GamesData.WordData.Letters.Contains(letter.ToString()));
                }
            }
        }

        [TestMethod]
        public void ShortWordsAreSorted()
        {
            for (int i = 0; i < GamesData.WordData.ShortWords.Count - 1; i++)
            {
                Assert.IsTrue(string.Compare(GamesData.WordData.ShortWords[i], GamesData.WordData.ShortWords[i + 1]) < 0);
            }
        }

        [TestMethod]
        public void ShortWordsAreShort()
        {
            foreach (string word in GamesData.WordData.ShortWords)
            {
                Assert.IsTrue(word.Length >= 2 && word.Length <= 3);
            }
        }

        [TestMethod]
        public void ShortWordsAreOnlyLetters()
        {
            for (int i = 0; i < GamesData.WordData.ShortWords.Count; i++)
            {
                foreach (char letter in GamesData.WordData.ShortWords[i])
                {
                    Assert.IsTrue(GamesData.WordData.Letters.Contains(letter.ToString()));
                }
            }
        }
    }
}
