using System;
using System.Collections.Generic;

namespace SmlInterpreter
{
	public class ContinueQueue : Queue<string>
	{
		private string[] source;
		public int currentLine { get; private set; } = 0;

		public ContinueQueue(string[] source)
		{
			this.source = source;
		}

		public new string Dequeue()
		{
			return Dequeue(true);
		}

		public string Dequeue(bool ignoreWhiteSpace)
		{
			if (ignoreWhiteSpace)
			{
				RemoveHeadingSpaces();
			}

			if (Count == 0)
			{
				if (currentLine != source.Length - 1)
				{
					Queue<string> next = Parse.Segregate(source[++currentLine]);
					while (next.Count != 0)
					{
						Enqueue(next.Dequeue());
					}
				}
			}

			return base.Dequeue();
		}

		public void RemoveHeadingSpaces()
		{
			string temp = Peek();
			while (temp[0] == ' ' || temp[0] == '\t')
			{
				Dequeue();
				temp = Peek();
			}
		}
	}
}