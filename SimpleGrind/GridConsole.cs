using System;
using System.IO;
using SimpleGrind.Parameters;

namespace SimpleGrind
{
	public interface IGridWriter
    {
        void WriteHeaders(string[] headers);
        void WriteCell(string cell);
        void WriteCells(string[] cells);
	    void WriteLine(string line);
    }

    public class GridConsole : IGridWriter
	{
        public GridConsole(TextWriter writer, int columnWidth, int noOfColumns)
        {
            _writer = writer;
            _columnWidth = columnWidth;
            _noOfColumns = noOfColumns;
        }

		readonly TextWriter _writer;
		readonly int _columnWidth;
		readonly int _noOfColumns;
		int _columnIndex = 0;

        public void WriteHeaders(string[] headers)
        {
	        if (headers.Length != _noOfColumns)
                throw new ArgumentException($"Number of headers ({headers.Length}) does not match number of columns ({_noOfColumns})");
	        WriteCells(headers);
        }
        public void WriteCell(string cell)
		{
        	var paddedCell = cell.PadRight(_columnWidth);
		    _writer.Write(paddedCell);
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

		public void WriteLine(string line)
		{
			if (_columnIndex > 0)
			{
				_columnIndex = 0;
				_writer.WriteLine();
			}
			_writer.WriteLine(line);
		}
	}
}