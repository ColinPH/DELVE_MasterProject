using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public class LocalizationManager : ManagerBase
    {
        public static LocalizationManager Instance { get; private set; }

        [SerializeField] Object _localizationDataFile;
        [SerializeField] LanguageType _currentLanguage = LanguageType.English;

        LocalizationDataReader _localizationDataReader;

        #region Initialization
        protected override void MonoAwake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }
        public override void Init()
        {
            Debugger.LogInit("Init in localization manager");
            Managers.localizationManager = this;
        }
        public override void MyAwake()
        {
            Debugger.LogInit("MyAwake in localization manager");
            _localizationDataReader = new LocalizationDataReader(_localizationDataFile);
            _localizationDataReader.ProcessDataFile();
        }
        public override void MyStart()
        {
            Debugger.LogInit("MyStart in localization manager");
            //TODO use the language from the save file
        }
        public override bool AlreadyExistsInScene(out ManagerBase existingManager)
        {
            existingManager = FindObjectOfType<LocalizationManager>();

            if (existingManager == null)
                return false;

            if (existingManager.GetType() != this.GetType())
                Debug.LogError("Check manager name ! Be careful when copy-pasting ;)" +
                    " A " + existingManager.GetType().ToString() +
                    " is not a " + this.GetType().ToString());

            return existingManager != null;
        }
        #endregion

        public string GetLocalizedText(TextClip textClip)
        {
            if (_localizationDataReader == null || textClip.LocalizationID == -1)
            {
                return textClip.DefaultText;
            }
            //TODO Check if the ID is valid
            
            //TODO return default text if ID is not valid

            //TODO If a different language than default is selected, read the localization datafile and return the correct version

            try
            {
                return _localizationDataReader.SampleLanguageData(_currentLanguage, textClip.LocalizationID);
            }
            catch
            {
                return textClip.DefaultText;
            }
        }
    }
}