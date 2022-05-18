using Company.Logger.Model;
using Company.Logger.Options;
using Company.Logger.Provider;
using Company.Logger.Service;
using Company.Logger.Stream;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;

namespace Company.UnitTests
{
    [TestClass]
    public class LoggerServiceTests
    {
        private const string Path = "c:\\temp";

        private readonly DateTime startDate = new DateTime(2022, 5, 16, 16, 25, 5);

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_IfOptionsParameterIsNull()
        {
            // Arrange
            Mock<ILogFile> logFile = new Mock<ILogFile>();
            Mock<IDateTimeProvider> dateTimeProvider = new();

            // Act
            Action createInstance = () => new LoggerService(null, logFile.Object, dateTimeProvider.Object, startDate);

            // Assert
            createInstance.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "options");
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_IfLogFileParameterIsNull()
        {
            // Arrange 
            Mock<IOptions<LoggerOptions>> options = new();
            Mock<IDateTimeProvider> dateTimeProvider = new();

            // Act
            Action createInstance = () => new LoggerService(options.Object, null, dateTimeProvider.Object, startDate);

            // Assert
            createInstance.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "logFile");
        }

        [TestMethod]
        public void Constructor_ShouldThrowArgumentNullException_IfDateTimeProviderParameterIsNull()
        {
            // Arrange
            Mock<IOptions<LoggerOptions>> options = new();
            Mock<ILogFile> logFile = new();

            // Act
            Action createInstance = () => new LoggerService(options.Object, logFile.Object, null, startDate);

            // Assert
            createInstance.Should().Throw<ArgumentNullException>().Where(e => e.ParamName == "dateTimeProvider");
        }

        [TestMethod]
        public void Run_ShouldWriteToLogFileAllLines_IfStopWithFlushIsSetToTrue()
        {
            // Arrange
            IOptions<LoggerOptions> options = Options.Create(new LoggerOptions() { FilePath = Path });
            Mock<ILogFile> logFile = new();
            Mock<IDateTimeProvider> dateTimeProvider = new();
            ILoggerService service = new LoggerService(options, logFile.Object, dateTimeProvider.Object, startDate);
            int times = 37;
            service.StopWithFlush();
            WriteToLogger(service, times);

            // Act
            service.Run();

            // Assert
            logFile.Verify(x => x.WriteLine(It.IsAny<LoggerLine>()), Times.Exactly(times));
        }

        [TestMethod]
        public void Run_ShouldWriteNothingToLogFile_IfStopWithoutFlushIsSetToTrue()
        {
            // Arrange
            IOptions<LoggerOptions> options = Options.Create(new LoggerOptions() { FilePath = Path });
            Mock<ILogFile> logFile = new();
            Mock<IDateTimeProvider> dateTimeProvider = new();
            ILoggerService service = new LoggerService(options, logFile.Object, dateTimeProvider.Object, startDate);
            int times = 100;
            service.StopWithoutFlush();
            WriteToLogger(service, times);

            // Act
            service.Run();

            // Assert
            logFile.Verify(x => x.WriteLine(It.IsAny<LoggerLine>()), Times.Never);
        }

        [TestMethod]
        public void Run_ShouldCreateNewFile_WhenMidnightIsCrossed()
        {
            // Arrange
            IOptions<LoggerOptions> options = Options.Create(new LoggerOptions() { FilePath = Path });
            Mock<ILogFile> logFile = new();
            Mock<IDateTimeProvider> dateTimeProvider = new();
            dateTimeProvider.Setup(m => m.GetNow()).Returns(startDate.AddDays(1));
            ILoggerService service = new LoggerService(options, logFile.Object, dateTimeProvider.Object, startDate);
            service.StopWithFlush();
            service.Write("Test Data");

            // Act
            service.Run();

            // Assert
            logFile.Verify(x => x.Initialise(It.IsAny<string>()), Times.Exactly(2));
        }

