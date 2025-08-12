using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

BenchmarkRunner.Run<Bench>();

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
            foreach (var version in (ReadOnlySpan<string>)["3.8.3", "3.9.3", "3.10.1", "3.11.0-preview.25408.2"])
            {
                foreach (var testScenario in (ReadOnlySpan<string>)["SingleClass10KTests", "ThousandClass200Tests", "HundredClass2000Tests"])
                {
                    jobs.Add(job.WithNuGet(new NuGetReferenceList(
                    [
                        new NuGetReference("MSTest.TestFramework", version),
                        new NuGetReference("MSTest.TestAdapter", version)
                    ])).WithCustomBuildConfiguration("Debug").WithArguments([new MsBuildArgument($"/p:TestScenario={testScenario}")]));
                }
            }

            AddJob(jobs.ToArray());

            // add the percentage column
            SummaryStyle =
                BenchmarkDotNet.Reports.SummaryStyle.Default
                .WithRatioStyle(BenchmarkDotNet.Columns.RatioStyle.Percentage);
            Options |= ConfigOptions.DisableOptimizationsValidator | ConfigOptions.GenerateMSBuildBinLog;
        }
    }

    [Benchmark]
    public async Task Benchmark()
    {
        await MSTestBench.MicrosoftTestingPlatformEntryPoint.Main([]);
    }
}
