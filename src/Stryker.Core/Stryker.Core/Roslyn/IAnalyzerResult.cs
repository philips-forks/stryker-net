using System.Collections.Generic;

namespace Buildalyzer
{
    public interface IAnalyzerResult
    {
        /// <summary>
        /// The full normalized path to the project file.
        /// </summary>
        string ProjectFilePath { get; }

        IEnumerable<string> ProjectReferences { get; }

        IReadOnlyDictionary<string, string> Properties { get; }

        string[] SourceFiles { get; }

        bool Succeeded { get; }

        string TargetFramework { get; }
    }
}
