using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Construction;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Roslyn
{
    public class ProjectAnalyzerManager : IProjectAnalyzerManager
    {
        internal static readonly SolutionProjectType[] SupportedProjectTypes = new SolutionProjectType[]
        {
            SolutionProjectType.KnownToBeMSBuildFormat,
            SolutionProjectType.WebProject
        };

        private readonly ConcurrentDictionary<string, IProjectAnalyzer> _projects = new ConcurrentDictionary<string, IProjectAnalyzer>();

        public IReadOnlyDictionary<string, IProjectAnalyzer> Projects => _projects;

        public string SolutionFilePath { get; }

        public SolutionFile SolutionFile { get; }

        public ProjectAnalyzerManager(string solutionFilePath, string projectFilePath)
        {
            if (string.IsNullOrWhiteSpace(solutionFilePath))
            {
                if (!File.Exists(projectFilePath))
                {
                    throw new StrykerInputException($"Incorrect project file path \"{projectFilePath}\". Project file not found. Please review your path setting.");
                }
                IProjectAnalyzer projectAnalyzer = new ProjectAnalyzer(projectFilePath);
                _projects.TryAdd(projectFilePath, projectAnalyzer);
            }
            else
            {
                if (!File.Exists(solutionFilePath))
                {
                    throw new StrykerInputException($"Incorrect solution path \"{solutionFilePath}\". Solution file not found. Please review your solution path setting.");
                }

                SolutionFile = SolutionFile.Parse(SolutionFilePath);

                // Initialize all the projects in the solution
                foreach (ProjectInSolution projectInSolution in SolutionFile.ProjectsInOrder)
                {
                    if (!SupportedProjectTypes.Contains(projectInSolution.ProjectType))
                    {
                        continue;
                    }
                    if (!File.Exists(projectInSolution.AbsolutePath))
                    {
                        continue;
                    }
                    if (!string.IsNullOrWhiteSpace(projectFilePath) && projectFilePath != projectInSolution.AbsolutePath)
                    {
                        continue;
                    }
                    IProjectAnalyzer projectAnalyzer = new ProjectAnalyzer(projectInSolution);
                    _projects.TryAdd(projectInSolution.AbsolutePath, projectAnalyzer);
                }
            }
        }

        //        public IProjectAnalyzer GetProject(string projectFilePath) => Projects[projectFilePath];
        //        public IProjectAnalyzer GetProject(string projectFilePath) => GetProject(projectFilePath, null);
    }
}
