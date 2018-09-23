using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;

using GCore.Logging;
using GCore.Extensions.StringShEx;
using GCore.Extensions.ArrayEx;
using Nuke.Common.Tooling;
using System.IO;

class Build : NukeBuild
{
    public static int Main () {
        Log.LoggingHandler.Add(new GCore.Logging.Logger.ConsoleLogger());
        return Execute<Build>(x => x.Compile);
    }

    [Parameter("ApiKey for the specified source.")] readonly string ApiKey;
    [Parameter("Defines the Github user account to push the Docs.")] readonly string GithubUsername;
    [Parameter("Defines the Github token to push the Docs.")] readonly string GithubToken;
    [Parameter("CommitFlags")] public string CommitFlags = "";
    [Parameter("DocFx tool command")] public string[] DocFxTool= new string[] { "docfx" };


    string Source => "https://api.nuget.org/v3/index.json";

    string Branch => GitRepository.Branch;
    string ChangelogFile => RootDirectory / "CHANGELOG.md";


    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    String GitVersion = "1.0.1";
    string GitVersionSuffix = "0";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath DocDirectory => RootDirectory / "doc";
    AbsolutePath DocBuildDirectory => DocDirectory / "build";
    AbsolutePath DocFxDirectory => DocDirectory / "docfx";
    AbsolutePath TempDir => (AbsolutePath)System.IO.Path.GetTempPath();
    AbsolutePath GhPagesDir => TempDir / "Goui";
    AbsolutePath WasmTest => RootDirectory / "PlatformSamples" / "WasmFormsApp" / "bin" / "Debug" / "netcoreapp2.0" / "dist";
    AbsolutePath WasmTestRelease => RootDirectory / "PlatformSamples" / "WasmFormsApp" / "bin" / "Release" / "netcoreapp2.0" / "dist";



    Regex CommentFlagsRex = new Regex(@"^.*\|\[([A-Z]+)\]\|$");
    public Build() {
        var version = "git tag".Sh().Split('\n').Get(-2).ExtractVersion();
        var suffix = "git rev-list --count HEAD".Sh().Replace("\n", "").Trim();

        if (CommitFlags.Length == 0) {
            var comment = "git log -1 --pretty=%B".Sh().Replace("\n", "").Trim();
            var match = CommentFlagsRex.Match(comment);
            if (match.Success)
                CommitFlags = match.Groups[1].Value;
        }
        
        Logger.Info($"Using version tag : '{version}'");
        Logger.Info($"Using rev suffix  : '{suffix}'");
        Logger.Info($"Using commit flags: '{CommitFlags}'");

        GitVersion = version.ToString(3);
        GitVersionSuffix = GitVersion + "." + suffix;

        Logger.Info($"Using Version     : {GitVersionSuffix}");
    }

    void Ex(IProcess process) {
        process.WaitForExit();
        if (process.ExitCode != 0)
            throw new Exception(process.FileName + " failed");
    }

    void DocFx(string command = "") {
        Ex(ProcessTasks.StartProcess(DocFxTool[0], DocFxTool.Length > 1 ? (string)(RootDirectory / DocFxTool[1]) + " " + command : command, DocFxDirectory));
    }

    Target Clean => _ => _
        .Executes(() =>
        {
            EnsureCleanDirectory(ArtifactsDirectory);
            EnsureCleanDirectory(DocBuildDirectory);
            EnsureCleanDirectory(GhPagesDir);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(SolutionFile));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(SolutionFile)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersionSuffix)
                .SetFileVersion(GitVersionSuffix)
                .SetInformationalVersion(GitVersionSuffix)
                .EnableNoRestore());
        });

    Target Doc => _ => _
        .DependsOn(Compile)
        .Executes(() => {
            DocFx();
            DocFx("build");
        });

    private Target Pack => _ => _
        .DependsOn(Compile)
        .DependsOn(Doc)
        .Executes(() =>
        {
            var version = GitVersionSuffix;

            DotNetPack(s => s
                .SetProject(SolutionFile)
                .SetVersion(version)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetConfiguration(Configuration)
                .EnableNoBuild()
                .EnableIncludeSymbols());
            
            var basePath = RootDirectory / "Goui.Wasm";
            
            NuGetTasks.NuGetPack(
                basePath / "Goui.Wasm.nuspec",
                s => s
                    .SetBasePath(basePath)
                    .SetVersion(version)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetConfiguration(Configuration)
                
            );
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Requires(() => ApiKey)
        .Executes(() =>
        {
            // Nuget deploy
            if (CommitFlags.Contains('N')) {
                GlobFiles(ArtifactsDirectory, "Goui.*.nupkg").NotEmpty()
                    .Where(x => !x.EndsWith(".symbols.nupkg"))
                    .ToList()
                    .ForEach(x => DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(Source)
                        .SetApiKey(ApiKey)));
            } else Logger.Warn("Skipping push to Nuget");
           
        });

    Target GhPages => _ => _
        .DependsOn(Doc)
        .Requires(() => GithubUsername)
        .Requires(() => GithubToken)
        .Executes(() => {
            Git($"clone -b gh-pages --single-branch https://{GithubUsername}:{GithubToken}@github.com/KevinGliewe/Goui --depth 1 {GhPagesDir}",
                TempDir);

            Logger.Info($"Exists DocBuildDirectory '{DocBuildDirectory}': {Directory.Exists(DocBuildDirectory)}");
            Logger.Info($"Exists GhPagesDir '{GhPagesDir}': {Directory.Exists(GhPagesDir)}");
            Logger.Info($"Exists WasmTest '{WasmTest}': {Directory.Exists(WasmTest)}");
            Logger.Info($"Exists WasmTestRelease '{WasmTestRelease}': {Directory.Exists(WasmTestRelease)}");

            CopyDirectoryRecursively(DocBuildDirectory, GhPagesDir, FileExistsPolicy.Overwrite);
            CopyDirectoryRecursively(WasmTestRelease, GhPagesDir / "WasmFormsApp", FileExistsPolicy.Overwrite);
            Git("add .", GhPagesDir);
            Git($"commit -m \"{GitVersionSuffix}\"", GhPagesDir);

            // Github pages commit
            if (CommitFlags.Contains('P')) {
                Git("push origin gh-pages", GhPagesDir);
            } else Logger.Warn("Skipping push to Github-Pages");
        });

    Target All => _ => _
        .DependsOn(Publish)
        .DependsOn(GhPages);
}
