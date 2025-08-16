using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

//TestScenarioGenerator.GenerateTestScenario(
//    "C:\\Users\\ygerges\\source\\repos\\MSTestBench\\MSTestBench\\",
//    "ThousandClass200Tests_Delay_100ms_1MB",
//    numberOfTestClasses: 1000,
//    numberOfTestMethodsPerClass: 200,
//    sleepMSInEachTestMethod: 200,
//    bytesAllocatedInEachTestMethod: 1000);
BenchmarkRunner.Run<Bench>();

internal static class TestScenarioGenerator
{
    public static void GenerateTestScenario(
        string basePath,
        string testScenarioName,
        int numberOfTestClasses,
        int numberOfTestMethodsPerClass,
        int sleepMSInEachTestMethod,
        int bytesAllocatedInEachTestMethod)
    {
        if (!Directory.Exists(Path.Combine(basePath, testScenarioName)))
        {
            Directory.CreateDirectory(Path.Combine(basePath, testScenarioName));
        }
        
        for (int i = 0; i < numberOfTestClasses; i++)
        {
            string className = $"TestClass{i + 1}";
            string classPath = Path.Combine(basePath, testScenarioName, $"{className}.cs");
            using var writer = new StreamWriter(classPath);
            writer.WriteLine($"#if {testScenarioName.ToUpper()}");
            writer.WriteLine("namespace MSTestBench;");
            writer.WriteLine();
            writer.WriteLine("[TestClass]");
            writer.WriteLine($"public class {className}");
            writer.WriteLine("{");
            for (int j = 0; j < numberOfTestMethodsPerClass; j++)
            {
                writer.WriteLine($"    [TestMethod]");
                writer.WriteLine($"    public void TestMethod{j + 1}()");
                writer.WriteLine("     {");
                if (sleepMSInEachTestMethod > 0)
                {
                    writer.WriteLine($"        System.Threading.Thread.Sleep({sleepMSInEachTestMethod});");
                }

                if (bytesAllocatedInEachTestMethod > 0)
                {
                    writer.WriteLine($"        byte[] buffer = new byte[{bytesAllocatedInEachTestMethod}];");
                    writer.WriteLine("        new Random().NextBytes(buffer);");
                }

                writer.WriteLine("    }");
            }

            writer.WriteLine("}");
            writer.WriteLine("#endif");
        }
    }
}

[Config(typeof(Config))]
[MemoryDiagnoser]
public class Bench
{
    private class Config : ManualConfig
    {
        public Config()
        {
            var job = Job.Default
                .WithStrategy(RunStrategy.Monitoring) // only monitoring
                .WithWarmupCount(3) // light warm-up
                .WithIterationCount(10) // only 10 interations
                .WithLaunchCount(1); // launch a test only once
            var jobs = new List<Job>();
            // IMPORTANT: Don't try to mix 3.x and 4.x
            // BDN won't handle it correctly because of the assembly rename. It will use whatever version in MSTestBench.csproj when running 4.x.
            foreach (var version in (ReadOnlySpan<string>)["3.11.0-preview.25408.2", "3.11.0-preview.25415.6"])
            {
                foreach (var testScenario in (ReadOnlySpan<string>)["SingleClass10KTests", "ThousandClass200Tests", "HundredClass2000Tests"])
                {
                    var currentJob = job
                        .WithNuGet("MSTest", version)
                        .WithCustomBuildConfiguration("Debug")
                        .WithArguments([new MsBuildArgument($"/p:TestScenario={testScenario}")])
                        .WithId($"{testScenario}-{version}");

                    if (testScenario.Contains("100ms"))
                    {
                        currentJob = currentJob.WithWarmupCount(0).WithIterationCount(1);
                    }

                    jobs.Add(currentJob);
                }
            }

            AddJob(jobs.ToArray());

            // add the percentage column
            SummaryStyle =
                BenchmarkDotNet.Reports.SummaryStyle.Default
                .WithRatioStyle(BenchmarkDotNet.Columns.RatioStyle.Percentage);
            this.HideColumns("Arguments", "NuGetReferences", "Method");
            Options |= ConfigOptions.DisableOptimizationsValidator | ConfigOptions.GenerateMSBuildBinLog;
        }
    }

    [Benchmark]
    public async Task Benchmark()
    {
        await MSTestBench.MicrosoftTestingPlatformEntryPoint.EntryPoint([]);
    }
}
