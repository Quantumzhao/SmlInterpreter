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
		public List<File> Referrences {get; private set; } = new List<File>();
		public bool IsReadOnly { get; set; } = false;

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
						token = FormatString.Create(parsingObject);
						break;

					//  # Pre-interpretation option. eg. #include <stdlib>
					case '#':
						token = PreInterpretationStatement.Create(parsingObject);
						break;

					// // Comment. eg. // Comment
					case '/':
						if (parsingObject.Peek()[0] == '/')
						{
							parsingObject.Dequeue();
							token = Comment.Create(parsingObject);
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

			token.Label = label;
			return token;
		}

		public Label Label { get; set; } = null;
		public Token Parent { get; set; } = null;

		public virtual Token Clone()
		{
			throw new NotImplementedException();
		}

		public abstract SmlBaseType Execute();

	}

	public class Comment : Statement
	{
		protected Comment() { }

		public static new Comment Create(ContinueQueue parsingObject)
		{
			Label temp = Label.Create(parsingObject);

			StringBuilder builder = new StringBuilder();
			while (parsingObject.Count != 0)
			{
				builder.Append(parsingObject.Dequeue());
			}

			return new Comment() { Content = builder.ToString(), Label = temp };
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
		public List<Token> Body { get; private set; } = new List<Token>();
		public List<Variable> LocalVariables { get; private set; } = new List<Variable>();
	}

	public class If_Procedure : Procedure
	{
		protected If_Procedure() { }

		public Term Head { get; private set; }

		private Else_Procedure Else = null;

		public new static If_Procedure Create(ContinueQueue parsingObject)
		{
			If_Procedure prefab = new If_Procedure();

			Label headLabel = Label.Create(parsingObject);

			// This should be `(`
			parsingObject.Dequeue();
			prefab.Head = Expression.Create(parsingObject);
			prefab.Head.Parent = prefab;
			// This should be `)`
			parsingObject.Dequeue();
			prefab.Head.Label = headLabel;

			// the next string should be "{", remove it
			parsingObject.Dequeue();

			while (parsingObject.Peek() != "}")
			{
				Label bodyLabel = Label.Create(parsingObject);
				var temp = Statement.Create(parsingObject);
				temp.Parent = prefab;
				temp.Label = bodyLabel;
				temp.Expression.Label = bodyLabel;

				prefab.Body.Add(temp);
			}

			// remove '}'
			parsingObject.Dequeue();

			prefab.Else = Else_Procedure.Create(parsingObject);

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
			else
			{
				Else?.Execute();
			}

			return null;
		}

		private class Else_Procedure : Procedure
		{
			private Else_Procedure() { }

			public static Else_Procedure Create(ContinueQueue parsingObject)
			{
				Else_Procedure prefab = null;

				if (parsingObject.Peek() == "else")
				{

				}

				return prefab;
			}

			public override SmlBaseType Execute()
			{
				for (int i = 0; i < Body.Count; i++)
				{
					Body[i].Execute();
				}

				return null;
			}
		}
	}

	public class Method : Procedure
	{
		public List<Variable> Parameters { get; private set; } = new List<Variable>();
		public string Name { get; set; }

		public new static Method Create(ContinueQueue parsingObject)
		{
			Method prefab = new Method();

			prefab.Name = parsingObject.Dequeue();

			// the next char should be `(`
			parsingObject.Dequeue();

			string next = parsingObject.Peek();

			while (next != ")")
			{
				Label paramLabel = Label.Create(parsingObject);
				var param = Variable.Create(parsingObject.Dequeue(), null);
				param.Label = paramLabel;
				prefab.Parameters.Add(param);
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

			// the next string should be "{", remove it
			parsingObject.Dequeue();

			while (parsingObject.Peek() != "}")
			{
				Label bodyLabel = Label.Create(parsingObject);
				var temp = Statement.Create(parsingObject);
				temp.Label = bodyLabel;
				temp.Expression.Label = bodyLabel;

				prefab.Body.Add(temp);
			}

			// remove '}'
			parsingObject.Dequeue();

			return prefab;

		}

		public override SmlBaseType Execute()
		{
			throw new NotImplementedException();
		}
	}

	public class Statement : Token
	{
		protected Statement() { }

		public new static Statement Create(ContinueQueue parsingObject)
		{
			Statement prefab = new Statement();

			Label label = Label.Create(parsingObject);
			prefab.Expression = Expression.Create(parsingObject);
			// A statement share the same label as its expression
			prefab.Label = label;
			prefab.Expression.Label = label;

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

						string next = parsingObject.Peek();

						while (next != ")")
						{
							Label paramLabel = Label.Create(parsingObject);
							var exp = Expression.Create(parsingObject);
							exp.Label = paramLabel;
							prefab.parameters.Add(exp);
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

						Label termLabel = Label.Create(parsingObject);
						prefab = Expression.Create(parsingObject, null);
						prefab.Label = termLabel;
						// Next char should be `)`
						parsingObject.Dequeue();
						break;

					case "\"":
						Label stringLabel = Label.Create(parsingObject);
						prefab = FormatString.Create(parsingObject);
						prefab.Label = stringLabel;
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
				return StandardLibrary.Invoke(
					StandardLibrary.Access(
						nameof(StandardLibrary), 
						Name
					),
					parameters
				);
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

	public class PreInterpretationStatement : Statement
	{
		protected PreInterpretationStatement() { }

		public new static PreInterpretationStatement Create(ContinueQueue parsingObject)
		{
			PreInterpretationStatement prefab = new PreInterpretationStatement();

			string next = parsingObject.Dequeue();
			switch (next)
			{
				case "include":
					prefab.CreateInclude(parsingObject);
					break;

				case "readonly":
					File.prefab.IsReadOnly = true;
					break;

				default:
					break;
			}

			return prefab;
		}

		private void CreateInclude(ContinueQueue parsingObject)
		{
			// The next character should be `<`
			parsingObject.Dequeue();
			File.prefab.Referrences.Add(File.Create(System.IO.File.ReadAllLines(parsingObject.Dequeue())));
			// The next character should be `>`
			parsingObject.Dequeue();
		}
	}

	public class Label
	{
		protected Label() { }

		/// <Summary>
		/// Attempts to create a label and updates the `parsingObject` correspondingly
		/// </Summary>
		public static Label Create(ContinueQueue parsingObject)
		{
			Label prefab = new Label();

			string name = parsingObject.Peek();
			if (char.IsLetter(name[0]))
			// Check if it is a potential label
			{
				if (parsingObject.Peek(1) == "_")
				// Handling `_` subscript
				{
					parsingObject.Dequeue();
					prefab.Name.Subscript = Expression.Create(parsingObject);
				}
				else if (parsingObject.Peek(1) == ":")
				// Handling normal cases
				{
					prefab.Name.Main = name;
					prefab.Name.Subscript = null;

					parsingObject.Dequeue();
					parsingObject.Dequeue();

					return prefab;
				}
			}

			// It is not a label
			return null;
		}

		public static List<Label> Labels { get; set; } = new List<Label>();
		
		public Token Parent { get; private set; }

		public LabelName Name { get; set; } = new LabelName();

		public class LabelName
		{
			public string Main { get; set; }
			public Expression Subscript { get; set; }

			public string Literal => Main + (Subscript.Execute()?.ToString() ?? "");
		}
	}
}
