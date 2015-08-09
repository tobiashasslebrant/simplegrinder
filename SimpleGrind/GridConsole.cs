using System;
using System.IO;

namespace SimpleGrind
{
    public interface IGridWriter
    {
        void WriteHeaders(string[] headers);
        void WriteCell(string cell);
        void WriteCells(string[] cells);
        void WriteLine(string line, params object[] args);
    }

    public class GridConsole : IGridWriter
	{
        public GridConsole(TextWriter writer, int columnWidth, int noOfColumns)
        {
            _writer = writer;
            _columnWidth = columnWidth;
            _noOfColumns = noOfColumns;
        }

        string _console = "";
        TextWriter _writer;
        int _columnWidth;
        int _noOfColumns;
        int _columnIndex = 0;

        public void WriteHeaders(string[] headers)
        {
            if (headers.Length != _noOfColumns)
                throw new ArgumentException($"Number of headers ({headers.Length}) does not match number of columns ({_noOfColumns})");
        }
        public void WriteCell(string cell)
		{
        	var paddedCell = cell.PadRight(_columnWidth);
		    _writer.Write(_console);
            if(++_columnIndex == _noOfColumns)
            {
                _writer.WriteLine();
                _columnIndex = 0;
            }

		}
		public void WriteCells(string[] cells)
		{
            foreach (var s in cells)
				WriteCell(s);
		}

		public void WriteLine(string line, params object[] args) => 
            _writer.WriteLine(line, args);
	}

   
}