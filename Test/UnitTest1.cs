using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Test
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			using (StreamReader reader = new StreamReader("TestData1.txt"))
			{
				System.Console.WriteLine(reader.ReadLine());
			}
		}
	}
}
