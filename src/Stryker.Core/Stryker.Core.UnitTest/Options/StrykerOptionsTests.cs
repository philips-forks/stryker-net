﻿using Microsoft.CodeAnalysis.Text;
using Serilog.Events;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class StrykerOptionsTests
    {
        [Fact]
        public void ShouldContainCorrectDefaults()
        {
            var options = new StrykerProjectOptions();

            options.DashboardUrl.ShouldBe("https://dashboard.stryker-mutator.io");
        }

        [Theory]
        [InlineData("error", LogEventLevel.Error)]
        [InlineData("", LogEventLevel.Information)]
        [InlineData(null, LogEventLevel.Information)]
        [InlineData("warning", LogEventLevel.Warning)]
        [InlineData("info", LogEventLevel.Information)]
        [InlineData("debug", LogEventLevel.Debug)]
        [InlineData("trace", LogEventLevel.Verbose)]
        public void Constructor_WithCorrectLoglevelArgument_ShouldAssignCorrectLogLevel(string argValue, LogEventLevel expectedLogLevel)
        {
            var options = new StrykerProjectOptions(logLevel: argValue);

            options.LogOptions.ShouldNotBeNull();
            options.LogOptions.LogLevel.ShouldBe(expectedLogLevel);
        }

        [Fact]
        public void Constructor_WithIncorrectLoglevelArgument_ShouldThrowStrykerInputException()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerProjectOptions(logLevel: logLevel);
            });

            ex.Message.ShouldBe("The value for one of your settings is not correct. Try correcting or removing them.");
        }

        [Fact]
        public void Constructor_WithIncorrectSettings_ShoudThrowWithDetails()
        {
            var logLevel = "incorrect";

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerProjectOptions(logLevel: logLevel);
            });

            ex.Details.ShouldNotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("./MyFolder/MyFile.cs", "MyFolder/MyFile.cs")]
        [InlineData("./MyFolder", "MyFolder/*.*")]
        [InlineData("C:/MyFolder/MyFile.cs", "C:/MyFolder/MyFile.cs")]
        public void FilesToExclude_should_be_converted_to_file_patterns(string fileToExclude, string expectedFilePattern)
        {
            // Act
            var result = new StrykerProjectOptions(filesToExclude: new[] { fileToExclude });

            // Assert
            var pattern = result.FilePatterns.Last();
            Path.GetFullPath(pattern.Glob.ToString()).ShouldBe(Path.GetFullPath(expectedFilePattern));
            pattern.TextSpans.ShouldContain(TextSpan.FromBounds(0, int.MaxValue));
            pattern.IsExclude.ShouldBeTrue();
        }

        [Fact]
        public void ShouldValidateApiKey()
        {
            const string strykerDashboardApiKey = "STRYKER_DASHBOARD_API_KEY";
            var key = Environment.GetEnvironmentVariable(strykerDashboardApiKey);
            try
            {
                var options = new StrykerProjectOptions();
                Environment.SetEnvironmentVariable(strykerDashboardApiKey, string.Empty);

                var ex = Assert.Throws<StrykerInputException>(() =>
                {
                    new StrykerProjectOptions(reporters: new string[] { "Dashboard" });
                });
                ex.Message.ShouldContain($"An API key is required when the {Reporter.Dashboard} reporter is turned on! You can get an API key at {options.DashboardUrl}");
                ex.Message.ShouldContain($"A project name is required when the {Reporter.Dashboard} reporter is turned on!");
            }
            finally
            {
                Environment.SetEnvironmentVariable(strykerDashboardApiKey, key);
            }
        }

        [Fact]
        public void ShouldValidateGitSource()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerProjectOptions(gitSource: "");
            });
            ex.Message.ShouldBe("GitSource may not be empty, please provide a valid git branch name");
        }

        [Fact]
        public void ShouldValidateOptimisationMode()
        {
            var options = new StrykerProjectOptions(coverageAnalysis: "perTestInIsolation");
            options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest).ShouldBeTrue();
            options.Optimizations.HasFlag(OptimizationFlags.CaptureCoveragePerTest).ShouldBeTrue();

            options = new StrykerProjectOptions();
            options.Optimizations.HasFlag(OptimizationFlags.CoverageBasedTest).ShouldBeTrue();

            options = new StrykerProjectOptions(coverageAnalysis: "all");
            options.Optimizations.HasFlag(OptimizationFlags.SkipUncoveredMutants).ShouldBeTrue();

            options = new StrykerProjectOptions(coverageAnalysis: "off");
            options.Optimizations.HasFlag(OptimizationFlags.NoOptimization).ShouldBeTrue();

            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new StrykerProjectOptions(coverageAnalysis: "gibberish");
            });
            ex.Details.ShouldBe($"Incorrect coverageAnalysis option gibberish. The options are [off, all, perTest or perTestInIsolation].");
        }

        [Theory]
        [InlineData(101, "The thresholds must be between 0 and 100")]
        [InlineData(1000, "The thresholds must be between 0 and 100")]
        [InlineData(-1, "The thresholds must be between 0 and 100")]
        [InlineData(59, "The values of your thresholds are incorrect. Change `--threshold-break` to the lowest value and `--threshold-high` to the highest.")]
        public void ShouldValidateThresholdsIncorrect(int thresholdHigh, string message)
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerProjectOptions(thresholdHigh: thresholdHigh, thresholdLow: 60, thresholdBreak: 60);
            });
            ex.Details.ShouldBe(message);
        }

        [Fact]
        public void ShouldValidateThresholds()
        {
            var options = new StrykerProjectOptions(thresholdHigh: 60, thresholdLow: 60, thresholdBreak: 50);
            options.Thresholds.High.ShouldBe(60);
            options.Thresholds.Low.ShouldBe(60);
            options.Thresholds.Break.ShouldBe(50);
        }

        [Fact]
        public void ShouldValidateTestRunner()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                var options = new StrykerProjectOptions(testRunner: "gibberish");
            });
            ex.Details.ShouldBe($"The given test runner (gibberish) is invalid. Valid options are: [{string.Join(",", Enum.GetValues(typeof(TestRunner)))}]");
        }
    }
}
