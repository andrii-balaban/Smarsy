namespace Smarsy.Test
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class SmarsyTest
    {
        [TestMethod]
        public void TestWhenSingleWordSeparatorsAreUsedThenOk()
        {
            var op = new Operational("1");

            var text = "Some text to be parsed";
            var tagFrom = "Some";
            var tagTo = "to";

            var result = op.GetTextBetweenSubstrings(text, tagFrom, tagTo);

            Assert.AreEqual(result, " text ");
        }

        [TestMethod]
        public void TestGetLessonNameFromLessonWithTeacher()
        {
            var op = new Operational("1");

            var result = op.GetLessonNameFromLessonWithTeacher("Название урока (Имя учителя)");

            Assert.AreEqual(result, "Название урока");
        }

        [TestMethod]
        public void TestGetLessonNameFromLessonWithTeacherWithoutTeacher()
        {
            var op = new Operational("1");

            var result = op.GetLessonNameFromLessonWithTeacher("Название урока");

            Assert.AreEqual(result, "Название урока");
        }

        [TestMethod]
        public void TestGetTeacherNameFromLessonWithTeacher()
        {
            var op = new Operational("1");

            var result = op.GetTeacherNameFromLessonWithTeacher("Название урока (Имя учителя)", "Название урока");

            Assert.AreEqual(result, "Имя учителя");
        }

        [TestMethod]
        public void TestWhenObjectIsInitializedThenStudentLoginIsSet()
        {
            var op = new Operational("1");
            Assert.AreEqual(op.Student.Login, "1");
        }

        [TestMethod]
        public void TestWhenObjectIsInitializedThenWebBrowserIsInitialized()
        {
            var op = new Operational("1");
            Assert.IsNotNull(op.SmarsyBrowser);
        }

        [TestMethod]
        public void TestGetDateFromText()
        {
            var result = Operational.GetDateFromText("17 января", 11);
            var expected = new DateTime(2006, 1, 17);

            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void TestGetDateFromTextWithSingleDigitNumber()
        {
            var result = Operational.GetDateFromText("2 мая", 10);
            var expected = new DateTime(2006, 5, 2);

            Assert.AreEqual(result, expected);
        }

        [TestMethod]
        public void TestWhenBirthdayNotYetThenSubstract1Year()
        {
            var result = Operational.GetDateFromText("17 декабря", 11);
            var expected = new DateTime(2005, 12, 17);

            Assert.AreEqual(result, expected);
        }
    }
}
