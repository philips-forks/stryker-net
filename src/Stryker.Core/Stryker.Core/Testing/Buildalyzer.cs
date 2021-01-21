using System.Diagnostics.CodeAnalysis;
using Stryker.Core.Roslyn;

namespace Stryker.Core.Testing
{
    /// <summary>
    /// This is an interface to mock buildalyzer classes
    /// </summary>
    public interface IProjectAnalyzerManagerProvider
    {
        IProjectAnalyzerManager Provide(string solutionFilePath);
    }

    [ExcludeFromCodeCoverage]
    public class ProjectAnalyzerManagerProvider : IProjectAnalyzerManagerProvider
    {
        public IProjectAnalyzerManager Provide(string solutionFilePath)
        {
            return new ProjectAnalyzerManager(solutionFilePath, string.Empty);
        }
    }
}
