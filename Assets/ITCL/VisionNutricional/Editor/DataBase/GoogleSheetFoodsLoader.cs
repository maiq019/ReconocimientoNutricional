using System.Collections.Generic;
using System.IO;
using ITCL.VisionNutricional.Runtime.DataBase;
using UnityEditor;
using UnityEngine;
using WhateverDevs.Localization.Editor;
using CsvReader = ITCL.VisionNutricional.Runtime.DataBase.CsvReader;

namespace ITCL.VisionNutricional.Editor.DataBase
{
    /// <summary>
    /// Default implementation of the google sheet loader.
    /// </summary>
    public class DefaultGoogleSheetLoader : GoogleSheetsLoader
    {
        [MenuItem("Nutrision/Foods/GoogleDrive #&f")]
        public static void ShowWindow() => GetWindow(typeof(DefaultGoogleSheetLoader), false, "Drive Sheet Food Parser");
    }

    /// <summary>
    ///     Class to load different Google sheets
    /// </summary>
    public abstract class GoogleSheetsLoader : EditorWindow
    {
        private string downLoadUrl = "";

        private bool deleteFileWhenFinished = true;

        private const string TemporalPath = "Temp/FoodsFile.csv";

        private void OnEnable()
        {
            downLoadUrl = EditorGUIUtility.systemCopyBuffer;
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("The file in drive must be shared publicly!", MessageType.Warning);

            EditorGUILayout.HelpBox("This tool downloads the file as a tsv with tab separators. "
                                    + "If you have tabs inside your localization text you will fuck up!",
                MessageType.Warning);

            downLoadUrl = EditorGUILayout.TextField("Url to file", downLoadUrl);

            deleteFileWhenFinished = EditorGUILayout.Toggle("Delete file when finished", deleteFileWhenFinished);

            if (GUILayout.Button("Download and Parse")) LoadFoods();
        }

        /// <summary>
        /// Load foods.
        /// </summary>
        /// <returns>IEnumerator</returns>
        private void LoadFoods()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Loading foods", "Downloading file...", .25f);

                string sheetUrl = downLoadUrl.Replace("edit?usp=sharing", "export?format=tsv");

                if (File.Exists(TemporalPath)) File.Delete(TemporalPath);

                FileInfo file = DriveFileDownloader.DownloadFileFromURLToPath(sheetUrl, TemporalPath);

                EditorUtility.DisplayProgressBar("Loading foods", "Parsing file...", .5f);

                ParseFoodsData(File.ReadAllText(file.FullName));

                if (deleteFileWhenFinished) file.Delete();
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        /// <summary>
        /// Parses the CSV formatted Sheet
        /// </summary>
        /// <param name="csvData">The Sheet in CSV format</param>
        private void ParseFoodsData(string csvData)
        {
            List<DB.Food> foodsList = CsvReader.Read(csvData);

            const string folderPath = "Assets/Resources/Foods/";

            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            string filePath = folderPath + "Foods.asset";

            ScriptableFood asset;

            if(File.Exists(filePath))
            {
                asset = AssetDatabase.LoadAssetAtPath<ScriptableFood>(filePath);
            }
            else
            {
                asset = CreateInstance<ScriptableFood>();
                AssetDatabase.CreateAsset(asset, filePath);
            }

            asset.Foods = foodsList;

            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
        }
    }
}