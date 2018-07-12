using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
#if UNITY_5_6_OR_NEWER
using UnityEditor.Build;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif
#endif

using Debug = UnityEngine.Debug;

public static class MoPubSDKBuild
{
    #region OS Commands

    // Helper function to run an external command and maybe capture the output of stdout.  The command name in exe will
    // be path-searched if necessary.  The command is run with the current directory set to 'dir' and the environment
    // augmented with any values passed in vars[] (each must be of the form "VAR=VALUE").
    private static bool RunExe(StringBuilder stdout, string exe, string args, string dir, string[] vars)
    {
#if UNITY_EDITOR_WIN
        // Convenience since lots of exes on Windows end with ".exe" when the corresponding exe on Mac ends with nothing.
        if (string.IsNullOrEmpty(Path.GetExtension(exe)))
            exe += ".exe";
#endif

        var proc = new Process { StartInfo = new ProcessStartInfo {
            FileName = exe,
            Arguments = args ?? string.Empty,
            WorkingDirectory = dir ?? string.Empty,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = stdout != null,
            RedirectStandardError = true,
        }};
        foreach (var vv in vars.Select(v => v.Split('='))) {
            proc.StartInfo.EnvironmentVariables.Add(vv[0], vv[1]);
        }

        if (stdout != null)
            proc.OutputDataReceived += (sender, arg) => stdout.AppendLine(arg.Data);
        var stderr = new StringBuilder();
        proc.ErrorDataReceived += (sender, arg) => stderr.AppendLine(arg.Data);

        try {
            proc.Start();
            if (stdout != null)
                proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            proc.WaitForExit();
            if (proc.ExitCode != 0) {
                Debug.LogErrorFormat("Error exit while running '{0} {1}': {2}\n{3}", exe, args, proc.ExitCode, stderr);
                return false;
            }
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while running '{0} {1}': {2}", exe, args, e);
            return false;
        }
        return true;
    }


    // Helper wrapper function for RunExe for when you are capturing output of stdout.  Returns the output as a string.
    // If the command succeeded (exit code 0), this will never be null.  Otherwise (exit code != 0) it will be null.
    private static string RunForOutput(string exe, string args = null, string dir = null, params string[] vars)
    {
        var stdout = new StringBuilder();
        return RunExe(stdout, exe, args, dir, vars) ? stdout.ToString() : null;
    }


    // Helper wrapper function for RunExe for when you don't want to capture stdout, only the success/failure of the
    // command.  This returns true if the command's exit code was 0, otherwise false.
    private static bool Run(string exe, string args = null, string dir = null, params string[] vars)
    {
        return RunExe(null, exe, args, dir, vars);
    }


    // Helper function to create dest directory, used in other functions below.  Normal usage is to call
    // MkDir(Path.GetDirectory(destFilePath)), to ensure that the file-to-be-created has a directory in which to be
    // created. Hence no action is taken for an empty path, since that's what you get when the file is going into the
    // current working directory.
    private static void Mkdir(string path)
    {
        if (!string.IsNullOrEmpty(path))
            Directory.CreateDirectory(path);
    }


    // Simple analog of the Linux/BSD "cp" command.
    internal static bool Cp(string source, string dest)
    {
        try {
            Mkdir(Path.GetDirectoryName(dest));
            File.Copy(source, dest, overwrite:true);
            return true;
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while copying file '{0}' to '{1}': {2}", source, dest, e);
            return false;
        }
    }


    // Simple analog of the Linux/BSD "mv" command.
    private static bool Mv(string source, string dest)
    {
        try {
            Mkdir(Path.GetDirectoryName(dest));
            File.Move(source, dest);
            return true;
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while renaming file '{0}' to '{1}': {2}", source, dest, e);
            return false;
        }
    }


    // Simple analog of the Linux/BSD "rm" command.
    internal static bool Rm(string path)
    {
        try {
            File.Delete(path);
            return true;
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while deleting file '{0}': {1}", path, e);
            return false;
        }
    }


    // Simple analog of the Linux/BSD "rmdir" command.
    private static bool RmDir(string path)
    {
        try {
            Directory.Delete(path);
            return true;
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while deleting directory '{0}': {1}", path, e);
            return false;
        }
    }


