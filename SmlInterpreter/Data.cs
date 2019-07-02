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
			Label label = Label.Create(parsingObject);
			Token token = null;

			string content = parsingObject.Dequeue();
			if (!char.IsLetterOrDigit(content[0]))
			{
				char character = content[0];
				switch (character)
				{
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
							token = Comment.Create(parsingObject, label);
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
						token = If_Procedure.Create(parsingObject, label);
						break;

					default:
						break;
				}
			}

			return token;
		}

		public Label Label { get; set; } = null;

		public virtual Token Clone()
		{
			throw new NotImplementedException();
		}

		public abstract SmlBaseType Execute();

	}

	public class Comment : Statement
	{
		protected Comment() { }

		public static new Comment Create(ContinueQueue parsingObject, Label label)
		{
			StringBuilder builder = new StringBuilder();
			while (parsingObject.Count != 0)
			{
				builder.Append(parsingObject.Dequeue());
			}

			return new Comment() { Content = builder.ToString(), Label = label };
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

		public static If_Procedure Create(ContinueQueue parsingObject, Label label)
		{
			If_Procedure prefab = new If_Procedure();
			prefab.Label = label;

			Label headLabel = Label.Create(parsingObject);
			prefab.Head = Expression.Create(parsingObject, headLabel);

			// At this moment, the next string should be ")", then remove it
			parsingObject.Dequeue();

			// the next string should be "{", remove it
			parsingObject.Dequeue();

			while (parsingObject.Count != 0 && parsingObject.Peek() != "}")
			{
				Label bodyLabel = Label.Create(parsingObject);
				prefab.Body.Add(Statement.Create(parsingObject, bodyLabel));
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

		public static Statement Create(ContinueQueue parsingObject, Label label)
		{
			Statement prefab = new Statement();
			prefab.Label = label;

			prefab.Expression = Expression.Create(parsingObject, label);
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
		public static Expression Create(ContinueQueue parsingObject, Label label, File definitionFile = null)
		{
			string termContent = parsingObject.Dequeue();

			Expression prefab = null;

			if (char.IsLetterOrDigit(termContent[0]))
			// It is a not a meta character
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
						prefab.Label = label;

						string next = parsingObject.Peek();

						while (next != ")")
						{
							Label paramLabel = Label.Create(parsingObject);
							prefab.parameters.Add(Expression.Create(parsingObject, paramLabel));
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
						prefab = Expression.Create(parsingObject, null);
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
					Label label = Label.Create(parsingObject);
					term = Expression.Create(parsingObject, label);
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
		protected Label() { }

		public static Label Create(ContinueQueue parsingObject)
		{
			Label prefab = new Label();

			string name = parsingObject.Peek();
			if (char.IsLetter(name[0]) && parsingObject.Peek(1) == ":")
			{
				prefab.Name = name;
				parsingObject.Dequeue();
				parsingObject.Dequeue();

				Labels.Add(prefab);

				return prefab;
			}
			else
			{
				return null;
			}
		}

		public static List<Label> Labels { get; set; } = new List<Label>();
		
		public Token Parent { get; private set; }

		public string Name { get; set; }
	}
}
