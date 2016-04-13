namespace MoPubInternal.Editor.Postbuild
{
	using UnityEditor;
	using UnityEngine;
	using UnityEditor.Callbacks;
	using System.IO;
	using System.Collections.Generic;
	using System.Linq;
	using MoPubInternal.Editor.Postbuild;
	using MoPubInternal.Editor.ThirdParty.xcodeapi;

	public class MoPubPostBuildiOS : PostBuildiOS
	{
		private static string[] platformFrameworks = new string[] {
			"AdSupport.framework",
			"StoreKit.framework",
			"EventKit.framework",
			"EventKitUI.framework",
			"CoreTelephony.framework",
			// AdMob
			"MessageUI.framework",
			// Millennial
			"MediaPlayer.framework",
			"PassKit.framework",
			"Social.framework",
			"MobileCoreServices.framework"
		};

		private static string[] frameworks = new string[] {
			"Fabric.framework"
		};

		private static string[] platformLibs = new string[] {
			"libz.dylib",
			"libsqlite3.dylib"
		};

		private static string[] libs = new string[] {
			"Fabric-Init/libFabriciOSInit.a",
			"MoPub/libMoPubSDK.a"
		};

		[PostProcessBuild(100)]
		public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath)
		{
			// BuiltTarget.iOS is not defined in Unity 4, so we just use strings here
			if (buildTarget.ToString () == "iOS" || buildTarget.ToString () == "iPhone") {
				CheckiOSVersion ();

				PrepareProject (buildPath);
				PreparePlist (buildPath, "MoPub");

				RenameMRAIDSource (buildPath);
			}
		}

		private static void CheckiOSVersion()
		{
			iOSTargetOSVersion[] oldiOSVersions = {
				iOSTargetOSVersion.iOS_4_0,
				iOSTargetOSVersion.iOS_4_1,
				iOSTargetOSVersion.iOS_4_2,
				iOSTargetOSVersion.iOS_4_3,
				iOSTargetOSVersion.iOS_5_0,
				iOSTargetOSVersion.iOS_5_1,
				iOSTargetOSVersion.iOS_6_0
			};
			var isOldiOSVersion = oldiOSVersions.Contains (PlayerSettings.iOS.targetOSVersion);
			
			if (isOldiOSVersion) {
				Debug.LogWarning ("MoPub requires iOS 7+. Please change the Target iOS Version in Player Settings to iOS 7 or higher.");
			}
		}

		private static void PrepareProject(string buildPath)
		{
			string projPath = Path.Combine (buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");
			PBXProject project = new PBXProject ();
			project.ReadFromString (File.ReadAllText(projPath));		
			string target = project.TargetGuidByName ("Unity-iPhone");
			
			AddPlatformFrameworksToProject (platformFrameworks, project, target);		
			AddFrameworksToProject (frameworks, buildPath, project, target);
			AddPlatformLibsToProject (platformLibs, project, target);
			AddLibsToProject (libs, project, target, buildPath);
			AddBuildProperty (project, target, "OTHER_LDFLAGS", "-ObjC");

			File.WriteAllText (projPath, project.WriteToString());			
		}

		private static void RenameMRAIDSource (string buildPath)
		{
			// Unity will try to compile anything with the ".js" extension. Since mraid.js is not intended
			// for Unity, it'd break the build. So we store the file with a fake extension and after the 
			// build rename it to the correct one.
			
			string basePath = Path.Combine (buildPath, "Frameworks/Plugins/iOS/Fabric/MoPub/MRAID.bundle");
			File.Move(Path.Combine(basePath, "mraid.js.prevent_unity_compilation"), Path.Combine(basePath, "mraid.js"));
		}
	}
}
