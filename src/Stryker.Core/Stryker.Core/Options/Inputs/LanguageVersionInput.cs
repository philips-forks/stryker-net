﻿using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Exceptions;
using System;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    public class LanguageVersionInput : ComplexStrykerInput<string, LanguageVersion>
    {
        public override StrykerInput Type => StrykerInput.LanguageVersion;
        public override string DefaultInput => "latest";
        public override LanguageVersion DefaultValue => new LanguageVersionInput(DefaultInput).Value;

        protected override string Description => $"The c# version used in compilation.";
        protected override string HelpOptions => FormatHelpOptions(DefaultInput, Enum.GetNames(DefaultValue.GetType()).Where(l => LanguageVersion.CSharp1.ToString() != l));


        public LanguageVersionInput() { }

        public LanguageVersionInput(string languageVersion)
        {
            if (languageVersion is { })
            {
                if (Enum.TryParse(languageVersion, true, out LanguageVersion result) && result != LanguageVersion.CSharp1)
                {
                    Value = result;
                }
                else
                {
                    throw new StrykerInputException($"The given c# language version ({languageVersion}) is invalid.");
                }
            }
        }
    }
}