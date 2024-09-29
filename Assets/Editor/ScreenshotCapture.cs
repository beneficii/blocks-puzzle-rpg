using UnityEngine;
using UnityEditor;
using System.IO;

public class ScreenshotCapture : EditorWindow
{
    static string screenshotFileName = "Screenshot";
    static string folderPath = "Screenshots";
    static int superSize = 1;

    // Assign F10 as the hotkey for capturing the screenshot
    [MenuItem("Tools/Capture Screenshot _F10")]
    public static void CaptureScreenshotHotkey()
    {
        CaptureScreenshot();
    }

    static void CaptureScreenshot()
    {
        // Ensure the folder exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Define the file path
        string filePath = Path.Combine(folderPath, screenshotFileName + "_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png");

        // Capture the screenshot
        ScreenCapture.CaptureScreenshot(filePath, superSize);

        Debug.Log($"Screenshot saved to: {filePath}");
    }
}
