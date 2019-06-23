using System;
using System.Collections.Generic;
using System.Text;

namespace SmlInterpreter
{
	public static class StandardLibrary
	{
		public static void print(SmlBaseType s)
		{
			IOutputDevice console = new IntegratedConsole();
			console.WriteLine(s.ToString());
		}

		public static SmlString ToSmlString(SmlInt value)
		{
			return new SmlString(value.Value.ToString());
		}

		public static SmlString ToSmlString(SmlBool value)
		{
			return new SmlString(value.Data.ToString());
		}
	}

	public abstract class SmlBaseType
	{
		protected SmlBaseType() { }

		public virtual object Data { get; set; }
	}

	public class SmlInt : SmlBaseType
	{
		public int Value { get; set; }
		public new int Data { get; set; }
	}

	public class SmlDouble : SmlBaseType
	{
		public double Value { get; set; }

		public new int Data { get; set; }
	}

	public class SmlBool : SmlBaseType
	{
		public SmlBool(bool value)
		{
			Data = value;
		}

		public readonly SmlTypes Type = SmlTypes.Bool;
		public new bool Data { get; set; }
	}

	public class SmlString : SmlBaseType
	{
		public SmlString(string content)
		{
			Data = content;
		}

		public new string Data { get; set; }

		public override string ToString()
		{
			return Data;
		}
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
