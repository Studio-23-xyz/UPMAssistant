using UnityEngine;
using System.IO;

public class FileSeparator : MonoBehaviour
{
    void Start()
    {
        string filePath = "Assets/Packages/com.studio23.ss2.test22222222/unknown.pdf";

        // Get the directory and file name using Path methods
        string directory = Path.GetDirectoryName(filePath);
        string fileName = Path.GetFileName(filePath);

        // Print the results to the console
        Debug.Log("Directory: " + directory);
        Debug.Log("File Name: " + fileName);
    }
}