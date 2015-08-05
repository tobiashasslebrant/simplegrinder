using System;

namespace SimpleGrindRunner
{
	public static class GridConsole
	{
		private static string _console = "";

		public static void WriteCell(string cell)
		{
			Console.Clear();
			var paddedCell = cell.PadRight(12);
			_console += paddedCell;
			Console.Write(_console);
		}
		public static void WriteCells(string[] cells)
		{
			foreach (var s in cells)
				WriteCell(s);
			_console += "\r\n";
			Console.WriteLine();
		}

		public static void WriteNoPersistantCells(string[] cells)
		{
			Console.Clear();
			Console.Write(_console);
			foreach (var s in cells)
				Console.Write(s.PadRight(12));
			Console.WriteLine();
		}

		public static void WriteLine(string line, params object[] args)
		{
			Console.Clear();
			_console += string.Format(line, args) + "\r\n";
			Console.Write(_console);
		}
	}
}