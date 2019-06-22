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
		private static File prefab;
		public List<Token> Tree = new List<Token>();
		private string[] sourcecode;

		public static File Create(string[] source)
		{
			prefab = new File();
			prefab.sourcecode = source;

			ContinueQueue queue = new ContinueQueue(source);
			while (queue.Count != 0)
			{
				prefab.Tree.Add(Token.Create(queue));
			}

			return prefab;
		}
	}

	public abstract class Token : IClonable<Token>
	{
		protected Token() { }
		public static Token Create(ContinueQueue parsingObject)
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
					* \ transcribe	eg. "{Label1: \"1\"}"
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
						break;
					case '{':
						break;

					case '\"':
						break;

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
				switch (content)
				{
					case "if":
						token = If_Procedure.Create(parsingObject);
						break;

					default:
						break;
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

		public new static Comment Create(ContinueQueue parsingObject)
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

	public abstract class Procedure : Token
	{
		protected Procedure() { }
		public new static Variable Create(ContinueQueue parsingObject)
		{
			throw new NotImplementedException();
		}
		public List<Statement> Body { get; private set; }
	}

	public class Method : Procedure
	{
		protected Method() { }

		public static new Method Create(ContinueQueue parsingObject)
		{
			throw new NotImplementedException();
		}
	}

	public class If_Procedure : Procedure
	{
		protected If_Procedure() { }

		public Term Head { get; private set; }

		public new static If_Procedure Create(ContinueQueue parsingObject)
		{
			parsingObject.RemoveHeadingSpaces();
			If_Procedure prefab = new If_Procedure();

			prefab.Head = Expression.Create(parsingObject);

			parsingObject.RemoveHeadingSpaces();
			// At this moment, the next string should be ")", then remove it
			parsingObject.Dequeue();

			parsingObject.RemoveHeadingSpaces();
			// the next string should be "{", remove it
			parsingObject.Dequeue();

			parsingObject.RemoveHeadingSpaces();
			while (parsingObject.Peek()[0] != '}')
			{
				prefab.Body.Add(Statement.Create(parsingObject));
				parsingObject.RemoveHeadingSpaces();
			}

			return prefab;
		}
	}

	public class Statement : Token
	{
		protected Statement() { }

		public static new Statement Create(ContinueQueue parsingObject)
		{
			Statement prefab = new Statement();
			prefab.Expression = Expression.Create(parsingObject);


			return prefab;
		}

		public Expression Expression { get; private set; }
	}

	public abstract class Term : Token
	{
		public new static Term Create(ContinueQueue parsingObject)
		{
			throw new NotImplementedException();
		}
	}

	public class Expression : Term
	{
		public List<Expression> parameters { get; private set; } = new List<Expression>();
		public string Name { get; protected set; }
		public File DefinitionFile { get; private set; }
		public new static Expression Create(ContinueQueue parsingObject)
		{
			parsingObject.RemoveHeadingSpaces();

			string termContent = parsingObject.Dequeue();
			parsingObject.RemoveHeadingSpaces();

			Expression prefab;

			if (char.IsLetterOrDigit(termContent[0]))
			// It is a function or variable, or numeric literal
			{
				if (char.IsLetter(termContent[0]))
				// It is a variable or function
				{
					if (parsingObject.Peek()[0] == '(')
					// It is a function
					{
						prefab = new Expression();
						prefab.Name = termContent;
					}
					else
					// It is a variable (declared)
					{
						switch (termContent)
						{
							case "true":
							case "false":
								prefab = Variable.Create(termContent, new SmlBool(true));
								break;

							default:
								break;
						}
					}
				}
			}
			else
			{
				switch (termContent[0])
				{
					case '(':
						// It is a term in the form of (Expression())
						prefab = Expression.Create(parsingObject);
						break;

					default:
						break;
				}
			}

			throw new NotImplementedException();
		}
	}

	public class Variable : Expression
	{
		protected Variable() { }

		public static Variable Create(string name, SmlBaseType value)
		{
			Variable prefab = new Variable();
			prefab.Name = name;
			prefab.Value = value;

			return prefab;
		}

		public SmlBaseType Value { get; private set; }
	}
}
