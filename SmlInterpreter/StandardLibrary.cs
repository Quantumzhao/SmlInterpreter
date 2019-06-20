﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SmlInterpreter
{
	public static class StandardLibrary
	{
		public static void print()
		{

		}
	}

	public class SmlBaseType
	{
		protected SmlBaseType() { }

		public static SmlBaseType Create(LineNumber label)
		{
			return new SmlBaseType() { Label = label};
		}

		public LineNumber Label { get; set; }
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

	}

	public class SmlString : SmlBaseType
	{

	}

	public class SmlArray : SmlBaseType
	{

	}
}