    // Simple analog of the Linux/BSD "sed" command, using the -i flag to edit a file in place.
    private static bool Sed(string file, string regex, string repl)
    {
        try {
            var text = File.ReadAllText(file);
            text = Regex.Replace(text, regex, repl, RegexOptions.Multiline);
            File.WriteAllText(file, text);
            return true;
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while rewriting file {0}: {1}", file, e);
            return false;
        }
    }


    // MSDN docs say that Directory.GetFileSystemEntries() returns the "names" of the directory's contents,
    // but we're getting back full paths.  This wrapper works around that because we need the simple names.
    // Both overloads of Directory.GetFileSystemEntries() -- with or without a search pattern -- are supported.
    private static IEnumerable<string> GetFileSystemEntries(string dir, string pattern = null)
    {
        try {
            return string.IsNullOrEmpty(pattern)
                ? from name in Directory.GetFileSystemEntries(dir) select Path.GetFileName(name)
                : from name in Directory.GetFileSystemEntries(dir, pattern) select Path.GetFileName(name);
        } catch (Exception e) {
            Debug.LogErrorFormat("Exception while finding files in dir {0}: {1}", dir, e);
            return Enumerable.Empty<string>();
        }
    }


    // Simple analog of the Linux/BSD "find" command.  It is equivalent to just "find dir" with flags "-type f"
    // and/or "-type d" added.  However the returned paths are relative to dir, unlike the find command.
    private static IEnumerable<string> Find(string dir, bool files = true, bool dirs = true)
    {
        if (!Directory.Exists(dir))
            yield break;
        foreach (var name in GetFileSystemEntries(dir)) {
            var path = Path.Combine(dir, name);
            if (File.Exists(path)) {
                if (files)
                    yield return name;
            } else if (Directory.Exists(path)) {
                if (dirs)
                    yield return name;
                foreach (var subPath in Find(path, files, dirs))
                    yield return Path.Combine(name, subPath);
            }
        }
    }


    // Simple analog of "rsync" command.  Note that each entry in names[] can contain a wildcard, like '*.png'.
    // If it refers to a directory (e.g., '*.framework' on a Mac) the whole directory is copied into the destination
    // recursively, and any files present in the destination that don't correspond to files in the source are deleted.
    // (.meta files are excepted for Unity friendliness).  In effect we are doing this:
    //    rsync -r --delete --exclude='*.meta' srcDir/DIR destDir/
    private static bool Rsync(string srcDir, string destDir, params string[] names)
    {
        // Expand wildcards.
        names = names.SelectMany(name => GetFileSystemEntries(srcDir, name)).ToArray();
        if (names.Length == 0) {
            Debug.LogErrorFormat("No matching files found in dir {0}", srcDir);
            return false;
        }

        foreach (var name in names) {
            var srcPath = Path.Combine(srcDir, name);
            var destPath = Path.Combine(destDir, name);
            if (File.Exists(srcPath)) {
                // This means: if there's a directory at the destination path where the src file wants to be, first try
                // to delete it (unlikely but has been known to occur).  Then copy the file over.  Stop if any of these
                // steps fail.
                if (Directory.Exists(destPath) && !RmDir(destPath)  // Unlikely, but has been known to occur...
                    || !Cp(srcPath, destPath))
                    return false;
            } else if (Directory.Exists(srcPath)) {
                // Look for stale paths in the destination directory (left over from a prior build, but some files have
                // since been renamed or removed).  Unity .meta files are handled along with their associated asset
                // file, rather than separately.  (Otherwise they'd all get deleted every time we build, and Unity would
                // generate all new ones with nothing but a different timestamp, which clogs up git diffs pointlessly.)
                var staleDestRms = from subPath in Find(destPath)
                                   where !subPath.EndsWith(".meta")
                                   let src = Path.Combine(srcPath, subPath)
                                   let dest = Path.Combine(destPath, subPath)
                                   let meta = dest + ".meta"
                                   select File.Exists(dest)
                                          ? File.Exists(src) || Rm(dest) && Rm(meta)
                                          : Directory.Exists(src) || RmDir(dest) && Rm(meta);
                // Copy all files in the src directory tree to the destination.
                var copySrcFiles = from subPath in Find(srcPath, dirs:false)
                                   let src = Path.Combine(srcPath, subPath)
                                   let dest = Path.Combine(destPath, subPath)
                                   select Cp(src, dest);
                // Fail if any of the above operations fails.
                if (staleDestRms.Contains(false) || copySrcFiles.Contains(false))
                    return false;
            }
        }
        return true;
    }


    #endregion

    #region Platform Specific Values


