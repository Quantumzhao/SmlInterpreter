using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmlInterpreter
{
	public static class InterpreterEntryPoint
	{
		public static File EntryFile { get; set; } = null;
		public static void Execute(string[] source)
		{
			Parse.ParseString(source);
			Parse.ExecuteFile();
		}
	}

	public static class Parse
	{
		public static void ParseString(string[] source)
		{
			InterpreterEntryPoint.EntryFile = File.Create(source);
		}
		public static void ExecuteFile()
		{
			InterpreterEntryPoint.EntryFile.Execute();
		}

		public static Queue<string> Segregate(string source)
		{
			Queue<string> segregation = new Queue<string>();
			source = source.Trim();
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < source.Length; i++)
			{
				if (char.IsLetterOrDigit(source[i]))
				{
					builder.Append(source[i]);
				}
				else
				{
					if (builder.Length != 0)
					{
						segregation.Enqueue(builder.ToString());
					}

					builder.Clear();
					segregation.Enqueue(source[i].ToString());
				}
			}

			if (builder.Length != 0)
			{
				segregation.Enqueue(builder.ToString());
			}

			return segregation;
		}
	}
}
