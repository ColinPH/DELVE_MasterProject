using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PropellerCap
{
    public static class HUD
    {
        static UIManager _UIManager;
        public static UIManager uiManager
        {
            get
            {
                if (_UIManager == null)
                    _UIManager = Managers.uiManager;
                return _UIManager;
            }
            set
            {
                _UIManager = value;
            }
        }
        public static FlareChargesVisualizer flareChargesVisualizer { get; set; }
        public static HookCrosshair crosshair { get; set; }
        public static SanityBar sanityBar { get; set; }
        public static CustomPassHandler customPassHandler { get; set; }
        public static BlackFader blackFader { get; set; }
        public static SubtitlesControler subtitlesControler { get; set; }
    }
}