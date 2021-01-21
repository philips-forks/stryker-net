using Buildalyzer;
using Microsoft.Build.Exceptions;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Roslyn;

namespace Stryker.Core.Initialisation
{
    public interface IProjectFileReader
    {
        IAnalyzerResult AnalyzeProject(string projectFilePath, string solutionFilePath);
    }

    public class ProjectFileReader : IProjectFileReader
    {
        private readonly INugetRestoreProcess _nugetRestoreProcess;
        private readonly ILogger _logger;

        public ProjectFileReader(INugetRestoreProcess nugetRestoreProcess = null)
        {
            _nugetRestoreProcess = nugetRestoreProcess ?? new NugetRestoreProcess();
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<ProjectFileReader>();
        }

        public IAnalyzerResult AnalyzeProject(string projectFilePath, string solutionFilePath)
        {
            _logger.LogDebug("Analyzing solution file {0}", solutionFilePath);
            IProjectAnalyzerManager manager;
            try
            {
                manager = new ProjectAnalyzerManager(solutionFilePath, projectFilePath);
            }
            catch (InvalidProjectFileException)
            {
                throw new StrykerInputException($"Incorrect solution path \"{solutionFilePath}\". Solution file not found. Please review your solution path setting.");
            }

            _logger.LogDebug("Analyzing project file {0}", projectFilePath);
            var analyzerResult = manager.Projects[projectFilePath].Build();
            if (!analyzerResult.Succeeded)
            {
                if (!analyzerResult.TargetFramework.Contains("netcoreapp"))
                {
                    // buildalyzer failed to find restored packages, retry after nuget restore
                    _logger.LogDebug("Project analyzer result not successful, restoring packages");
                    _nugetRestoreProcess.RestorePackages(solutionFilePath);
                    analyzerResult = manager.Projects[projectFilePath].Build();
                }
                else
                {
                    // buildalyzer failed, but seems to work anyway.
                    _logger.LogDebug("Project analyzer result not successful");
                }
            }

            return analyzerResult;
        }
    }
}
