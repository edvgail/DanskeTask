using Company.Logger.Service;
using Validation;

namespace Company.Logger
{
    public class Logger : ILogger
    {
        private readonly Thread runThread;
        private readonly ILoggerService loggerService;

        public Logger(ILoggerService loggerService)
        {
            this.loggerService = Requires.NotNull(loggerService, nameof(loggerService));
            this.runThread = new Thread(MainLoop);
            this.runThread.Start();
        }

        public void StopWithoutFlush()
        {
            this.loggerService.StopWithoutFlush();
        }

        public void StopWithFlush()
        {
            this.loggerService.StopWithFlush();
        }

        public void Write(string text)
        {
            this.loggerService.Write(text);
        }

        private void MainLoop()
        {
            this.loggerService.Run();
        }
    }
}