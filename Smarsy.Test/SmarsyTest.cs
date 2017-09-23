using System.Threading;
using FluentAssertions;
using NUnit.Framework;

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
            Operational operational = CreateOperational();

            string text = "Some text to be parsed";
            string tagFrom = "Some";
            string tagTo = "to";

            // Act
            string result = operational.GetTextBetweenSubstrings(text, tagFrom, tagTo);

            // Assert
            result.Should().Be(" text ");
        }

        [Test]
        public void GetLessonNameFromLessonWithTeacher_WhenLessonContainsTeacherName_ShouldReturnLessonName()
        {
            // Arrange
            Operational operational = CreateOperational();

            string expected = "Lesson name";

            // Act
            string result = operational.GetLessonNameFromLessonWithTeacher("Lesson name (Teacher name)");

            // Assert
            result.Should().Be(expected);
        }

        [Test]
        public void GetLessonNameFromLessonWithTeacher_WhenLessonDoesNotContaineteacherName_ShouldReturnLessonName()
        {
            // Arrange
            Operational operational = CreateOperational();

            // Act
            string result = operational.GetLessonNameFromLessonWithTeacher("Lesson name");

            // Assert
            result.Should().Be("Lesson name");
         }

        [Test]
        public void GetTeacherNameFromLessonWithTeacher_WhenLessonNameContainesTescherName_ShouldReturnTeacherName()
        {
            // Arrange
            Operational operational = CreateOperational();

            // Act
            string result = operational.GetTeacherNameFromLessonWithTeacher("Lesson name (Teacher name)", "Lesson name");

            // Assert
            result.Should().Be("Teacher name");
        }

        [Test]
        public void Login_ShouldReturnExcpectedLogin()
        {
            // Arrange
            Operational operational = CreateOperational();

            // Act
            string studentLogin = operational.Student.Login;

            // Assert
            studentLogin.Should().Be("1");
        }

        [Test]
        public void GetDateFromText_WhenTextContainesDate_ShouldReturnExpectedDate()
        {
            // Arrange
            DateTime expected = new DateTime(2006, 1, 17);

            // Act
            DateTime result = Operational.GetDateFromText("17 января", 11);
            
            // Assert
            result.Should().Be(expected);
        }

        [Test]
        public void GetDateFromText_WhenTextContainesSingleDigitNumberDate_ShouldReturnExpectedDate()
        {
            // Arrange
            DateTime expected = new DateTime(2007, 5, 2);

            // Act
            DateTime result = Operational.GetDateFromText("2 мая", 10);
            
            // Assert
            result.Should().Be(expected);
        }

        [Test]
        public void GetDateFromText_WhenBirthdayNotYet_ShouldReturnSubstractedOneYear()
        {
            // Arrange
            DateTime expected = new DateTime(2005, 12, 17);

            // Act
            DateTime result = Operational.GetDateFromText("17 декабря", 11);
            
            // Assert
            result.Should().Be(expected);
        }

        private static Operational CreateOperational()
        {
            string login = "1";

            return new Operational(new DataBaseStub(), login);
        }
    }
}
