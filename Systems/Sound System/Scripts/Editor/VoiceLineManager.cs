using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace PropellerCap
{
    public class VoiceLineManager : EditorWindow
    {
        private VoiceLine _voiceLineObj;
        SerializedObject _windowSO;

        //Main properties
        List<VoiceLine> _voiceLines = new List<VoiceLine>();
        int _currentVoiceLineIndex = 0;
        LocalizationDataReader _dataReader;

        Dictionary<int, string> _defaultTextLocalized = new Dictionary<int, string>();
        Dictionary<int, string> _contextLocalized = new Dictionary<int, string>();
        Dictionary<int, string> _notesLocalized = new Dictionary<int, string>();

        //Localization file
        string _localiationFilePath = "";
        TextAsset _localizationFile;

        //VoiceLine sections temporary data
        string _temporaryDefaultText = "";
        string _temporaryContext = "";
        string _temporaryNotes = "";

        //Main controls
        int _quickInspectIndex = 0;
        int _totalVoiceLines = 0;

        [MenuItem("Propeller Cap/Tools/Voice Line Manager")]
        public static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(VoiceLineManager));
            window.titleContent = new GUIContent("Voice Line Manager");
        }

        private void OnEnable()
        {
            _windowSO = new SerializedObject(this);
            _FindAndProcessVoiceLines();

            if (Selection.objects[0] is TextAsset)
                _localizationFile = Selection.objects[0] as TextAsset;

            if (_localizationFile != null)
            {
                _ProcessLocalizationFile(_localizationFile);
                _ReadTemporaryInfoFromVoiceLine(_voiceLines[_currentVoiceLineIndex]);
            }
        }

        private void OnGUI()
        {
            _windowSO.Update();

            //Draw the field to select the localization file
            _DrawLocalizationFile();

            //Draw the warnings about voice lines with wrong or identical IDs
            _DrawErrorsAndWarnings();

            //Draw the controls to move to the next or previous voice line, go to a specific one, and show the progress
            _DrawMainControls();

            //Draw all the information about the current voice line
            _DrawActiveVoiceLineInfo(_voiceLines[_currentVoiceLineIndex]);

            _windowSO.ApplyModifiedProperties();
        }

        private void _DrawLocalizationFile()
        {
            GUILayout.Space(10);
            GUIContent content = new GUIContent("Localization File");
            _localizationFile = EditorGUILayout.ObjectField(content, _localizationFile, typeof(TextAsset), false) as TextAsset;
            _DrawHorizontalSeparationLine();
        }

        private void _DrawMainControls()
        {
            GUILayout.BeginHorizontal();

            //Enter the index of the voice line to inspect
            _quickInspectIndex = EditorGUILayout.IntField("", _quickInspectIndex, GUILayout.Width(40));

            //Focus the given integer
            if (GUILayout.Button("Inspect", GUILayout.Width(55)))
            {
                GUI.FocusControl(null);
                if (_quickInspectIndex > 0 && _quickInspectIndex <= _totalVoiceLines)
                    _currentVoiceLineIndex = _quickInspectIndex - 1;
                _ReadTemporaryInfoFromVoiceLine(_voiceLines[_currentVoiceLineIndex]);
            }

            //Go to previous voice line
            if (GUILayout.Button("<<", GUILayout.Width(28)))
            {
                GUI.FocusControl(null);
                _currentVoiceLineIndex -= 1;
                if (_currentVoiceLineIndex < 0)
                    _currentVoiceLineIndex = _totalVoiceLines - 1;
                _ReadTemporaryInfoFromVoiceLine(_voiceLines[_currentVoiceLineIndex]);
            }

            //Go to next voice line
            if (GUILayout.Button(">>", GUILayout.Width(28)))
            {
                GUI.FocusControl(null);
                _currentVoiceLineIndex += 1;
                if (_currentVoiceLineIndex >= _totalVoiceLines)
                    _currentVoiceLineIndex = 0;
                _ReadTemporaryInfoFromVoiceLine(_voiceLines[_currentVoiceLineIndex]);
            }

            //The slider that represents the progress
            GUILayout.HorizontalSlider((float)(_currentVoiceLineIndex) / (float)(_totalVoiceLines - 1), 0.0f, 1.0f, GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));

            //Show the text that represents the progress
            GUILayout.Label($"[{_currentVoiceLineIndex + 1}-{_totalVoiceLines}]", GUILayout.ExpandWidth(false));

            //Update the list of voice lines
            if (GUILayout.Button("Update", GUILayout.Width(60)))
            {
                GUI.FocusControl(null);
                _FindAndProcessVoiceLines();
                _ProcessLocalizationFile(_localizationFile);
                _ReadTemporaryInfoFromVoiceLine(_voiceLines[_currentVoiceLineIndex]);
            }

            GUILayout.EndHorizontal();

            _DrawHorizontalSeparationLine();
        }

        private void _DrawActiveVoiceLineInfo(VoiceLine voiceLine)
        {
            EditorGUI.BeginChangeCheck();

            //The current voice line scriptable object to facilitate finding it in the assets
            _DrawCurrentVoiceLineScriptableObject(voiceLine);

            //Localization ID and screen time
            _DrawLocalizationIDAndScreenTime(voiceLine);

            //The Wwise event
            _DrawVoiceLineWwiseEvent(voiceLine);

            _DrawHorizontalSeparationLine();

            //The default text of the voice line
            _DrawVoiceLineDefaultText(voiceLine);

            _DrawHorizontalSeparationLine();

            //The context of the voice line
            _DrawVoiceLineContext(voiceLine);

            _DrawHorizontalSeparationLine();

            //The notes about that voice line
            _DrawVoiceLineNotes(voiceLine);


            //Set the VoiceLine scriptable object dirty if there have been changes
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(voiceLine);
            }

            _DrawHorizontalSeparationLine();
        }


        #region Draw the sections of the current voice line
        private void _DrawCurrentVoiceLineScriptableObject(VoiceLine voiceLine)
        {
            GUIContent content = new GUIContent("Current voice line SO :");
            EditorGUILayout.ObjectField(content, voiceLine, typeof(VoiceLine), false);
        }

        private void _DrawLocalizationIDAndScreenTime(VoiceLine voiceLine)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Localization ID", GUILayout.Width(90));
            voiceLine.LocalizationID = EditorGUILayout.IntField("", voiceLine.LocalizationID, GUILayout.Width(40));
            //Button to find the next localization ID available
            if (GUILayout.Button("Find Available", GUILayout.Width(120)))
            {
                GUI.FocusControl(null);
                _dataReader.ProcessDataFile();
                voiceLine.LocalizationID = _dataReader.FindNextAvailableLocalizationID();
            }
            
            GUILayout.Label($"Screen Time", GUILayout.Width(80));
            voiceLine.ScreenTime = EditorGUILayout.FloatField("", voiceLine.ScreenTime, GUILayout.Width(40));
            GUILayout.EndHorizontal();
        }

        private void _DrawVoiceLineWwiseEvent(VoiceLine voiceLine)
        {
            SerializedObject serializedObject = new SerializedObject(voiceLine);
            SerializedProperty myProperty = serializedObject.FindProperty(nameof(voiceLine.soundEvent));
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Sound Event", GUILayout.Width(90));
            EditorGUILayout.PropertyField(myProperty, new GUIContent(""));
            GUILayout.EndHorizontal();
        }

        private void _DrawVoiceLineDefaultText(VoiceLine voiceLine)
        {
            bool defaultTextIsIdentical = true;
            //Title of the section
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Default Text:", GUILayout.Width(190));
            //Display warning if the localization ID is not valid
            if (voiceLine.LocalizationID < 0)
            {
                EditorGUILayout.HelpBox("The localization ID should not be negative.", MessageType.Warning);
            }
            //Display warning if the localization file does not contain an entry with the given ID
            else if (_defaultTextLocalized.ContainsKey(voiceLine.LocalizationID) == false)
            {
                EditorGUILayout.HelpBox($"The localization file does not contain an entry with ID ({voiceLine.LocalizationID}).", MessageType.Warning);
                //Draw button to add it to the localization file
                GUIStyle buttonStyle = GUI.skin.button;
                buttonStyle.wordWrap = true;
                if (GUILayout.Button("Add to file now", buttonStyle, GUILayout.Height(EditorGUIUtility.singleLineHeight * 2)))
                {
                    //TODO Add to the file
                    _dataReader.AddLineToFile(voiceLine.LocalizationID, voiceLine.Context, voiceLine.Notes, voiceLine.DefaultText);

                    //Update the temporary veriables
                    _defaultTextLocalized[voiceLine.LocalizationID] = voiceLine.DefaultText;
                    _contextLocalized[voiceLine.LocalizationID] = voiceLine.Context;
                    _notesLocalized[voiceLine.LocalizationID] = voiceLine.Notes;
                    _temporaryDefaultText = voiceLine.DefaultText;
                    _temporaryContext = voiceLine.Context;
                    _temporaryNotes = voiceLine.Notes;

                    EditorUtility.SetDirty(_localizationFile);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            //If text in SO and localization file are different
            else if (voiceLine.DefaultText != _defaultTextLocalized[voiceLine.LocalizationID])
            {
                EditorGUILayout.HelpBox($"Default text in the localization file and {nameof(VoiceLine)} SO do not match.", MessageType.Warning);
                defaultTextIsIdentical = false;
            }
            GUILayout.EndHorizontal();

            if (defaultTextIsIdentical)
            {
                _temporaryDefaultText = EditorGUILayout.TextArea(_temporaryDefaultText, GUILayout.Height(60));
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                bool userHasMadeChanges = _temporaryDefaultText != voiceLine.DefaultText;
                if (userHasMadeChanges == false)
                    GUI.backgroundColor = Color.grey;
                if (GUILayout.Button("Apply", GUILayout.Width(100)) && userHasMadeChanges)
                {
                    voiceLine.DefaultText = _temporaryDefaultText;
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                //Display the two texts next to each other and buttons to select if there is a difference
                bool keepSO = false;
                bool keepFile = false;
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                EditorStyles.textField.wordWrap = true;
                string tempSO = EditorGUILayout.TextArea(voiceLine.DefaultText, GUILayout.Height(60), GUILayout.Width(position.width / 2f));
                if (GUILayout.Button("Keep SO", GUILayout.Width(100)))
                    keepSO = true;
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                string tempFile = EditorGUILayout.TextArea(_defaultTextLocalized[voiceLine.LocalizationID], GUILayout.Height(60), GUILayout.Width(position.width / 2f));
                if (GUILayout.Button("Keep File", GUILayout.Width(100)))
                    keepFile = true;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                //Apply the changes of the one that is kept
                string newText = "";
                if (keepSO)
                {
                    _defaultTextLocalized[voiceLine.LocalizationID] = tempSO;
                    newText = tempSO;
                }
                else if (keepFile)
                {
                    voiceLine.DefaultText = tempFile;
                    newText = tempFile;
                }

                //Apply the changes
                if (keepSO || keepFile)
                {
                    EditorUtility.SetDirty(voiceLine);
                    //Change the data in the file
                    if (keepSO)
                    {
                        _dataReader.ProcessDataFile();
                        _dataReader.SetDefaultText(voiceLine.LocalizationID, newText);
                    }

                    _temporaryDefaultText = newText;
                    EditorUtility.SetDirty(_localizationFile);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        private void _DrawVoiceLineContext(VoiceLine voiceLine)
        {
            bool defaultTextIsIdentical = true;
            //Title of the section
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Context:", GUILayout.Width(190));
            //Display warning if the localization ID is not valid
            if (voiceLine.LocalizationID < 0)
            {
                EditorGUILayout.HelpBox("The localization ID should not be negative.", MessageType.Warning);
            }
            //Display warning if the localization file does not contain an entry with the given ID
            else if (_contextLocalized.ContainsKey(voiceLine.LocalizationID) == false)
            {
                EditorGUILayout.HelpBox($"The localization file does not contain an entry with ID ({voiceLine.LocalizationID}).", MessageType.Warning);
            }
            //If text in SO and localization file are different
            else if (voiceLine.Context != _contextLocalized[voiceLine.LocalizationID])
            {
                EditorGUILayout.HelpBox($"Context in the localization file and {nameof(VoiceLine)} SO do not match.", MessageType.Warning);
                defaultTextIsIdentical = false;
            }
            GUILayout.EndHorizontal();

            if (defaultTextIsIdentical)
            {
                _temporaryContext = EditorGUILayout.TextArea(_temporaryContext, GUILayout.Height(60));
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                bool userHasMadeChanges = _temporaryContext != voiceLine.Context;
                if (userHasMadeChanges == false)
                    GUI.backgroundColor = Color.grey;
                if (GUILayout.Button("Apply", GUILayout.Width(100)) && userHasMadeChanges)
                {
                    voiceLine.Context = _temporaryContext;
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                //Display the two texts next to each other and buttons to select if there is a difference
                bool keepSO = false;
                bool keepFile = false;
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                EditorStyles.textField.wordWrap = true;
                string tempSO = EditorGUILayout.TextArea(voiceLine.Context, GUILayout.Height(60), GUILayout.Width(position.width / 2f));
                if (GUILayout.Button("Keep SO", GUILayout.Width(100)))
                    keepSO = true;
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                string tempFile = EditorGUILayout.TextArea(_contextLocalized[voiceLine.LocalizationID], GUILayout.Height(60), GUILayout.Width(position.width / 2f));
                if (GUILayout.Button("Keep File", GUILayout.Width(100)))
                    keepFile = true;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                //Apply the changes of the one that is kept
                string newText = "";
                if (keepSO)
                {
                    _contextLocalized[voiceLine.LocalizationID] = tempSO;
                    newText = tempSO;
                }
                else if (keepFile)
                {
                    voiceLine.Context = tempFile;
                    newText = tempFile;
                }

                //Apply the changes
                if (keepSO || keepFile)
                {
                    EditorUtility.SetDirty(voiceLine);
                    //Change the data in the file
                    if (keepSO)
                    {
                        _dataReader.ProcessDataFile();
                        _dataReader.SetContext(voiceLine.LocalizationID, newText);
                    }

                    _temporaryContext = newText;
                    EditorUtility.SetDirty(_localizationFile);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        private void _DrawVoiceLineNotes(VoiceLine voiceLine)
        {
            bool defaultTextIsIdentical = true;
            //Title of the section
            GUILayout.BeginHorizontal();
            GUILayout.Label($"Notes:", GUILayout.Width(190));
            //Display warning if the localization ID is not valid
            if (voiceLine.LocalizationID < 0)
            {
                EditorGUILayout.HelpBox("The localization ID should not be negative.", MessageType.Warning);
            }
            //Display warning if the localization file does not contain an entry with the given ID
            else if (_contextLocalized.ContainsKey(voiceLine.LocalizationID) == false)
            {
                EditorGUILayout.HelpBox($"The localization file does not contain an entry with ID ({voiceLine.LocalizationID}).", MessageType.Warning);
            }
            //If text in SO and localization file are different
            else if (voiceLine.Notes != _notesLocalized[voiceLine.LocalizationID])
            {
                EditorGUILayout.HelpBox($"Notes in the localization file and {nameof(VoiceLine)} SO do not match.", MessageType.Warning);
                defaultTextIsIdentical = false;
            }
            GUILayout.EndHorizontal();

            if (defaultTextIsIdentical)
            {
                _temporaryNotes = EditorGUILayout.TextArea(_temporaryNotes , GUILayout.Height(60));
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                bool userHasMadeChanges = _temporaryNotes != voiceLine.Notes;
                if (userHasMadeChanges == false)
                    GUI.backgroundColor = Color.grey;
                if (GUILayout.Button("Apply", GUILayout.Width(100)) && userHasMadeChanges)
                {
                    voiceLine.Notes = _temporaryNotes;
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                //Display the two texts next to each other and buttons to select if there is a difference
                bool keepSO = false;
                bool keepFile = false;
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                EditorStyles.textField.wordWrap = true;
                string tempSO = EditorGUILayout.TextArea(voiceLine.Notes, GUILayout.Height(60), GUILayout.Width(position.width / 2f));
                if (GUILayout.Button("Keep SO", GUILayout.Width(100)))
                    keepSO = true;
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                string tempFile = EditorGUILayout.TextArea(_notesLocalized[voiceLine.LocalizationID], GUILayout.Height(60), GUILayout.Width(position.width / 2f));
                if (GUILayout.Button("Keep File", GUILayout.Width(100)))
                    keepFile = true;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                //Apply the changes of the one that is kept
                string newText = "";
                if (keepSO)
                {
                    _notesLocalized[voiceLine.LocalizationID] = tempSO;
                    newText = tempSO;
                }
                else if (keepFile)
                {
                    voiceLine.Notes = tempFile;
                    newText = tempFile;
                }

                //Apply the changes
                if (keepSO || keepFile)
                {
                    EditorUtility.SetDirty(voiceLine);
                    //Change the data in the file
                    if (keepSO)
                    {
                        _dataReader.ProcessDataFile();
                        _dataReader.SetNotes(voiceLine.LocalizationID, newText);
                    }

                    _temporaryNotes = newText;
                    EditorUtility.SetDirty(_localizationFile);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }
        #endregion Draw the sections of the current voice line

        private void _DrawErrorsAndWarnings()
        {
            
        }


        private void _DrawHorizontalSeparationLine()
        {
            // Define the line texture
            Texture2D lineTexture = new Texture2D(1, 1);
            lineTexture.SetPixel(0, 0, Color.grey);
            lineTexture.Apply();

            // Draw the line
            GUILayout.Box(GUIContent.none, GUILayout.Height(1), GUILayout.ExpandWidth(true));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(), lineTexture);
        }


        private void _ReadTemporaryInfoFromVoiceLine(VoiceLine vLine)
        {
            _temporaryDefaultText = vLine.DefaultText;
            _temporaryContext = vLine.Context;
            _temporaryNotes = vLine.Notes;
        }

        public void _FindAndProcessVoiceLines()
        {
            List<VoiceLine> vLines = new List<VoiceLine>();

            // Find all VoiceLine assets in the project
            string[] guids = AssetDatabase.FindAssets("t:VoiceLine");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                VoiceLine voiceLine = AssetDatabase.LoadAssetAtPath<VoiceLine>(path);
                if (voiceLine != null)
                {
                    vLines.Add(voiceLine);
                }
            }

            //Reset the indexes
            _quickInspectIndex = 0;
            _currentVoiceLineIndex = 0;
            _totalVoiceLines = vLines.Count;
            _voiceLines = vLines;
        }

        private void _ProcessLocalizationFile(TextAsset localizationFile)
        {
            if (_localizationFile == null)
            {
                Debug.LogError("Localization file has not been assigned");
                return;
            }

            _dataReader = new LocalizationDataReader(localizationFile);
            _dataReader.ProcessDataFile();
            _defaultTextLocalized = _dataReader.GetDefaultTexts();
            _contextLocalized = _dataReader.GetContexts();
            _notesLocalized = _dataReader.GetNotes();
        }

        private void _DrawWwiseEvent()
        {
            _voiceLineObj = EditorGUILayout.ObjectField("My Scriptable Object", _voiceLineObj, typeof(VoiceLine), false) as VoiceLine;

            if (_voiceLineObj != null)
            {
                

                //serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
