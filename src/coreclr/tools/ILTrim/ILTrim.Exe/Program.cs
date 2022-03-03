// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;

namespace ILTrim
{
    public class Program
    {
        private static readonly Command rootCommand = new RootCommand("ILTrim is a tool to trim unused IL from assemblies.");
        private static readonly Option<string[]> referencesOption = new Option<string[]>(new[]{"-r", "--reference"}, "Reference assemblies");
        private static readonly Option<string[]> trimAssembliesOption = new Option<string[]>(new[]{"-t", "--trim"}, "Trim assemblies");
        private static readonly Option<string> outputPathOption = new Option<string>(new[]{"-o", "--out"}, "Output path");
        private static readonly Option<LogStrategy> logStrategyOption = new Option<LogStrategy>(new[]{"-l", "--log"}, "Logging strategy");
        private static readonly Option<string> logFilePathOption = new Option<string>("--logfile", "Path to the log file");
        private static readonly Option<int?> parallelismOption = new Option<int?>("--parallelism", "Parallelism");
        private static readonly Option<bool> libraryModeOption = new Option<bool>("--library", "Library mode");
        private static readonly ParseArgument<KeyValuePair<string, bool>[]> parseFeatures = (ArgumentResult args) => {
            KeyValuePair<string, bool> ParseToken(Token t) {
                int sep = t.Value.IndexOf('=');
                if (sep == -1)
                    throw new ArgumentException("The format of --feature value is <featureswitch>=<value>");

                string fsName = t.Value.Substring(0, sep);
                string fsValue = t.Value.Substring(sep + 1);
                return new KeyValuePair<string, bool>(fsName, bool.Parse(fsValue));
            }

            return args.Tokens.Select(ParseToken).ToArray();
        };
        private static readonly Option<KeyValuePair<string, bool>[]> features = new Option<KeyValuePair<string, bool>[]>("--feature", parseArgument: parseFeatures, description: "Feature switches");
        private static readonly Argument<string> inputArg = new Argument<string>("input", "Assembly to trim");

        static void Main(string[] args)
        {
            rootCommand.AddOption(referencesOption);
            rootCommand.AddOption(trimAssembliesOption);
            rootCommand.AddOption(outputPathOption);
            rootCommand.AddOption(logStrategyOption);
            rootCommand.AddOption(logFilePathOption);
            rootCommand.AddOption(parallelismOption);
            rootCommand.AddOption(libraryModeOption);
            rootCommand.AddOption(features);
            rootCommand.AddValidator((CommandResult v) => {
                var _ = (v.GetValueForOption(logStrategyOption), v.GetValueForOption(logFilePathOption)) switch {
                    (LogStrategy.FullGraph, null) =>
                        v.ErrorMessage = "Log strategy reqires a log file path",
                    (LogStrategy.FirstMark, null) =>
                        v.ErrorMessage = "Log strategy reqires a log file path",
                    (_, { } logFile) =>
                        v.ErrorMessage = "Specified log strategy can't use logFile option",
                };
            });
            rootCommand.AddArgument(inputArg);
            rootCommand.SetHandler((KeyValuePair<string, bool>[] featureSwitches, int? parallelism, LogStrategy logStrategy, string logFile, bool libraryMode, string input, string[] trimAssemblies, string outputPath, string[] references) => {
                Dictionary<string, bool> featureSwitchesDictionary = new(featureSwitches ?? Array.Empty<KeyValuePair<string, bool>>());
                var settings = new TrimmerSettings(
                    MaxDegreeOfParallelism: parallelism,
                    LogStrategy: logStrategy,
                    LogFile: logFile,
                    LibraryMode: libraryMode,
                    FeatureSwitches: featureSwitchesDictionary);
                Trimmer.TrimAssembly(
                    input.Trim(),
                    trimAssemblies,
                    outputPath ?? Directory.GetCurrentDirectory(),
                    references,
                    settings);
            }, features, parallelismOption, logStrategyOption, logFilePathOption, libraryModeOption, inputArg, trimAssembliesOption, outputPathOption, referencesOption);

            new CommandLineBuilder(rootCommand).UseDefaults().Build().Invoke(args);
        }
    }
}
