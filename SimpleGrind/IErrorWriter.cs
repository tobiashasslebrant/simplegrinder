using System.IO;

namespace SimpleGrind
{
    public interface IErrorWriter
    {
        void WriteLine(string str);
    }

    public class ErrorWriter : IErrorWriter
    {
        private readonly TextWriter _textWriter;

        public ErrorWriter(TextWriter textWriter) => _textWriter = textWriter;

        public void WriteLine(string str)
            => _textWriter.WriteLine(str);

    }
}