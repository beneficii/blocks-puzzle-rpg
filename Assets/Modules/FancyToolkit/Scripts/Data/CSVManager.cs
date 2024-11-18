using System.IO;
using UnityEngine;

namespace FancyToolkit
{
    public class CSVManager
    {
        private static CSVManager _current;

        public static CSVManager current
        {
            get
            {
                _current ??= new CSVManager();
                return _current;
            }
        }

        const string csvSubDirectory = "CSVTables";
        private readonly string csvDirectory;
        private readonly bool usePersistentStorage;

        private CSVManager()
        {
#if !UNITY_WEBGL

#if UNITY_ANDROID && !UNITY_EDITOR
            csvDirectory = Path.Combine("/storage/emulated/0/Documents", Application.productName, csvSubDirectory);
#else
            csvDirectory = Path.Combine(Application.persistentDataPath, csvSubDirectory);
#endif

            if (!Directory.Exists(csvDirectory))
            {
                Directory.CreateDirectory(csvDirectory);
            }

            CopyDefaultCSVFiles();
            usePersistentStorage = true;
#endif
        }

        private void CopyDefaultCSVFiles()
        {
            var csvFiles = Resources.LoadAll<TextAsset>(csvSubDirectory);

            if (csvFiles.Length == 0)
            {
                Debug.LogError($"No text files found in {csvSubDirectory}");
            }

            foreach (TextAsset csvFile in csvFiles)
            {
                string fileName = csvFile.name + ".csv";
                string destinationPath = Path.Combine(csvDirectory, fileName);

                if (!File.Exists(destinationPath))
                {
                    File.WriteAllText(destinationPath, csvFile.text);
                }
            }
        }

        public string GetContent(string fileName)
        {
            if (usePersistentStorage)
            {
                string filePath = Path.Combine(csvDirectory, fileName + ".csv");
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }

            // Fallback to original file in Resources
            TextAsset csvFile = Resources.Load<TextAsset>($"{csvSubDirectory}/{fileName}");
            if (csvFile == null)
            {
                Debug.LogError($"CSV file '{fileName}' not found in persistent storage or Resources.");
                return null;
            }

            
            return csvFile.text;
        }
    }
}.