        [TestMethod]
        public void Run_ShouldNotCreateNewFile_WhenMidnightIsNotCrossed()
        {
            // Arrange
            IOptions<LoggerOptions> options = Options.Create(new LoggerOptions() { FilePath = Path });
            Mock <ILogFile> logFile = new();
            Mock<IDateTimeProvider> dateTimeProvider = new();
            dateTimeProvider.Setup(m => m.GetNow()).Returns(startDate);
            ILoggerService service = new LoggerService(options, logFile.Object, dateTimeProvider.Object, startDate);
            service.StopWithFlush();
            service.Write("Test Data");

            // Act
            service.Run();

            // Assert
            logFile.Verify(x => x.Initialise(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void Run_ShouldStopWritingToFile_WhenStopWithoutFlushIsSetToTrue()
        {
            // Arrange
            IOptions<LoggerOptions> options = Options.Create(new LoggerOptions() { FilePath = Path });
            Mock<ILogFile> logFile = new();
            Mock<IDateTimeProvider> dateTimeProvider = new();
            dateTimeProvider.Setup(m => m.GetNow()).Returns(startDate);
            ILoggerService service = new LoggerService(options, logFile.Object, dateTimeProvider.Object, startDate);
            int timesCalled = 0;
            void setStopWithoutFlush()
            {
                timesCalled++;
                if (timesCalled == 50)
                {
                    service.StopWithoutFlush();
                }
            }
            logFile.Setup(m => m.WriteLine(It.IsAny<LoggerLine>())).Callback(setStopWithoutFlush);
            int times = 100;
            int expectedTimesToWriteToFile = 50;
            WriteToLogger(service, times);

            // Act
            service.Run();

            // Assert
            logFile.Verify(x => x.WriteLine(It.IsAny<LoggerLine>()), Times.Exactly(expectedTimesToWriteToFile));
        }


        [TestMethod]
        public void Run_ShouldFinishWritingToFile_WhenStopWithFlushIsSetToTrue()
        {
            // Arrange
            IOptions<LoggerOptions> options = Options.Create(new LoggerOptions() { FilePath = Path });
            Mock<ILogFile> logFile = new();
            Mock<IDateTimeProvider> dateTimeProvider = new();
            dateTimeProvider.Setup(m => m.GetNow()).Returns(startDate);
            ILoggerService service = new LoggerService(options, logFile.Object, dateTimeProvider.Object, startDate);
            int timesCalled = 0;
            void setStopWithoutFlush()
            {
                timesCalled++;
                if (timesCalled == 50)
                {
                    service.StopWithFlush();
                }
            }
            logFile.Setup(m => m.WriteLine(It.IsAny<LoggerLine>())).Callback(setStopWithoutFlush);
            int times = 100;
            int expectedTimesToWriteToFile = 100;
            WriteToLogger(service, times);

            // Act
            service.Run();

            // Assert
            logFile.Verify(x => x.WriteLine(It.IsAny<LoggerLine>()), Times.Exactly(expectedTimesToWriteToFile));
        }

        [TestMethod]
        public void Run_ShouldFinishWritingToFile_WhenExceptionOccursAndStopWithFlushIsTrue()
        {
            // Arrange
            IOptions<LoggerOptions> options = Options.Create(new LoggerOptions() { FilePath = Path });
            Mock<ILogFile> logFile = new();
            Mock<IDateTimeProvider> dateTimeProvider = new();
            dateTimeProvider.Setup(m => m.GetNow()).Returns(startDate);
            ILoggerService service = new LoggerService(options, logFile.Object, dateTimeProvider.Object, startDate);
            int timesCalled = 0;
            service.StopWithFlush();
            void throwException()
            {
                timesCalled++;
                if (timesCalled == 50)
                {
                    throw new Exception("Something went wrong");
                }
            }
            logFile.Setup(m => m.WriteLine(It.IsAny<LoggerLine>())).Callback(throwException);
            int times = 100;
            int expectedTimesToWriteToFile = 100;
            WriteToLogger(service, times);

            // Act
            service.Run();

            // Assert
            logFile.Verify(x => x.WriteLine(It.IsAny<LoggerLine>()), Times.Exactly(expectedTimesToWriteToFile));
        }

        private void WriteToLogger(ILoggerService service, int times)
        {
            for (int i = 0; i < times; i++)
            {
                service.Write("Test Data");
            }
        }
    }
}
