using System;
using System.Collections.Generic;
using System.Text;

namespace SmlInterpreter
{
	public static class StandardLibrary
	{
		public static void print(string s)
		{
			IOutputDevice console = new IntegratedConsole();
			console.WriteLine(s);
		}
	}

	public class SmlBaseType
	{
		protected SmlBaseType() { }
	}

	public class SmlInt : SmlBaseType
	{
		public int Data { get; set; }
	}

	public class SmlDouble : SmlBaseType
	{
		public double Data { get; set; }
	}

	public class SmlBool : SmlBaseType
	{
		public SmlBool(bool value)
		{
			Value = value;
		}

		public readonly SmlTypes Type = SmlTypes.Bool;
		public bool Value { get; set; }
	}

	public class SmlString : SmlBaseType
	{

	}

	public class SmlArray : SmlBaseType
	{

	}

	public enum SmlTypes
	{
		Integer,
		Double,
		Bool,
		String,
		Array,
		Line
	}
}
