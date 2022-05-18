using Company.Logger.Model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Company.UnitTests
{
    [TestClass]
    public class LoggerLineTests
    {
        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_IfTextParameterIsNull()
        {
            // Arrange
            DateTime dateTime = new DateTime(2022, 5, 17, 17, 20, 20);

            // Act
            Action createInstance = () => new LoggerLine(null, dateTime);

            // Assert
            createInstance.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "text");
        }

        [TestMethod]
        public void ToString_ShouldThrowArgumentNullException_IfTextParameterIsNull()
        {
            // Arrange
            string text = "Test data";
            string expectedResult = "2022-05-17 17:20:20:000	Test data. 	";
            DateTime dateTime = new DateTime(2022, 5, 17, 17, 20, 20);
            LoggerLine loggerLine = new LoggerLine(text, dateTime);

            // Act
            string result = loggerLine.ToString();

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}