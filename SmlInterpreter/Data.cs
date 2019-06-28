using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SmlInterpreter
{
	public interface IClonable<T>
	{
		T Clone();
	}

	public class File
	{
		public static File prefab;
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

		public void Execute()
		{
			for (int i = 0; i < Tree.Count; i++)
			{
				Tree[i].Execute();
			}
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

			return token;
		}

		public virtual Token Clone()
		{
			throw new NotImplementedException();
		}

		public abstract SmlBaseType Execute();

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

		public override SmlBaseType Execute() => null;

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
		public List<Statement> Body { get; private set; } = new List<Statement>();
	}

	public class If_Procedure : Procedure
	{
		protected If_Procedure() { }

		public Term Head { get; private set; }

		public new static If_Procedure Create(ContinueQueue parsingObject)
		{
			If_Procedure prefab = new If_Procedure();

			prefab.Head = Expression.Create(parsingObject);

			// At this moment, the next string should be ")", then remove it
			parsingObject.Dequeue();

			// the next string should be "{", remove it
			parsingObject.Dequeue();

			while (parsingObject.Count != 0 && parsingObject.Peek() != "}")
			{
				prefab.Body.Add(Statement.Create(parsingObject));
				// remove '}'
				parsingObject.Dequeue();
			}

			return prefab;
		}

		public override SmlBaseType Execute()
		{
			if ((Head.Execute() as SmlBool).Data)
			{
				for (int i = 0; i < Body.Count; i++)
				{
					Body[i].Execute();
				}
			}

			return null;
		}
	}

	public class Method : Procedure
	{
		public override SmlBaseType Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class Statement : Token
	{
		protected Statement() { }

		public static new Statement Create(ContinueQueue parsingObject)
		{
			Statement prefab = new Statement();
			prefab.Expression = Expression.Create(parsingObject);
			// The next character should be ';', remove it
			parsingObject.Dequeue();

			return prefab;
		}

		public override SmlBaseType Execute()
		{
			Expression.Execute();

			return null;
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
		protected Expression() { }
		public List<Expression> parameters { get; private set; } = new List<Expression>();
		public string Name { get; protected set; }
		public File DefinitionFile { get; private set; }
		public static Expression Create(ContinueQueue parsingObject, File definitionFile = null)
		{
			string termContent = parsingObject.Dequeue();

			Expression prefab = null;

			if (char.IsLetterOrDigit(termContent[0]))
			// It is a function or variable, or numeric literal
			{
				if (char.IsLetter(termContent[0]))
				// It is a variable or function
				{
					if (parsingObject.Peek() == "(")
					// It is a function
					{
						parsingObject.Dequeue();

						prefab = new Expression();
						prefab.Name = termContent;

						string next = parsingObject.Peek();

						while (next != ")")
						{
							prefab.parameters.Add(Expression.Create(parsingObject));
							next = parsingObject.Peek();
							if (next == ",")
							{
								parsingObject.Dequeue();
							}
						}

						if (next == ")")
						{
							parsingObject.Dequeue();
							return prefab;
						}
					}
					else if (parsingObject.Peek() == ":")
					{

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
				switch (termContent)
				{
					case "(":
						// It is a term in the form of (Expression())
						prefab = Expression.Create(parsingObject);
						break;

					case "\"":
						prefab = FormatString.Create(parsingObject);
						break;

					default:
						break;
				}
			}

			return prefab;
		}

		public override SmlBaseType Execute()
		{
			if (DefinitionFile == null)
			{
				return StandardLibrary.Invoke(StandardLibrary.Access(nameof(StandardLibrary), Name), parameters);
			}
			else
			{
				throw new NotImplementedException();
			}
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

		public SmlBaseType Value { get; set; }

		public override SmlBaseType Execute()
		{
			return Value;
		}
	}

	public class FormatString : Expression
	{
		protected FormatString() { }

		public new static FormatString Create(ContinueQueue parsingObject)
		{
			FormatString prefab = new FormatString();
			string current = parsingObject.Dequeue(false);
			int nameCounter = 0;
			while (current != "\"")
			{
				Expression term;
				if (current == "{")
				{
					term = Expression.Create(parsingObject);
					prefab.Literals.Add(term);
					// it should be removing character '}'
					parsingObject.Dequeue();
				}
				else
				{
					prefab.Literals.Add(Variable.Create($"string{nameCounter}", new SmlString(current)));
				}

				current = parsingObject.Dequeue();
				nameCounter++;
			}

			return prefab;
		}

		public List<Expression> Literals { get; private set; } = new List<Expression>();

		public override SmlBaseType Execute()
		{
			StringBuilder builder = new StringBuilder();

			for (int i = 0; i < Literals.Count; i++)
			{
				builder.Append(Literals[i].Execute().ToString());
			}

			return new SmlString(builder.ToString());
		}
	}

	public class Label
	{
		public Label(Token attachedObject)
		{
			Labels.Add(attachedObject);
		}

		public static List<Token> Labels { get; set; } = new List<Token>();
		
		public string Name { get; set; }
	}
}
