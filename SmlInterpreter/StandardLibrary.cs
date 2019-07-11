using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

namespace SmlInterpreter
{
	public static class StandardLibrary
	{
		public static void print(SmlBaseType s)
		{
			IOutputDevice console = new IntegratedConsole();
			console.Write(s.ToString());
		}

		public static Method Access(File file, string name)
		{
			throw new NotImplementedException();
		}
		public static MethodInfo Access(string libName, string name)
		{
			return Type.GetType($"SmlInterpreter.{libName}").GetMethod(name);
		}

		public static void Assign(Variable variable, Expression expression)
		{
			variable.Value = expression.Execute();
		}

		public static SmlBaseType Invoke(Method method, List<Expression> arguments)
		{
			return method.Execute();
		}
		public static SmlBaseType Invoke(MethodInfo method, List<Expression> arguments)
		{
			return method.Invoke(null, arguments.Select(p => p.Execute()).ToArray()) as SmlBaseType;
		}

		public static void Test()
		{
			Console.WriteLine("Test() Success");
		}
		public static void Test2(SmlBaseType s1, SmlBaseType s2)
		{
			Console.WriteLine($"Test(,) Success {s1} {s2}");
		}
		public static void Test3(SmlBaseType s1, SmlBaseType s2, SmlBaseType s3)
		{
			Console.WriteLine($"Test(,,) Success {s1} {s2} {s3}");
		}
	}

	public static class Math
	{
		public static SmlInt Add(SmlInt value1, SmlInt value2)
		{
			return new SmlInt(value1.Data + value2.Data);
		}
		public static SmlDouble Add(SmlDouble value1, SmlDouble value2)
		{
			return new SmlDouble(value1.Data + value2.Data);
		}

		public static SmlInt Minus(SmlInt value1, SmlInt value2)
		{
			return new SmlInt(value1.Data - value2.Data);
		}
		public static SmlDouble Minus(SmlDouble value1, SmlDouble value2)
		{
			return new SmlDouble(value1.Data - value2.Data);
		}

		public static SmlInt Multiply(SmlInt value1, SmlInt value2)
		{
			return new SmlInt(value1.Data * value2.Data);
		}
		public static SmlDouble Multiply(SmlDouble value1, SmlDouble value2)
		{
			return new SmlDouble(value1.Data * value2.Data);
		}

		public static SmlInt Divide(SmlInt value1, SmlInt value2)
		{
			return new SmlInt(value1.Data / value2.Data);
		}
		public static SmlDouble Divide(SmlDouble value1, SmlDouble value2)
		{
			return new SmlDouble(value1.Data / value2.Data);
		}

		public static SmlInt Modulus(SmlInt value1, SmlInt value2)
		{
			return new SmlInt(value1.Data % value2.Data);
		}
		public static SmlDouble Modulus(SmlDouble value1, SmlDouble value2)
		{
			return new SmlDouble(value1.Data % value2.Data);
		}

		public static SmlDouble Power(SmlDouble value1, SmlDouble value2)
		{
			return new SmlDouble(System.Math.Pow(value1.Data, value2.Data));
		}

		public static SmlInt Negate(SmlInt value)
		{
			return new SmlInt(-value.Data);
		}

		public static SmlBool IsGreater(SmlDouble value1, SmlDouble value2)
		{
			return new SmlBool(value1.Data > value2.Data);
		}
		public static SmlBool IsSmaller(SmlDouble value1, SmlDouble value2)
		{
			return new SmlBool(value1.Data < value2.Data);
		}
		public static SmlBool IsEqual(SmlDouble value1, SmlDouble value2)
		{
			return new SmlBool(value1.Data == value2.Data);
		}

	}

	public abstract class SmlBaseType
	{
		protected SmlBaseType() { }
	}

	public class SmlInt : SmlBaseType
	{
		public SmlInt(int data)
		{
			Data = data;
		}

		public int Data { get; set; }
	}

	public class SmlDouble : SmlBaseType
	{
		public SmlDouble(double data)
		{
			Data = data;
		}

		public double Data { get; set; }
	}

	public class SmlBool : SmlBaseType
	{
		public SmlBool(bool value)
		{
			Data = value;
		}

		public readonly SmlTypes Type = SmlTypes.Bool;
		public bool Data { get; set; }
	}

	public class SmlString : SmlBaseType
	{
		public SmlString(string content)
		{
			Data = content;
		}

		public string Data { get; set; }

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
