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

		public static Queue<string> RemoveSucceedingSpaces(Queue<string> parsingObject)
		{
			string temp = parsingObject.Peek();
			while (temp[0] == ' ' || temp[0] == '\t')
			{
				parsingObject.Dequeue();
				temp = parsingObject.Peek();
			}

			return parsingObject;
		}
	}

	public class Token : IClonable<Token>
	{
		protected Token() { }
		public static Token Create(Queue<string> parsingObject)
		{
			Token token = null;

			string content = parsingObject.Dequeue();
			if (!char.IsLetterOrDigit(content[0]))
			{
				char character = content[0];
				switch (character)
				{
					/* possible metacharacters:
					* ! Not			eg. !true == false
					* ~ Range		eg. 0~9
					* % Modulus		eg. 8 % 5 == 3
					* * multiply	eg. 3 * 5 == 15
					* - subtract	eg. 5 - 3 == 2, 3 - 5 == -2
					* + Add			eg. 1 + 1 == 2
					* = assignment	eg. x = 1;
					* : Label		eg. Label1: Expression();, Expression({Label1: "1"});
					* " string		eg. "Hello world"
					* < smaller		eg. 3 < 5 == true
					*   spcl char	eg. #include <stdlib>
					* > greater		eg. 5 > 3 == true
					* / divide		eg. 4 / 2 == 2
					* , seperator	eg. Expression(1, 2)
					* . Accessor	eg. lineof(9).Label
					*   double		eg. {a: 0.0}
					* \ transcribe	eg. "\{Label1: \"1\"\}"
					* _ array		eg. {arr_0: 1} (declaration), arr[0] = 2; (operation)
					* ; xprsn end	eg. x = 1;
					* += Add and Assignment
					* -= Minus and Assignment
					* <= smaller or equal to
					* >= greater or equal to
					* == is equal
					* || or
					* && and
					*/

					case '(':
						token = Term.Create(parsingObject, '(');
						break;
					case '{':
						break;

					case '\"':
						break;

					case ';':
						return token;

					//  # Pre-interpretation option. eg. #include <stdlib>
					case '#':
						
						break;

					// // Comment. eg. // Comment
					case '/':
						if (parsingObject.Peek()[0] == '/')
						{
							parsingObject.Dequeue();
							token = Comment.Create(parsingObject);
							return token;
						}
						break;

					default:
						break;
				}
			}
			else
			{
				parsingObject = Parse.RemoveSucceedingSpaces(parsingObject);
				string next = parsingObject.Dequeue();
				if (char.IsLetterOrDigit(next[0]))
				{
					switch (next[0])
					{
						case '(':
							token = Expression.Create(parsingObject, '(');
							break;

						case '[':
							break;

						default:
							break;
					}
				}
			}

			throw new NotImplementedException();
		}

		//public string Content { get; set; }

		public virtual Token Clone()
		{
			throw new NotImplementedException();
		}
	}

	public class Comment : Token
	{
		protected Comment() { }

		public new static Comment Create(Queue<string> parsingObject)
		{
			StringBuilder builder = new StringBuilder();
			while (parsingObject.Count != 0)
			{
				builder.Append(parsingObject.Dequeue());
			}

			return new Comment() { Content = builder.ToString() };
		}

		public readonly string Head = "//";
		public string Content { get; set; }
	}

	public class Procedure : Token
	{
		protected Procedure() { }
		public new static Variable Create(Queue<string> parsingObject)
		{
			throw new NotImplementedException();
		}
	}

	public class Expression : Token
	{
		public static Variable Create(Queue<string> parsingObject, char head)
		{
			throw new NotImplementedException();
		}
	}

	public class Term : Expression
	{
		public new static Variable Create(Queue<string> parsingObject, char head)
		{
			throw new NotImplementedException();
		}
	}

	public class Variable : Term
	{
		public new static Variable Create(Queue<string> parsingObject)
		{
			throw new NotImplementedException();
		}
	}
}
