using System;
using System.Collections.Generic;

namespace SmlInterpreter
{
	public class ContinueQueue
	{
		public ContinueQueue(string[] source)
		{
			this.source = source;
			Queue<string> next = Parse.Segregate(source[currentLine]);
			while (next.Count != 0)
			{
				Enqueue(next.Dequeue());
			}

			RemoveHeadingSpaces();
		}

		private LinkedList<string> queue = new LinkedList<string>();
		private string[] source;
		public int currentLine { get; private set; } = 0;

		public int Count => queue.Count;

		public void Enqueue(string item)
		{
			queue.AddLast(item);
		}

		public string Dequeue(bool ignoreWhiteSpace = true)
		{
			if (queue.Count == 0)
			{
				if (currentLine != source.Length - 1)
				{
					LoadMore();
				}
			}

			string temp = queue.First.Value;
			queue.RemoveFirst();

			if (ignoreWhiteSpace && queue.Count != 0)
			{
				RemoveHeadingSpaces();
			}
			else if (queue.Count == 0)
			{
				LoadMore();
			}

			return temp;
		}

		public string Peek(int index = 0)
		{
			LinkedListNode<string> temp = queue.First;
			while (index != 0)
			{
				if (temp.Next == null)
				{
					LoadMore();
				}
				temp = temp.Next;
				if (!char.IsWhiteSpace(temp.Value[0]))
				{
					index--;
				}
			}

			return temp.Value;
		}

		private void RemoveHeadingSpaces()
		{
			string peek = Peek();
			while (peek[0] == ' ' || peek[0] == '\t')
			{
				Dequeue(false);
				peek = Peek();
			}
		}

		private void LoadMore()
		{
			while (queue.Count == 0 && currentLine != source.Length - 1)
			{
				Queue<string> next = Parse.Segregate(source[++currentLine]);
				while (next.Count != 0)
				{
					Enqueue(next.Dequeue());
				}
			}
		}
	}
}