    // Returns the full (absolute) path of a subdirectory of the MoPub unity repo.  Since the unity-sample-app subdir is
    // where Unity is running, we have to include a ".." to adjust for that.
    private static string GetFullPath(string subdir)
    {
        return Path.GetFullPath(Path.Combine("..", subdir));
    }


    // The path to the platform MoPub SDK (Android or iOS, internal or public subrepo).
    private static string GetSdkSubdir(BuildTarget buildTarget, bool internalSdkBuild)
    {
        var sdkDir = "mopub";
        switch (buildTarget) {
            case BuildTarget.Android:
                sdkDir += "-android";
                break;
            case BuildTarget.iOS:
                sdkDir += "-ios";
                break;
            default:
                Debug.LogError("Invalid build target: " + buildTarget);
                return null;
        }
        if (!internalSdkBuild)
            sdkDir += "-sdk";
        return GetFullPath(sdkDir);
    }


    // The file within the platform MoPub SDK that contains the SDK's version string constant.
    private static readonly Dictionary<BuildTarget, string> FileWithVersionNumber = new Dictionary<BuildTarget, string>
    {
        {BuildTarget.Android, "mopub-sdk/mopub-sdk-base/src/main/java/com/mopub/common/MoPub.java"},
        {BuildTarget.iOS, "MoPubSDK/MPConstants.h"}
    };


    // The suffix to add to the platform MoPub SDK's version string.  For public builds this is simply "unity".  For
    // internal development builds, it is the git commit (short) SHA that we are building against.
    private static string GetSdkVersionSuffix(BuildTarget buildTarget, bool internalSdkBuild)
    {
        if (!internalSdkBuild)
            return "unity";
        // cd to appropriate git submodule directory and get the (shortened) SHA of the latest commit
        var dir = GetSdkSubdir(buildTarget, internalSdkBuild);
        var githash = RunForOutput("git", "rev-parse --short HEAD", dir) ?? "unknown";
        return githash.Trim();
    }


    // A regex that matches the part of the version string occupied by our Unity suffix.  Uses lookbehind and lookahead
    // anchors to locate the spot.  The resulting matched substring will be zero-length when no suffix is already in place
    // (the usual case on a fresh checkout).  But that's fine since the Regex.Replace() function knows where in the string
    // the match happened regardless.
    private static readonly Dictionary<BuildTarget, string> RegexForVersionLine = new Dictionary<BuildTarget, string>
    {
        { BuildTarget.Android, @"(?<=public static final String SDK_VERSION[^""]*""[^+""]*)(\+[^""]*)?(?="")" },
        { BuildTarget.iOS,     @"(?<=#define MP_SDK_VERSION[^""]*""[^+""]*)(\+[^""]*)?(?="")" }
    };


    #endregion

    #region Build Steps


    // Each platform SDK has a file that contains a version string constant which identifies the current build.
    // We append a suffix to this version string to identify that the platform SDK is being used from Unity rather
    // than standalone.
    private static bool AppendUnityVersion(BuildTarget buildTarget, bool internalSdkBuild)
    {
        Debug.Log("Appending Unity marker to MoPub platform SDK version string");
        var file = Path.Combine(GetSdkSubdir(buildTarget, internalSdkBuild), FileWithVersionNumber[buildTarget]);
        var regex = RegexForVersionLine[buildTarget];
        var repl = "+" + GetSdkVersionSuffix(buildTarget, internalSdkBuild);
        return Sed(file, regex, repl);
    }


