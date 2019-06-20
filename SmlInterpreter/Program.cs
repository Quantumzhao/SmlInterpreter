using System;
using System.Collections;
using System.Collections.Generic;

namespace SmlInterpreter
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] s = System.IO.File.ReadAllLines("TestData.txt");
			InterpreterEntryPoint.Execute(s);
		}
	}
}
