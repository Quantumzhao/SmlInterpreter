using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SmlInterpreter
{
	public static class InterpreterEntryPoint
	{
		public static void Execute(string[] source)
		{
			Parse.ParseString(source);
		}
	}

	public static class Parse
	{
		public static Granule Current { get; set; } = null;
		public static void ParseString(string[] source)
		{
			File file = File.ParseSource(source);
		}

		public static Queue<string> Segregate(string source)
		{
			Queue<string> segregation = new Queue<string>();
			source = source.Trim();
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < source.Length; i++)
			{
				if (char.IsLetterOrDigit(source[i]))
				{
					builder.Append(source[i]);
				}
				else
				{
					if (builder.Length != 0)
					{
						segregation.Enqueue(builder.ToString());
					}

					builder.Clear();
					segregation.Enqueue(source[i].ToString());
				}
			}

			if (builder.Length != 0)
			{
				segregation.Enqueue(builder.ToString());
			}

			return segregation;
		}
	}
	public static class SyntaxMarkUp
	{
		public static void Produce(Queue<string> source, int lineNumber)
		{
			while (source.Count != 0)
			{
				string item = source.Dequeue();
				if (!Group.IsBracket(item[0]))
				{
					Group.Current.Add(new Item(item, new LineNumber(lineNumber)));
				}
				else
				{
					switch (item[0])
					{
						case '(':
						case '[':
						case '{':
							Group group = new Group(item[0], new LineNumber(lineNumber));
							Group.Current.Add(group);
							Group.Current = group;
							break;

						default:
							Group.Current = Group.Current.parent;
							break;
					}
				}
			}
		}

		public abstract class Particle
		{
			public Particle(LineNumber lineNo)
			{
				LineNo = lineNo;
			}
			public Group parent { get; set; }
			public LineNumber LineNo { get; set; }
		}

		public class Item : Particle
		{
			public Item(string content, LineNumber lineNo) : base(lineNo)
			{
				Content = content;
			}

			public string Content { get; set; }
		}

		public class Group : Particle
		{
			public Group(char type, LineNumber lineNo) : base(lineNo)
			{
				Type = type;
			}

			public char Type { get; set; }
			public static Group Current { get; set; } = new Group(' ', null);
			public Queue<Particle> ParticleCollection { get; set; } = new Queue<Particle>();

			public void Add(Particle particle)
			{
				particle.parent = this;
				ParticleCollection.Enqueue(particle);
			}

			public static bool IsBracket(char character)
			{
				return
					character == '(' ||
					character == ')' ||
					character == '[' ||
					character == ']' ||
					character == '{' ||
					character == '}';
			}
		}
	}

	public class Token
	{

	}
}
