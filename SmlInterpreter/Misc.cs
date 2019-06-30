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
			Queue<string> next = Parse.Segregate(source[currentLine]);
			while (next.Count != 0)
			{
				Enqueue(next.Dequeue());
			}

			RemoveHeadingSpaces();
		}

		public new string Dequeue()
		{
			return Dequeue(true);
		}

		public string Dequeue(bool ignoreWhiteSpace)
		{
			if (Count == 0)
			{
				if (currentLine != source.Length - 1)
				{
					LoadMore();
				}
			}

			string temp = base.Dequeue();

			if (ignoreWhiteSpace && Count != 0)
			{
				RemoveHeadingSpaces();
			}
			else if (Count == 0)
			{
				LoadMore();
			}

			return temp;
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
			while (Count == 0 && currentLine != source.Length - 1)
			{
				Queue<string> next = Parse.Segregate(source[++currentLine]);
				while (next.Count != 0)
				{
					Enqueue(next.Dequeue());
				}
			}
		}
	}

	public class CQueue
	{
		public CQueue(string[] source)
		{
			this.source = source;
		}

		private LinkedList<string> queue = new LinkedList<string>();
		private string[] source;

		public void Enqueue(string item)
		{
			queue.AddLast(item);
		}

		public string Dequeue()
		{
			string temp = queue.First.Value;
			queue.Remove(queue.First);

			return temp;
		}

		public string Peek(int index = 0)
		{
			LinkedListNode<string> temp = queue.First;
			while (index != 0)
			{
				temp = temp.Next;
				index--;
			}

			return temp.Value;
		}
	}
}