    // Build the platform MoPub SDK using its native build system.
    private static bool BuildPlatformSdk(BuildTarget buildTarget, bool internalSdkBuild, bool debug)
    {
        Debug.LogFormat("Building MoPub platform SDK for {0} (internal: {1}, debug: {2})", buildTarget, internalSdkBuild, debug);
        switch (buildTarget) {
            case BuildTarget.Android:
#if UNITY_EDITOR_OSX
                const string cmd = "bash";
                const string gradlew = "gradlew";
#elif UNITY_EDITOR_WIN
                const string cmd = "cmd";
                const string gradlew = "/c gradlew.bat";
#endif
                var sdkDir = GetSdkSubdir(buildTarget, internalSdkBuild);
                return Run(cmd, string.Format("{0} clean assemble{1}", gradlew, debug ? "Debug" : "Release"),
                           GetFullPath("mopub-android-sdk-unity"),
                           "SDK_DIR=" + Path.GetFileName(sdkDir),
                           "ANDROID_HOME=" + EditorPrefs.GetString("AndroidSdkRoot"));
#if UNITY_EDITOR_OSX
            case BuildTarget.iOS:
                var project = "mopub-ios-sdk-unity.xcodeproj";
                if (internalSdkBuild)
                    project = "internal-" + project;
                var jsfile = GetFullPath("mopub-ios-sdk-unity/bin/MoPubSDKFramework.framework/MRAID.bundle/mraid.js");
                return Run("xcrun",
                           "xcodebuild"
                           + " -project " + project
                           + " -scheme \"MoPub for Unity\""
                           + " -configuration \"" + (debug ? "Debug" : "Release") + "\""
                           + " OTHER_CFLAGS=\"-fembed-bitcode -w\""
                           + " BITCODE_GENERATION_MODE=bitcode"
                           + " clean build",
                           GetFullPath("mopub-ios-sdk-unity"))
                    // Have to rename the .js file inside the framework so that Unity doesn't try to compile it...
                    // This rename is reverted in the MoPubPostBuildiOS script during an app build.
                    && Mv(jsfile, jsfile + ".prevent_unity_compilation");
#endif
            default:
                Debug.LogError("Invalid build target: " + buildTarget);
                return false;
        }
    }


    // Undo the edit to the platform MoPub SDK's source file that contains the version string.
    private static bool RestoreVersionFile(BuildTarget buildTarget, bool internalSdkBuild)
    {
        Debug.Log("Restoring platform SDK's version file");
        var dir = GetSdkSubdir(buildTarget, internalSdkBuild);
        var file = FileWithVersionNumber[buildTarget];
        return Run("git", "checkout -- " + file, dir);
    }


    private static bool CopyBuildArtifacts(BuildTarget buildTarget, bool internalSdkBuild, bool debug)
    {
        Debug.Log("Copying MoPub libraries to Unity project");
        var sdkDir = GetSdkSubdir(buildTarget, internalSdkBuild);
        switch (buildTarget) {
            case BuildTarget.Android: {
                const string destDir = "Assets/Plugins/Android/mopub/libs";
                // Our wrapper jar.
                var jarDir = debug ? "debug" : "release";
                if (!Cp(GetFullPath("mopub-android-sdk-unity/build/intermediates/bundles/" + jarDir + "/classes.jar"),
                    Path.Combine(destDir, "mopub-unity-plugins.jar")))
                    return false;
                // Platform SDK jars.
                var libCps = from lib in new[] { "base", "banner", "interstitial", "rewardedvideo", "native-static" }
                             let src = string.Format("{0}/mopub-sdk/mopub-sdk-{1}/build/intermediates/bundles/{2}/classes.jar",
                                                     sdkDir, lib, jarDir)
                             let dst = string.Format("{0}/mopub-sdk-{1}.jar", destDir, lib)
                             select Cp(src, dst);
                if (libCps.Contains(false))
                    return false;
                // Copy native ads jar to placeholder directory (as a source for the preference being toggled)
                const string nativeJar = "mopub-sdk-native-static.jar";
                Cp(Path.Combine(destDir, nativeJar), Path.Combine("Assets/MoPub/Extras", nativeJar));
                // Check whether to remove the native ads jar only now, rather than in the loop above, so that it
                // gets removed even if it was present from a prior build.
                return EditorUserBuildSettings.activeScriptCompilationDefines.Contains(MoPubPreferences.MoPubNativeAdsDefine)
                    || Rm(Path.Combine(destDir, "mopub-sdk-native-static.jar"));
            }
#if UNITY_EDITOR_OSX
            case BuildTarget.iOS: {
                const string destDir = "Assets/Plugins/iOS";
                var projDir = GetFullPath("mopub-ios-sdk-unity/bin");
                var htmlDir = Path.Combine(sdkDir, "MoPubSDK/Resources");
                return Rsync(projDir, destDir, "*.h", "*.m", "*.mm", "*.framework")
                    && Rsync(htmlDir, Path.Combine(destDir, "MoPubSDKFramework.framework"), "*.html", "*.png");
            }
#endif
            default:
                Debug.LogError("Invalid build target: " + buildTarget);
                return false;
        }
    }


    #endregion

    #region Menu and Build Hooks


#if mopub_menu_beta
    private const string MoPubBuildInternalPref = "MoPubBuildInternal";
    private const string MoPubBuildDebugPref = "MoPubBuildDebug";
    private const string MoPubBuildOnDemandPref = "MoPubBuildOnDemand";

