using Buildalyzer;

namespace Stryker.Core.Roslyn
{
    public interface IProjectAnalyzer
    {
        string ProjectFilePath { get; }
        IAnalyzerResult Build();
    }
}
