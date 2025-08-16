using System.IO;
using System.Text;

namespace MSTestBench;

[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
public sealed class MicrosoftTestingPlatformEntryPoint
{
    public static async global::System.Threading.Tasks.Task<int> EntryPoint(string[] args)
    {
        global::Microsoft.Testing.Platform.Builder.ITestApplicationBuilder builder = await global::Microsoft.Testing.Platform.Builder.TestApplication.CreateBuilderAsync(args);
        // SelfRegisteredExtensions.AddSelfRegisteredExtensions(builder, args);
        builder.AddMSTest(() => [typeof(MicrosoftTestingPlatformEntryPoint).Assembly]);
        using (global::Microsoft.Testing.Platform.Builder.ITestApplication app = await builder.BuildAsync())
        {
            return await app.RunAsync();
        }
    }
}
