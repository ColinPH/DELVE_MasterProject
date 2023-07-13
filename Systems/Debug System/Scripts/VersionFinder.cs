using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

namespace PropellerCap
{
    public class VersionFinder : MonoBehaviour
    {
        public TMP_Text textComponent;
        public string textPreVersion = "Version ";
        // Start is called before the first frame update


        void Start()
        {

            string date = DateTime.Now.ToString("dd/MM/yyyy");//.Replace("/", "-");
            textComponent.text = textPreVersion + Application.version + "_" + date;
        }


    }
}