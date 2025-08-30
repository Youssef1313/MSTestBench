#if IS_XUNIT
global using TestMethodAttribute = Xunit.FactAttribute;
global using TestClassAttribute = MSTestBench.DummyAttribute;
#endif

using System.IO;
using System.Text;

namespace MSTestBench;

#if IS_XUNIT
public sealed class DummyAttribute : System.Attribute { }
#endif

[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class MicrosoftTestingPlatformEntryPoint
{
    public static async global::System.Threading.Tasks.Task<int> EntryPoint(string[] args)
    {
#if IS_XUNIT
        System.Reflection.Assembly.SetEntryAssembly(typeof(MicrosoftTestingPlatformEntryPoint).Assembly);
        return await Xunit.Runner.InProc.SystemConsole.TestingPlatform.TestPlatformTestFramework.RunAsync(args, SelfRegisteredExtensions.AddSelfRegisteredExtensions);
#else
        global::Microsoft.Testing.Platform.Builder.ITestApplicationBuilder builder = await global::Microsoft.Testing.Platform.Builder.TestApplication.CreateBuilderAsync(args);
        // SelfRegisteredExtensions.AddSelfRegisteredExtensions(builder, args);
        builder.AddMSTest(() => [typeof(MicrosoftTestingPlatformEntryPoint).Assembly]);
        using (global::Microsoft.Testing.Platform.Builder.ITestApplication app = await builder.BuildAsync())
        {
            return await app.RunAsync();
        }
#endif
    }
}