    private static bool BuildMoPubSdk(BuildTarget buildTarget)
    {
#if mopub_developer
        var internalSdkBuild = EditorPrefs.GetBool(MoPubBuildInternalPref, EditorUserBuildSettings.development);
#else
        const bool internalSdkBuild = false;
#endif
        var debug = EditorPrefs.GetBool(MoPubBuildDebugPref, EditorUserBuildSettings.development);

        if (!AppendUnityVersion(buildTarget, internalSdkBuild))
            return false;
        // Hang on to this since we want to undo the version edit even if the build fails.
        var built = BuildPlatformSdk(buildTarget, internalSdkBuild, debug);
        return RestoreVersionFile(buildTarget, internalSdkBuild)
            && built
            && CopyBuildArtifacts(buildTarget, internalSdkBuild, debug);
    }


    [MenuItem("MoPub/Build/Build Current Platform", false, 0)]
    public static bool BuildMoPubSdkCurrent()
    {
        return BuildMoPubSdk(EditorUserBuildSettings.activeBuildTarget);
    }


    [MenuItem("MoPub/Build/Build Current Platform", true)]
    public static bool ValidateBuildMoPubSdkCurrent()
    {
        return EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android
#if UNITY_EDITOR_OSX
            || EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS
#endif
            ;
    }


    [MenuItem("MoPub/Build/Build All Platforms", false, 1)]
    public static bool BuildMoPubSdkAll()
    {
        return BuildMoPubSdk(BuildTarget.Android)
            && BuildMoPubSdk(BuildTarget.iOS);
    }


    [MenuItem("MoPub/Build/Build All Platforms", true)]
    public static bool ValidateBuildMoPubSdkAll()
    {
        return
#if UNITY_EDITOR_OSX
            true
#else
            false
#endif
            ;
    }


