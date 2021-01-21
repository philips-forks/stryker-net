using System.Collections.Generic;

namespace Stryker.Core.Roslyn
{
    public interface IProjectAnalyzerManager
    {
        //        ILoggerFactory LoggerFactory { get; set; }

        IReadOnlyDictionary<string, IProjectAnalyzer> Projects { get; }

        //        SolutionFile SolutionFile { get; }

        //        string SolutionFilePath { get; }

        /// <summary>
        /// Analyzes an MSBuild binary log file.
        /// </summary>
        /// <param name="binLogPath">The path to the binary log file.</param>
        /// <param name="buildLoggers">MSBuild loggers to replay events from the log to.</param>
        /// <returns>A dictionary of target frameworks to <see cref="AnalyzerResult"/>.</returns>
        //        IAnalyzerResults Analyze(string binLogPath, IEnumerable<Microsoft.Build.Framework.ILogger> buildLoggers = null);

        //IProjectAnalyzer GetProject(string projectFilePath);

        //        void RemoveGlobalProperty(string key);

        //        void SetEnvironmentVariable(string key, string value);

        //        void SetGlobalProperty(string key, string value);
    }
}
