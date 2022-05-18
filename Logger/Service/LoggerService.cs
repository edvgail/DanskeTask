using Company.Logger.Model;
using Company.Logger.Options;
using Company.Logger.Provider;
using Company.Logger.Stream;
using Microsoft.Extensions.Options;
using Validation;

namespace Company.Logger.Service
{
    public class LoggerService : ILoggerService
    {
        private const string DateTimeFormat = "yyyyMMdd_HHmmss_fff";
        private const string FileName = "Log";

        private readonly WrappedList<LoggerLine> loggerLines = new WrappedList<LoggerLine>();
        private readonly IOptions<LoggerOptions> options;
        private readonly ILogFile logFile;
        private readonly IDateTimeProvider dateTimeProvider;
        private bool quitWithFlush;
        private bool exit;
        private DateTime startDate;

        public LoggerService(IOptions<LoggerOptions> options, 
                             ILogFile logFile, 
                             IDateTimeProvider dateTimeProvider, 
                             DateTime? startDate = null)
        {
            this.options = Requires.NotNull(options, nameof(options));
            this.logFile = Requires.NotNull(logFile, nameof(logFile));
            this.dateTimeProvider = Requires.NotNull(dateTimeProvider, nameof(dateTimeProvider));
            this.startDate = startDate ?? DateTime.Now;
            this.logFile.Initialise(this.GetLogFileName());
        }

        public void StopWithoutFlush()
        {
            this.exit = true;
        }

        public void StopWithFlush()
        {
            this.quitWithFlush = true;
        }

        public void Write(string text)
        {
            this.loggerLines.Add(new LoggerLine(text, DateTime.Now));
        }

        public void Run()
        {
            while (!this.exit)
            {
                try
                {
                    if (this.loggerLines.Count > 0)
                    {
                        LoggerLine line = this.loggerLines.TakeOne();
                        try
                        {
                            if (AllowedToWrite())
                            {
                                WriteToFile(line);
                            }
                        }
                        finally
                        {
                            this.loggerLines.Remove(line);
                        }
                    }

                    if (Finished())
                    {
                        this.exit = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Thread.Sleep(30);
            }

            Clean();
        }

        private void Clean()
        {
            this.loggerLines.Clean();
        }

        private bool Finished()
        {
            return this.quitWithFlush && this.loggerLines.Count == 0;
        }

        private bool AllowedToWrite()
        {
            return !this.exit || this.quitWithFlush;
        }

        private void WriteToFile(LoggerLine loggerLine)
        {
            if (NewFileNeedsToBeCreated())
            {
                this.logFile.Initialise(this.GetLogFileName());
            }

            this.logFile.WriteLine(loggerLine);
        }

        private bool NewFileNeedsToBeCreated()
        {
            bool needsToBeCreated = false;
            DateTime now = this.dateTimeProvider.GetNow();
            if ((now.Date - startDate.Date).Days != 0)
            {
                startDate = now;
                needsToBeCreated = true;
            }

            return needsToBeCreated;
        }

        private string GetLogFileName()
        {
            return $"{Path.Combine(this.options.Value.FilePath, FileName)}_{DateTime.Now.ToString(DateTimeFormat)}.log";
        }
    }
}