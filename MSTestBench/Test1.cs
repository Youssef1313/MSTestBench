using System.IO;
using System.Text;

namespace MSTestBench;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        //for (int i = 0; i < 100; i++)
        //{
        //    var builder = new StringBuilder();
        //    builder.AppendLine($$"""
        //    #if HUNDRED_CLASS_2000_TESTS
        //    namespace MSTestBench;

        //    [TestClass]
        //    public sealed class HundredClass2000Tests_{{i + 1}}
        //    {
        //    """);
        //    for (int j = 1; j <= 2000; j++)
        //    {
        //        builder.Append($$"""
        //            [TestMethod]
        //            public void TestMethod{{j}}()
        //            {
        //            }

        //        """);
        //    }

        //    builder.AppendLine("}");
        //    builder.AppendLine("#endif");
        //    File.WriteAllText($"C:\\Users\\ygerges\\source\\repos\\MSTestBench\\MSTestBench\\HundredClass2000Tests\\HundredClass2000Tests_{i + 1}.cs", builder.ToString(), Encoding.UTF8);
        //}
    }
}
