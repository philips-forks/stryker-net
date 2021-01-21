using System.Collections.Generic;
using Buildalyzer;

namespace Stryker.Core.Roslyn
{
    public class AnalyzerResult : IAnalyzerResult
    {
        private readonly string _projectFilePath;
        private readonly IEnumerable<string> _references;
        private readonly IReadOnlyDictionary<string, string> _properties;
        private readonly string[] _sourceFiles;
        private readonly bool _succeeded;
        private readonly string _targetFramework;


        public string ProjectFilePath { get { return _projectFilePath; } }

        public IEnumerable<string> ProjectReferences { get { return _references; } }

        public IReadOnlyDictionary<string, string> Properties { get { return _properties; } }

        public string[] SourceFiles { get { return _sourceFiles; } }

        public bool Succeeded { get { return _succeeded; } }

        public string TargetFramework { get { return _targetFramework; } }

        public AnalyzerResult(string projectFilePath, IEnumerable<string> references, IReadOnlyDictionary<string, string> properties, string[] sourceFiles, bool succeeded, string targetFramework)
        {
            _projectFilePath = projectFilePath;
            _references = references;
            _properties = properties;
            _sourceFiles = sourceFiles;
            _succeeded = succeeded;
            _targetFramework = targetFramework;
        }
    }
}
