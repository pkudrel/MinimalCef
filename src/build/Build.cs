using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AbcVersionTool;
using Helpers;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    /// - JetBrains ReSharper        https://nuke.build/resharper
    /// - JetBrains Rider            https://nuke.build/rider
    /// - Microsoft VisualStudio     https://nuke.build/visualstudio
    /// - Microsoft VSCode           https://nuke.build/vscode
    [Parameter("Build counter from outside environment")]
    readonly int BuildCounter;

    readonly DateTime BuildDate = DateTime.UtcNow;


    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [GitRepository] readonly GitRepository GitRepository;

    readonly bool IsAzureDevOps = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AGENT_NAME")) == false;

    [Solution] readonly Solution Solution;



    AbsolutePath ToolsDir => RootDirectory / "tools";
    AbsolutePath DevDir => RootDirectory / "dev";
    AbsolutePath LibzPath => ToolsDir / "LibZ.Tool" / "tools" / "libz.exe";

    AbsolutePath NugetPath => ToolsDir / "nuget.exe";
    AbsolutePath TmpBuild => TemporaryDirectory / "build";
    AbsolutePath ArtifactsDir => RootDirectory / "artifacts";
    AbsolutePath SourceDir => RootDirectory / "src";

    AbcVersion AbcVersion => AbcVersionFactory.Create(BuildCounter, BuildDate);


    ProductInfo ProductInfo => new ProductInfo
    {
        Company = "Deneblab",
        Copyright = $"Deneblab © {DateTime.UtcNow.Year}"
    };

    /// Projects
    Project MinimalCefProject => Solution.GetProject("MinimalCef").NotNull();

    List<ProjectDefinition> Projects => new List<ProjectDefinition>
    {
        new ProjectDefinition
        {
            Name = MinimalCefProject.Name,
            Dir = MinimalCefProject.Name,
            Exe = "MinimalCef.exe",
            DstExe = "MinimalCef.exe",
            AzureContainerName = "application-minimal-cef",
            Project = MinimalCefProject
        }
    };

    Target Information => _ => _
        .Executes(() =>
        {
            var b = AbcVersion;
            Serilog.Log.Debug($"Host: '{Host}'");
            Serilog.Log.Debug($"Version: '{b.SemVersion}'");
            Serilog.Log.Debug($"Date: '{b.DateTime:s}Z'");
            Serilog.Log.Debug($"FullVersion: '{b.InformationalVersion}'");
            Serilog.Log.Debug($"env:Agent.Name: '{Environment.GetEnvironmentVariable("AGENT_NAME")}'");
            Serilog.Log.Debug(
                $"env:Build.ArtifactStagingDirectory: '{Environment.GetEnvironmentVariable("BUILD_ARTIFACTSTAGINGDIRECTORY")}'");
        });


    Target ConfigureAzureDevOps => _ => _
        .DependsOn(Information)
        .OnlyWhenStatic(() => IsAzureDevOps)
        .Executes(() =>
        {
            Serilog.Log.Debug($"Set version to AzureDevOps: {AbcVersion.SemVersion}");
            // https://github.com/microsoft/azure-pipelines-tasks/blob/master/docs/authoring/commands.md
            Serilog.Log.Debug($"##vso[build.updatebuildnumber]{AbcVersion.SemVersion}");
        });

    Target Configure => _ => _
        .DependsOn(ConfigureAzureDevOps);


    Target CheckTools => _ => _
        .DependsOn(Configure)
        .Executes(() =>
        {
            Downloader.DownloadIfNotExists("https://dist.nuget.org/win-x86-commandline/latest/nuget.exe", NugetPath,
                "Nuget");
        });

    Target Clean => _ => _
        .DependsOn(CheckTools)
        .Executes(() =>
        {
            EnsureExistingDirectory(TmpBuild);
            GlobDirectories(TmpBuild, "**/*").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDir);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            using (var process = ProcessTasks.StartProcess(
                NugetPath,
                $"restore  {Solution.Path}",
                SourceDir))
            {
                process.AssertWaitForExit();
               
            }
        });


    Target MakeSyrup => _ => _
        .DependsOn(Restore)
        .Executes(() =>

        {
            var project = Projects.FirstOrDefault(x => x.Project == MinimalCefProject);
            BuildFn(project);
            MargeFn(project);
        });




    Target Ready => _ => _
        .DependsOn(MakeSyrup)
        .Executes(() =>

        {
            var p = Projects.FirstOrDefault(x => x.Project == MinimalCefProject);
            if (p == null) return;
            var tmpMerge = TmpBuild / CommonDir.Merge / p.Dir;
            var tmpReady = TmpBuild / CommonDir.Ready;
            var fileName = $"{p.Name}-{AbcVersion.SemVersion}.exe";
            var fileSrc = tmpMerge / p.Exe;
            var fileDst = tmpReady / fileName;


            EnsureExistingDirectory(tmpReady);
            CopyFile(fileSrc,fileDst, FileExistsPolicy.Overwrite);

            
        });



    





    Target PublishLocalStandalone => _ => _
        .DependsOn(Ready)
        .Executes(() =>
        {
            var p = Projects.FirstOrDefault(x => x.Project == MinimalCefProject);
            if (p == null) return;
            var tmpMerge = TmpBuild / CommonDir.Merge / p.Dir;
            var tmpMergeFile = tmpMerge / p.Exe;
            var devStandalone = DevDir / "app.standolone";
            var devStandaloneFile = devStandalone / p.Exe;
            EnsureExistingDirectory(devStandalone);
            CopyFile(tmpMergeFile, devStandaloneFile, FileExistsPolicy.Overwrite);
        });


    Target Publish => _ => _
        .DependsOn(  PublishLocal);


    Target PublishLocal => _ => _
        .OnlyWhenStatic(() => IsAzureDevOps == false)
        .DependsOn(PublishLocalStandalone);


    
    void BuildFn(ProjectDefinition p)
    {
        var buildOut = TmpBuild / CommonDir.Build / p.Dir;
        var projectFile = p.Project.Path;
        var projectDir = Path.GetDirectoryName(projectFile);
        EnsureExistingDirectory(buildOut);
        Serilog.Log.Debug($"Build; Project file: {projectFile}");
        Serilog.Log.Debug($"Build; Project dir: {projectDir}");
        Serilog.Log.Debug($"Build; Out dir: {buildOut}");
        Serilog.Log.Debug($"Build; Target: {Configuration}");
        Serilog.Log.Debug($"Build; Target: {GitRepository.Branch}");

        AssemblyTools.Patch(projectDir, AbcVersion, p, ProductInfo);

        try
        {
            MSBuild(s => s
                .SetProjectFile(projectFile)
                .SetOutDir(buildOut)
                .SetVerbosity(MSBuildVerbosity.Quiet)
                .SetConfiguration(Configuration)
                .SetTargetPlatform(MSBuildTargetPlatform.x64)
                .SetMaxCpuCount(Environment.ProcessorCount)
                .SetNodeReuse(IsLocalBuild));
        }
        finally
        {
            AssemblyTools.RollbackOriginalFiles(projectDir);
        }
    }

    void MargeFn(ProjectDefinition p)
    {

        var doNotMarge = new[]
        {
            "build.dll", "libcef.dll", "chrome_elf.dll", "d3dcompiler_47.dll",
            "libEGL.dll", "libGLESv2.dll", "CefSharp.dll", "CefSharp.Core.dll",
            "CefSharp.BrowserSubprocess.Core.dll"
        };
        var exclude = string.Join(' ', doNotMarge.Select(x => $"--exclude={x}"));

        var buildOut = TmpBuild / CommonDir.Build / p.Dir;
        var margeOut = TmpBuild / CommonDir.Merge / p.Dir;

        EnsureExistingDirectory(margeOut);
        CopyDirectoryRecursively(buildOut, margeOut, DirectoryExistsPolicy.Merge);

        using (var process = ProcessTasks.StartProcess(
            LibzPath,
            $"inject-dll --assembly {p.Exe} --include *.dll {exclude} --move",
            margeOut))
        {
            process.AssertWaitForExit();
            
        }
    }

    public static int Main() => Execute<Build>(x => x.Publish);
}