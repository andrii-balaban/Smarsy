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
        public void Login_ShouldReturnExcpectedLogin()
        {
            // Arrange
            Smarsy smarsy = CreateOperational();
            smarsy.Login("1");

            // Act
            string studentLogin = smarsy.Student.Credentials.GetNetworkCredentials().UserName;

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

        private static Smarsy CreateOperational()
        {
            return new Smarsy(new DataBaseStub(), new SmarsyBrowserStub(), new DateTimeProvider());
        }
    }
}
