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
		public List<Granule> Tree = new List<Granule>();

		public static File ParseSource(string[] source)
		{
			//for (int i = 0; i < source.Length; i++)
			//{
			//	SyntaxMarkUp.Produce(Parse.Segregate(source[i]), i);
			//}

			//return Reconstruct(SyntaxMarkUp.Group.Current);


		}


		private static File Reconstruct(SyntaxMarkUp.Group markUpGroup)
		{
			var c = markUpGroup.ParticleCollection;
			while (c.Count != 0)
			{

			}

			throw new NotImplementedException();
		}
		private static void ItemReconstruction(SyntaxMarkUp.Item item)
		{
			if (!char.IsLetterOrDigit(item.Content[0]))
			{
				SmlBaseType smlObject = SmlBaseType.Create(item.LineNo);
				char metaCharacter = item.Content[0];
				/* possible metacharacters:
				 * ! Not		eg. !true == false
				 * # Comment	eg. # Comment
				 * ~ Range		eg. 0~9
				 * % Modulus	eg. 8 % 5 == 3
				 * * multiply	eg. 3 * 5 == 15
				 * - subtract	eg. 5 - 3 == 2, 3 - 5 == -2
				 * + Add		eg. 1 + 1 == 2
				 * = assignment	eg. x = 1;
				 * : Label		eg. Label1: Expression();, Expression({Label1: "1"});
				 * " string		eg. "Hello world"
				 * < smaller	eg. 3 < 5 == true
				 * > greater	eg. 5 > 3 == true
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
				switch (metaCharacter)
				{
					case '#':
						break;

					default:
						throw new InvalidOperationException("Ungrammatical input");
				}
			}
		}
	}

	public abstract class Granule
	{
		protected Granule() { }

		public LineNumber LineNo { get; set; }
	}

	public class Fragment : Granule
	{
		public class Head : Line
		{

		}

		public class Body : Fragment
		{

		}
	}

	public class Line : Granule
	{

	}

	public class LineNumber : IClonable<LineNumber>
	{
		public LineNumber(int number)
		{
			Number = number;
		}
		public int Number { get; set; }

		public LineNumber Clone()
		{
			return new LineNumber(Number);
		}
	}

	public class Label : LineNumber
	{
		public static List<Granule> GranuleCollection = new List<Granule>();
		public Label(int lineNumber, string labelName) : base(lineNumber)
		{
			Name = labelName;
		}

		public string Name { get; set; }

		public new Label Clone()
		{
			return new Label(Number, Name);
		}
	}

	public class Expression
	{

	}
}
