using System;

namespace SimpleGrindRunner
{
	public class Cons
	{
		private static string _console = "";
		
		public static void Write(string s)
		{
			Console.Clear();
			var msg = s.PadRight(12);
			_console += msg;
			Console.Write(_console);
		}
		public static void WriteLine(string[] arr)
		{
			foreach (var s in arr)
				Write(s);
			_console += "\r\n";
			Console.WriteLine();
		}

		public static void WriteBufferLine(string[] arr)
		{
			Console.Clear();
			Console.Write(_console);
			foreach (var s in arr)
				Console.Write(s.PadRight(12));
			Console.WriteLine();
		}
	}
}