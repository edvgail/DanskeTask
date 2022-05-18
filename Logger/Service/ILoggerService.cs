namespace Company.Logger.Service
{
    public interface ILoggerService
    {
        void StopWithoutFlush();
        void StopWithFlush();
        void Write(string text);
        void Run();
    }
}