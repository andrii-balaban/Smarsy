using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using SmarsyEntities;

namespace Smarsy.Test
{
    using System;

    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class SmarsyTest
    {
        [Test]
        public void GetTextBetweenSubstrings_WhenSingleWordSeparatorsAreUsed_ShouldReturnExpected()
        {
            // Arrange
            TextProcessor textProcessor = TextProcessor.Processor;

            string text = "Some text to be parsed";
            string tagFrom = "Some";
            string tagTo = "to";

            // Act
            string result = textProcessor.GetTextBetweenSubstrings(text, tagFrom, tagTo);

            // Assert
            result.Should().Be(" text ");
        }

        [Test]
        public void GetLessonNameFromLessonWithTeacher_WhenLessonContainsTeacherName_ShouldReturnLessonName()
        {
            // Arrange
            SmarsyBrowser smarsyBrowser = new SmarsyBrowser();

            string expected = "Lesson name";

            // Act
            string result = smarsyBrowser.GetLessonNameFromLessonWithTeacher("Lesson name (Teacher name)");

            // Assert
            result.Should().Be(expected);
        }

        [Test]
        public void GetLessonNameFromLessonWithTeacher_WhenLessonDoesNotContaineteacherName_ShouldReturnLessonName()
        {
            // Arrange
            SmarsyBrowser smarsyBrowser = new SmarsyBrowser();

            // Act
            string result = smarsyBrowser.GetLessonNameFromLessonWithTeacher("Lesson name");

            // Assert
            result.Should().Be("Lesson name");
         }

        [Test]
        public void GetTeacherNameFromLessonWithTeacher_WhenLessonNameContainesTescherName_ShouldReturnTeacherName()
        {
            // Arrange
            SmarsyBrowser smarsyBrowser = new SmarsyBrowser();

            // Act
            string result = smarsyBrowser.GetTeacherNameFromLessonWithTeacher("Lesson name (Teacher name)", "Lesson name");

            // Assert
            result.Should().Be("Teacher name");
        }

        [Test]
        public void Login_ShouldReturnExcpectedLogin()
        {
            // Arrange
            SmarsyOperations smarsyOperations = CreateOperational();
            smarsyOperations.LoginToSmarsy("1");

            // Act
            string studentLogin = smarsyOperations.Student.Login;

            // Assert
            studentLogin.Should().Be("1");
        }

        [Test]
        public void GetDateFromText_WhenTextContainesDate_ShouldReturnExpectedDate()
        {
            // Arrange
            DateTime expected = new DateTime(2006, 1, 17);
            TextProcessor textProcessor = TextProcessor.Processor;

            // Act
            DateTime result = textProcessor.GetDateFromText("17 января", 11);
            
            // Assert
            result.Should().Be(expected);
        }

        [Test]
        public void GetDateFromText_WhenTextContainesSingleDigitNumberDate_ShouldReturnExpectedDate()
        {
            // Arrange
            DateTime expected = new DateTime(2007, 5, 2);
            TextProcessor textProcessor = TextProcessor.Processor;

            // Act
            DateTime result = textProcessor.GetDateFromText("2 мая", 10);
            
            // Assert
            result.Should().Be(expected);
        }

        [Test]
        public void GetDateFromText_WhenBirthdayNotYet_ShouldReturnSubstractedOneYear()
        {
            // Arrange
            DateTime expected = new DateTime(2005, 12, 17);
            TextProcessor textProcessor = TextProcessor.Processor;

            // Act
            DateTime result = textProcessor.GetDateFromText("17 декабря", 11);
            
            // Assert
            result.Should().Be(expected);
        }

        private static SmarsyOperations CreateOperational()
        {
            return new SmarsyOperations(new DataBaseStub(), new SmarsyBrowserStub());
        }
    }
}
