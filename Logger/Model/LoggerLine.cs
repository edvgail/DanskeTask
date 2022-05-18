using System.Text;
using Validation;

namespace Company.Logger.Model
{
    public class LoggerLine
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss:fff";

        private readonly string text;
        private readonly DateTime timestamp;

        public LoggerLine(string text, DateTime timestamp)
        {
            this.text = Requires.NotNull(text, nameof(text));
            this.timestamp = timestamp;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(this.timestamp.ToString(DateTimeFormat));
            stringBuilder.Append('\t');
            if (this.text.Length > 0)
            {
                stringBuilder.Append(this.text);
                stringBuilder.Append(". ");
            }
            stringBuilder.Append('\t');

            return stringBuilder.ToString();
        }
    }
}