using Company.Logger.Model;

namespace Company.Logger.Stream
{
    public class LogFile : ILogFile
    {
        private string header = $"{"Timestamp",-20}\tData\t";
        private StreamWriter? streamWriter;

        public void Initialise(string path)
        {
            if (path != null)
            {
                if (this.streamWriter != null)
                {
                    this.streamWriter.Close();
                }
                CreateFolderIfNotExists(path);
                this.streamWriter = new StreamWriter(path);
                this.streamWriter.WriteLine(this.header);
                this.streamWriter.AutoFlush = true;
            }
        }

        public void WriteLine(LoggerLine loggerLine)
        {
            if (this.streamWriter != null)
            {
                this.streamWriter.WriteLine(loggerLine.ToString());
            }
            else
            {
                throw new Exception("Log file is not initialised!");
            }
        }

        private void CreateFolderIfNotExists(string path)
        {
            string dir = Path.GetDirectoryName(path);
            if (dir != null)
            { 
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
        }
    }
}