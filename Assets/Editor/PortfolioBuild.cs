using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

// Batch-mode WebGL build for the portfolio. Added 2026-10 as build tooling, not gameplay.
//   Unity.exe -batchmode -quit -projectPath <project> -executeMethod PortfolioBuild.WebGL -buildOut <dir> -logFile <log>
public static class PortfolioBuild
{
	public static void WebGL()
	{
		string outDir = GetArg("-buildOut") ?? "Builds/WebGL";

		// The 2022 build used a fixed 1920x1024 canvas, too big for a laptop browser window.
		PlayerSettings.defaultWebScreenWidth = 1280;
		PlayerSettings.defaultWebScreenHeight = 720;
		// Uncompressed files run from any static host without Content-Encoding headers.
		PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;

		BuildPlayerOptions options = new BuildPlayerOptions
		{
			scenes = new[] { "Assets/Scenes/Menu.unity", "Assets/Scenes/Game.unity" },
			locationPathName = outDir,
			target = BuildTarget.WebGL,
			options = BuildOptions.None
		};
		BuildReport report = BuildPipeline.BuildPlayer(options);
		Debug.Log("PortfolioBuild: " + report.summary.result + ", errors " + report.summary.totalErrors
			+ ", " + report.summary.totalSize + " bytes -> " + outDir);
		if (report.summary.result != BuildResult.Succeeded)
		{
			EditorApplication.Exit(1);
		}
	}

	static string GetArg(string name)
	{
		string[] args = Environment.GetCommandLineArgs();
		for (int i = 0; i < args.Length - 1; i++)
		{
			if (args[i] == name)
			{
				return args[i + 1];
			}
		}
		return null;
	}
}
