namespace Company.Logger
{
    public interface ILogger
    {
        void StopWithoutFlush();
        void StopWithFlush();
        void Write(string text);
    }
}