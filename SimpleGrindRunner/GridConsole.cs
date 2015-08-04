using System;

namespace SimpleGrindRunner
{
	public class GridConsole
	{
		private static string _console = "";
		
		public static void WriteCell(string cell)
		{
			Console.Clear();
			var paddedCell = cell.PadRight(12);
			_console += paddedCell;
			Console.Write(_console);
		}
		public static void WriteLine(string[] cells)
		{
			foreach (var s in cells)
				WriteCell(s);
			_console += "\r\n";
			Console.WriteLine();
		}

		public static void WriteNoPersistantLine(string[] cells)
		{
			Console.Clear();
			Console.Write(_console);
			foreach (var s in cells)
				Console.Write(s.PadRight(12));
			Console.WriteLine();
		}
	}
}