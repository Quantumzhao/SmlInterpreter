using System;
using System.Collections.Generic;
using System.Text;

namespace SmlInterpreter
{
	public interface IOutputDevice
	{
		void WriteLine(string s);
		void Write(string s);
	}

	public class IntegratedConsole : IOutputDevice
	{
		public void WriteLine(string s) => Console.WriteLine(s);
		public void Write(string s) => Console.Write(s);
	}
}
