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
			using (StreamReader reader = new StreamReader("../../../../SmlInterpreter/TestData1.txt"))
			{

				var s = File.ReadAllText("../../../../SmlInterpreter/TestData1.txt");
				char a = s[0];
			}
		}
	}
}
