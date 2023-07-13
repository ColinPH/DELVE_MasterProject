using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PropellerCap.QA
{
    public class FramePerSecondsCanvasDisplay : MonoBehaviour
    {
        [Header("")]
        [SerializeField] TMP_Text _fpsTitleText;
        [SerializeField] TMP_Text _fpsText;
        [Header("")]
        [SerializeField] TMP_Text _msTitleText;
        [SerializeField] TMP_Text _msText;
        [Header("")]
        [SerializeField] TMP_Text _averageFPSTitleText;
        [SerializeField] TMP_Text _averageFPSText;
        [Header("")]
        [SerializeField] TMP_Text _lowestMsTitleText;
        [SerializeField] TMP_Text _lowestMsText;
        [Header("")]
        [SerializeField] TMP_Text _highestMsTitleText;
        [SerializeField] TMP_Text _highestMsText;
        [Header("Interactables")]
        [SerializeField] Toggle _autoRefreshToggle;
        [SerializeField] Button _manualRefreshButton;

        FrameRateProcessor _fpsProcessor;

        public void InitializeCanvasDisplay(FrameRateProcessor fpsProcessor)
        {
            _fpsProcessor = fpsProcessor;
        }

        public void UpdateCanvasInformation()
        {
            //Update current fps
            int fps = _fpsProcessor.latestFPS;
            _fpsText.color = _fpsProcessor.GetFPSColour(fps);
            _fpsText.text = fps.ToString();

            //Update the last delta time in milli seconds
            float ms = _fpsProcessor.latestDelta;
            _msText.color = _fpsProcessor.GetMilliSecondsColor(ms);
            _msText.text = (ms * 1000f).ToString("00.00");

            //Update the average fps
            int average = _fpsProcessor.averageFPS;
            _averageFPSText.color = _fpsProcessor.GetFPSColour(average);
            _averageFPSText.text = average.ToString();

            //Update the lowest ms
            float lowestMs = _fpsProcessor.lowestDelta;
            _lowestMsText.color = _fpsProcessor.GetMilliSecondsColor(lowestMs);
            _lowestMsText.text = (lowestMs * 1000f).ToString("00.00");

            //Update the highest ms
            float highestMs = _fpsProcessor.highestDelta;
            _highestMsText.color = _fpsProcessor.GetMilliSecondsColor(highestMs);
            _highestMsText.text = (highestMs * 1000f).ToString("00.00");
        }

        public void OnDebugConsoleShow()
        {
            //Hook the button for the manual refresh of the min and max ms
            _manualRefreshButton.onClick.AddListener(() =>
            {
                _fpsProcessor.ResetMinMaxMSValues();
                UpdateCanvasInformation();
            });

            //Hook the togle for the auto refresh of the min and max ms
            _autoRefreshToggle.onValueChanged.AddListener((selected) =>
            {
                _fpsProcessor.resetMinMaxAfterSeconds = selected;
                UpdateCanvasInformation();
            });
        }

        public void OnDebugConsoleHide()
        {

        }
    }
}
