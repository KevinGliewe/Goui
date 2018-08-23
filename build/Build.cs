using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.NuGet;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

using GCore.Extensions.StringShEx;
using GCore.Extensions.ArrayEx;

class Build : NukeBuild
{
    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("ApiKey for the specified source.")] readonly string ApiKey;

    string Source => "https://api.nuget.org/v3/index.json";

    string Branch => GitRepository.Branch;
    string ChangelogFile => RootDirectory / "CHANGELOG.md";


    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    String GitVersion = "1.0.0";
    string GitVersionSuffix = "0";

    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    public Build() {
        var version = "git tag".Sh().Split('\n').Get(-2).ExtractVersion();
        var suffix = "git rev-list --count HEAD".Sh().Replace("\n", "").Trim();

        GitVersion = version.ToString(3);
        GitVersionSuffix = GitVersion + "." + suffix;
    }

    Target Clean => _ => _
        .Executes(() =>
        {
            DeleteDirectories(GlobDirectories(TestsDirectory, "**/bin", "**/obj"));
            EnsureCleanDirectory(ArtifactsDirectory);
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

    private Target Pack => _ => _
        .DependsOn(Compile)
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
            GlobFiles(ArtifactsDirectory, "Goui.*.nupkg").NotEmpty()
                .Where(x => !x.EndsWith(".symbols.nupkg"))
                .ToList()
                .ForEach(x => DotNetNuGetPush(s => s
                    .SetTargetPath(x)
                    .SetSource(Source)
                    .SetApiKey(ApiKey)));
        });
}
