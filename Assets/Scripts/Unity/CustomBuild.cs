using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class CustomBuild
{
    [MenuItem("Tools/Build game without strictmode")]
    public static void BuildGame()
    {
        string buildPath = "C:/Users/RdenBlaauwen/Desktop/test"; // Change this to your desired output folder
        string[] scenes =
        {
            "Assets/Scenes/MainMenu.unity",
            "Assets/Scenes/IntroScene.unity",
            "Assets/Scenes/LevelsMenu.unity",
            "Assets/Scenes/Level_1.unity",
            "Assets/Scenes/Level_2.unity",
            "Assets/Scenes/Level_3.unity",
            "Assets/Scenes/Level_4.unity",
            "Assets/Scenes/Briefings/Briefinglvl1.unity",
            "Assets/Scenes/Level_5.unity",
            "Assets/Scenes/BadEnd.unity",
            "Assets/Scenes/GoodEnd.unity",
        }; // List your scenes here

        BuildOptions options = BuildOptions.None; // Remove StrictMode by not including it
        
        BuildReport report = BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.StandaloneWindows64, options);

        if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded!");
        }
        else
        {
            Debug.LogError("Build failed: " + report.summary.result);
        }
    }
}