using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmlInterpreter
{
	public interface IClonable<T>
	{
		T Clone();
	}

	public class File
	{
		private static File prefab = new File();
		public List<Token> Tree = new List<Token>();

		public static File ParseSource(string[] source)
		{
			for (int i = 0; i < source.Length; i++)
			{
				Queue<string> segregation = Parse.Segregate(source[i]);
				prefab.Tree.Add(Token.Create(segregation));
			}

			return prefab;
		}
	}
}
