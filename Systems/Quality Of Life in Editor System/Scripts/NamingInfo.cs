using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PropellerCap.EditorUsability
{
    public class NamingInfo
    {
        public string prefix = "";
        public string baseName = "";
        public List<string> specifications = new List<string>();
        public string variant = "";
        public string suffix = "";

        private bool _nameIsValid = true;
        private string _split;
        public NamingInfo(string rawName, string splitCharacter = "Default Settings")
        {
            if (splitCharacter == "Default Settings")
                _split = Utils.FindQOLSettings().namingConventionsSettings.textSplitter;
            else
                _split = splitCharacter;

            //cut the name into pieces and sort them
            string[] words = rawName.Split(_split);

            if (words.Length == 1)
            {
                //Here there is nothing separated by underscores so we keep all the information in the name
                baseName = rawName;
                return;
            }

            prefix = words[0];
            baseName = words[1];
            /*string tempSuffix = words[words.Length - 1];
            if (Utils.FindQOLSettings().namingConventionsSettings.IsSuffix(tempSuffix))
                suffix = tempSuffix;*/

            int index = 0;
            foreach (string word in words)
            {
                if (index == 0 || index == 1) //We don't want the prefix or name
                {
                    index++;
                    continue;
                }

                //Current word is last or second to last in array AND can be parsed into a number
                if (_IsVariationIndex(word, index, words.Length))
                {
                    variant = word;
                    break;
                }
                else
                {
                    //Otherwise we are a specification word
                    if (index == words.Length - 1)
                    {
                        //If we are the last word in the name
                        if (Utils.FindQOLSettings().namingConventionsSettings.IsSuffix(word))
                            suffix = word;
                        else
                            specifications.Add(word);
                    }
                    else
                        specifications.Add(word);
                }

                index++;
            }
        }

        public bool NameIsValid()
        {
            return _nameIsValid;
        }

        public string GetNameWithoutSuffix()
        {
            string toReturn = prefix + _split + baseName;

            foreach (string word in specifications)
            {
                toReturn += _split + word;
            }

            if (variant != "")
                toReturn += _split + variant;

            return toReturn;
        }

        public string GetNameWithOutPrefix()
        {
            string toReturn = baseName;

            foreach (string word in specifications)
            {
                toReturn += _split + word;
            }

            if (variant != "")
                toReturn += _split + variant;
            if (suffix != "")
                toReturn += _split + suffix;

            return toReturn;
        }

        public void DebugName()
        {
            Debug.Log("Full name is " + GetFullName());
            Debug.Log("Prefix is " + prefix);
            Debug.Log("Base name is " + baseName);
            foreach (string word in specifications)
            {
                Debug.Log("Specification " + (specifications.IndexOf(word) + 1) + " is " + word);
            }
            Debug.Log("Variant number is " + variant);
            Debug.Log("Suffix is " + suffix);
        }

        public string GetFullName()
        {
            string toReturn = prefix + _split + baseName;

            foreach (string word in specifications)
            {
                toReturn += _split + word;
            }

            if (variant != "")
                toReturn += _split + variant;
            if (suffix != "")
                toReturn += _split + suffix;

            return toReturn;
        }

        private bool _CanParse(string text)
        {
            int result;
            return int.TryParse(text, out result);
        }

        private bool _IsVariationIndex(string text, int index, int length)
        {
            if (_CanParse(text) && (index == length - 2 || index == length - 1))
            {
                return true;
            }
            else
                return false;
        }
    }
}