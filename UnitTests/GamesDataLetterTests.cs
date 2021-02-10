using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class GamesDataLetterTests
    {
        [TestMethod]
        public void LettersAreInOrder()
        {
            for (int i = 0; i < GamesData.WordData.Letters.Count - 1; i++)
            {
                Assert.IsTrue(string.Compare(GamesData.WordData.Letters[i], GamesData.WordData.Letters[i + 1]) < 0);
            }
        }

        [TestMethod]
        public void LettersLength()
        {
            Assert.AreEqual(GamesData.WordData.Letters.Count, 26);
        }

        [TestMethod]
        public void LettersAreUppercase()
        {
            foreach (string letter in GamesData.WordData.Letters)
            {
                Assert.IsTrue(char.IsUpper(letter[0]));
            }
        }
    }
}