    [MenuItem("MoPub/Build/Export MoPub Package", false, 2)]
    public static bool ExportPackage()
    {
        var build = EditorPrefs.GetBool(MoPubBuildOnDemandPref, false);
        if (build) {
            if (!BuildMoPubSdk(BuildTarget.Android))
                return false;
#if UNITY_EDITOR_OSX
            if (!BuildMoPubSdk(BuildTarget.iOS))
                return false;
#endif
        }

        Debug.Log("Exporting package");
        var dirs = new[] { "MoPub", "Plugins", "Scripts", "Scenes" };
        AssetDatabase.ExportPackage(dirs.Select(d => Path.Combine("Assets", d)).ToArray(),
                                    GetFullPath("mopub-unity-plugin/MoPubUnity.unitypackage"),
                                    ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
        return true;
    }

#if UNITY_5_6_OR_NEWER
    internal class BuildProcessor :
#if UNITY_2018_1_OR_NEWER
        IPreprocessBuildWithReport
#else
        IPreprocessBuild
#endif
    {
        // Controls order of execution among multiple IOrderedCallback implementations
        // https://docs.unity3d.com/ScriptReference/Build.IOrderedCallback.html
        public int callbackOrder { get { return 0; } }

#if UNITY_2018_1_OR_NEWER
        public void OnPreprocessBuild(BuildReport report)
        {
            var target = report.summary.platform;
#else
        public void OnPreprocessBuild(BuildTarget target, string path)
        {
#endif
            var build = EditorPrefs.GetBool(MoPubBuildOnDemandPref, false);
#if UNITY_EDITOR_WIN
            // Can't build iOS SDK on Windows
            if (target == BuildTarget.iOS)
                build = false;
#endif
            if (build)
                BuildMoPubSdk(target);
        }
    }
#endif

#endif


    #endregion

    #region Options


#if mopub_menu_beta
    private const string MopubBuildOption = "MoPub/Build/Option: ";


#if mopub_developer
    //////  Use Public SDK

    private const string MoPubBuildInternalOption = MopubBuildOption + "Use Public SDK/";
    private const string MoPubBuildInternalOptionAlways = MoPubBuildInternalOption + "Always";
    private const string MoPubBuildInternalOptionNever = MoPubBuildInternalOption + "Never";
    private const string MoPubBuildInternalOptionReleaseBuild = MoPubBuildInternalOption + "If Release Build";


    [MenuItem(MoPubBuildInternalOptionAlways, false, 20)]
    public static void ForceInternalBuild()
    {
        EditorPrefs.SetBool(MoPubBuildInternalPref, true);
    }


    [MenuItem(MoPubBuildInternalOptionAlways, true)]
    public static bool ValidateForceInternalBuild()
    {
        var isChecked = EditorPrefs.GetBool(MoPubBuildInternalPref, false);
        Menu.SetChecked(MoPubBuildInternalOptionAlways, isChecked);
        return true;
    }


    [MenuItem(MoPubBuildInternalOptionNever, false, 21)]
    public static void ForcePublicBuild()
    {
        EditorPrefs.SetBool(MoPubBuildInternalPref, false);
    }


    [MenuItem(MoPubBuildInternalOptionNever, true)]
    public static bool ValidateForcePublicBuild()
    {
        var isChecked = !EditorPrefs.GetBool(MoPubBuildInternalPref, true);
        Menu.SetChecked(MoPubBuildInternalOptionNever, isChecked);
        return true;
    }


    [MenuItem(MoPubBuildInternalOptionReleaseBuild, false, 22)]
    public static void InternalBuildInDevelopmentMode()
    {
        EditorPrefs.DeleteKey(MoPubBuildInternalPref);
    }


    [MenuItem(MoPubBuildInternalOptionReleaseBuild, true)]
    public static bool ValidateInternalBuildInDevelopmentMode()
    {
        var isChecked = !EditorPrefs.HasKey(MoPubBuildInternalPref);
        Menu.SetChecked(MoPubBuildInternalOptionReleaseBuild, isChecked);
        return true;
    }
#endif

    //////  Debug

    private const string MoPubBuildDebugOption = MopubBuildOption + "Debug Build/";
    private const string MoPubBuildDebugOptionAlways = MoPubBuildDebugOption + "Always";
    private const string MoPubBuildDebugOptionNever = MoPubBuildDebugOption + "Never";
    private const string MoPubBuildDebugOptionDevelopmentBuild = MoPubBuildDebugOption + "If Development Build";


    [MenuItem(MoPubBuildDebugOptionAlways, false, 23)]
    public static void ForceDebugBuild()
    {
        EditorPrefs.SetBool(MoPubBuildDebugPref, true);
    }


    [MenuItem(MoPubBuildDebugOptionAlways, true)]
    public static bool ValidateForceDebugBuild()
    {
        var isChecked = EditorPrefs.GetBool(MoPubBuildDebugPref, false);
        Menu.SetChecked(MoPubBuildDebugOptionAlways, isChecked);
        return true;
    }


    [MenuItem(MoPubBuildDebugOptionNever, false, 24)]
    public static void ForceReleaseBuild()
    {
        EditorPrefs.SetBool(MoPubBuildDebugPref, false);
    }


    [MenuItem(MoPubBuildDebugOptionNever, true)]
    public static bool ValidateForceReleaseBuild()
    {
        var isChecked = !EditorPrefs.GetBool(MoPubBuildDebugPref, true);
        Menu.SetChecked(MoPubBuildDebugOptionNever, isChecked);
        return true;
    }


    [MenuItem(MoPubBuildDebugOptionDevelopmentBuild, false, 25)]
    public static void DebugBuildInDevelopmentMode()
    {
        EditorPrefs.DeleteKey(MoPubBuildDebugPref);
    }


    [MenuItem(MoPubBuildDebugOptionDevelopmentBuild, true)]
    public static bool ValidateDebugBuildInDevelopmentMode()
    {
        var isChecked = !EditorPrefs.HasKey(MoPubBuildDebugPref);
        Menu.SetChecked(MoPubBuildDebugOptionDevelopmentBuild, isChecked);
        return true;
    }


    //////  On Demand

    private const string MoPubBuildOnDemandOption = MopubBuildOption + "Compile on App Build";


    [MenuItem(MoPubBuildOnDemandOption, false, 26)]
    public static void ToggleOnDemandBuild()
    {
        var onDemand = EditorPrefs.GetBool(MoPubBuildOnDemandPref, false);
        EditorPrefs.SetBool(MoPubBuildOnDemandPref, !onDemand);
    }


    [MenuItem(MoPubBuildOnDemandOption, true)]
    public static bool ValidateToggleOnDemandBuild()
    {
        var isChecked = EditorPrefs.GetBool(MoPubBuildOnDemandPref, false);
        Menu.SetChecked(MoPubBuildOnDemandOption, isChecked);
        return true;
    }
#endif


    #endregion
}
