using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace PropellerCap
{
    public class LocalizationDataReader
    {
        private UnityEngine.Object _localizationDataFile;

        Dictionary<LanguageType, List<string>> _processedData = new Dictionary<LanguageType, List<string>>();
        List<int> _IDs = new List<int>();
        List<string> _contexts = new List<string>();
        List<string> _notes = new List<string>();

        string _filePath = "";

        public LocalizationDataReader(UnityEngine.Object localisationDataFile)
        {
            _localizationDataFile = localisationDataFile;
#if UNITY_EDITOR
            _filePath = Path.Combine(Application.dataPath, AssetDatabase.GetAssetPath(localisationDataFile).Substring("Assets\\".Length));
#endif
        }

        public void ProcessDataFile()
        {
            _IDs.Clear();
            _contexts.Clear();
            _notes.Clear();
            _processedData = new Dictionary<LanguageType, List<string>>();

            // Check that the file is a TextAsset
            TextAsset csvText = _localizationDataFile as TextAsset;
            if (csvText == null)
            {
                Debug.LogError("File is not a TextAsset");
                return;
            }
            
            // Split the text into rows
            string[] rows = csvText.text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            // Get the number of columns for later iteration
            int numCols = rows[0].Split(',').Length;

            if (rows.Length == 1) return;

            // Loop through the rows starting from index 1 to skip the column titles
            for (int i = 1; i < rows.Length; i++)
            {
                // Split the row into columns
                string[] columns = rows[i].Split(',');

                //Colomn 0 is the IDs
                if (int.TryParse(columns[0], out int result) == false)
                    continue;
                _IDs.Add(result);

                //Column 1 is the context
                _contexts.Add(columns[1]);
                //Column 2 is the notes
                _notes.Add(columns[2]);

                // Loop through the columns
                for (int j = 0; j < numCols; j++)
                {
                    // Skip the first 3 columns
                    if (j < 3) continue;

                    LanguageType currentLanguage = _GetLanguageFromIndex(j);

                    if (_processedData.ContainsKey(currentLanguage) == false)
                        _processedData.Add(currentLanguage, new List<string>());

                    //Add the data to the corresponding language
                    string toAdd = columns[j];
                    _processedData[currentLanguage].Add(toAdd);
                }
            }
        }

        public string SampleLanguageData(LanguageType targetLanguage, int localizationID)
        {
            int realID = _IDs.IndexOf(localizationID);
            //Sample the list at the given language
            return _processedData[targetLanguage][realID];
        }

        public int FindNextAvailableLocalizationID()
        {
            int nextID = 1;
            while (_IDs.Contains(nextID))
            {
                nextID++;
            }
            return nextID;
        }

        public TextClip GetTextClipInfo(LanguageType targetLanguage, int localizationID)
        {
            int realID = _IDs.IndexOf(localizationID);
            string text = _processedData[targetLanguage][realID];
            string context = _contexts[realID];
            string notes = _notes[realID];
            return new TextClip(localizationID, text, context, notes, 2f);
        }

        public Dictionary<int, string> GetNotes()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();
            for (int i = 0; i < _IDs.Count; i++)
            {
                toReturn.Add(_IDs[i], _notes[i]);
            }
            return toReturn;
        }

        public Dictionary<int, string> GetContexts()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();
            for (int i = 0; i < _IDs.Count; i++)
            {
                toReturn.Add(_IDs[i], _contexts[i]);
            }
            return toReturn;
        }

        public Dictionary<int, string> GetDefaultTexts()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();
            for (int i = 0; i < _IDs.Count; i++)
            {
                toReturn.Add(_IDs[i], _processedData[LanguageType.English][i]);
            }
            return toReturn;
        }

#if UNITY_EDITOR
        public void SetNotes(int localizationID, string newText)
        {
            if (_IDs.Contains(localizationID) == false)
            {
                Debug.LogError($"The localization file does not contain the ID ({localizationID}), can not set the notes at that ID.");
                return;
            }

            // Split the text into lines
            TextAsset csvText = _localizationDataFile as TextAsset;
            string[] lines = csvText.text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            int rowIndex = _IDs.IndexOf(localizationID) + 1; //Skip the header row
            //Debug.Log("ID is " + rowIndex);
            var row = lines[rowIndex];
            var cells = row.Split(',');
            cells[2] = newText; //Index 2 is the notes
            lines[rowIndex] = string.Join(",", cells);

            StreamWriter writer = new StreamWriter(_filePath, false);
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
            writer.Close();
        }

        public void SetContext(int localizationID, string newText)
        {
            if (_IDs.Contains(localizationID) == false)
            {
                Debug.LogError($"The localization file does not contain the ID ({localizationID}), can not set the context at that ID.");
                return;
            }

            // Split the text into lines
            TextAsset csvText = _localizationDataFile as TextAsset;
            string[] lines = csvText.text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            int rowIndex = _IDs.IndexOf(localizationID) + 1; //Skip the header row
            var row = lines[rowIndex];
            var cells = row.Split(',');
            cells[1] = newText; //Index 1 is the context
            lines[rowIndex] = string.Join(",", cells);

            StreamWriter writer = new StreamWriter(_filePath, false);
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
            writer.Close();
        }

        public void SetDefaultText(int localizationID, string newText)
        {
            if (_IDs.Contains(localizationID) == false)
            {
                Debug.LogError($"The localization file does not contain the ID ({localizationID}), can not set the default text at that ID.");
                return;
            }
            
            // Split the text into lines
            TextAsset csvText = _localizationDataFile as TextAsset;
            string[] lines = csvText.text.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

            int rowIndex = _IDs.IndexOf(localizationID) + 1; //Skip the header row
            var row = lines[rowIndex];
            var cells = row.Split(',');
            cells[3] = newText; //Index 3 is the default language (english)
            lines[rowIndex] = string.Join(",", cells);

            StreamWriter writer = new StreamWriter(_filePath, false);
            foreach (var line in lines)
            {
                writer.WriteLine(line);
            }
            writer.Close();
        }

        public void AddLineToFile(int localizationID, string context, string notes, string defaultText)
        {
            // Combine the parameters into a single string
            string newLine = string.Join(",", localizationID.ToString(), context, notes, defaultText);
            //Add the languages that are not english (amount of languages - 1)
            for (int i = 0; i < Enum.GetValues(typeof(LanguageType)).Length - 1; i++)
            {
                newLine = string.Join(",", newLine, "");
            }
            // Append the new line to the CSV file
            File.AppendAllText(_filePath, newLine + Environment.NewLine);
        }
#endif

        private LanguageType _GetLanguageFromIndex(int languageIndex)
        {
            switch (languageIndex - 3) //Remove 3 to match the indexes of the enum
            {
                case 0: return LanguageType.English;
                case 1: return LanguageType.Danish;
                case 2: return LanguageType.French;
                case 3: return LanguageType.Dutch;
                default:
                    Debug.LogError($"Language column index ({languageIndex}) is not implemented in '{nameof(LocalizationDataReader)}' switch statement. Using {LanguageType.English} as default");
                    return LanguageType.English;
            }
        }
    }
}