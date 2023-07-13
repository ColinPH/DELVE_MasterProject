using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PropellerCap.QA
{
    public class LoggerUtility
    {
        /// <summary>
        /// Returns whether the specified folder path contains the specified folder name
        /// </summary>
        public static bool FolderNameExists(string folderPath, string nameToTest)
        {
            //TODO this doesn't support macOS
            return Directory.Exists(folderPath + "/" + nameToTest);
        }

        /// <summary>
        /// Tries to create a folder at the given path
        /// </summary>
        public static bool CreateFolderAtPath(string folderPath, string newFolderName, out string newFolderPath)
        {
            newFolderPath = folderPath + GetPathSeparator() + newFolderName;

            if (Directory.Exists(newFolderPath))
                return false;
            else
            {
                //We create a new folder
                Directory.CreateDirectory(newFolderPath);
                return true;
            }
        }

        public static string GetPathSeparator()
        {
            //TODO support macOS
            return "/";
        }

        public static List<DirectoryInfo> GetFolderPathsInFolder(string parentFolder)
        {
            DirectoryInfo mainFolder = new DirectoryInfo(parentFolder);
            List<DirectoryInfo> toReturn = new List<DirectoryInfo>();
            foreach (DirectoryInfo folder in mainFolder.GetDirectories())
            {
                toReturn.Add(folder);
            }
            return toReturn;
        }

        /// <summary>
        /// Deletes the folder at the given path
        /// </summary>
        public static void DeleteFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
                Directory.Delete(folderPath, true);
            else
                Debug.LogError("Tried to delete a missing folder at path " + folderPath);
        }
    }
}