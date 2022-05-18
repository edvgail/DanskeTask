using Company.Logger.Model;

namespace Company.Logger.Stream
{
    public interface ILogFile
    {
        void WriteLine(LoggerLine loggerLine);
        void Initialise(string path);
    }
}