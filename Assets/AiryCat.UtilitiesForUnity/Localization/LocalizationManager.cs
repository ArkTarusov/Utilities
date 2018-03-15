using System;
using System.Collections.Generic;
using UnityEngine;

namespace AiryCat.UtilitiesForUnity.Localization
{
    public static class LocalizationManager
    {
        #region Private Variable

        private static Dictionary<string, string> _localizedText;
        private const string PATH_TO_FILE_LANG = "Localization/";
        private const string MISSING_TEXT_STRING = "Localized text not found";

        #endregion

        #region Public class

        [Serializable]
        public enum TextStyle
        {
            AllUpper,
            AllLower,
            FirstToUpperRestToLower,
            AsItIsNow
        }

        #endregion

        #region Private class

        [Serializable]
        private class LocalizationData
        {
            public LocalizationItem[] items;

        }

        [Serializable]
        private class LocalizationItem
        {
            public string key;
            public string value;
        }

        #endregion

        #region Events

        public static Action SwitchLangEvent;

        #endregion

        #region Public static method

        public static string GetLocalizedValue(string key, object[] args = null, TextStyle style = TextStyle.AsItIsNow)
        {
            if (_localizedText == null || _localizedText.Count < 2)
            {
                LoadLocalizedText();
            }
            if (!_localizedText.ContainsKey(key)) return MISSING_TEXT_STRING;

            string result;
            if (args != null && args.Length > 0)
            {
                result = string.Format(_localizedText[key], args);
            }
            else
            {
                result = _localizedText[key];
            }

            switch (style)
            {
                case TextStyle.AsItIsNow:
                    break;
                case TextStyle.AllUpper:
                    result = result.ToUpperInvariant();
                    break;
                case TextStyle.AllLower:
                    result = result.ToLowerInvariant();
                    break;
                case TextStyle.FirstToUpperRestToLower:
                    result = result.ToLowerInvariant();
                    result = result.Substring(0, 1).ToUpper() + result.Remove(0, 1);
                    break;
                default:
                    break;
            }
            return result;
        }

        public static void LoadLocalizedText(string fileName = "")
        {
            if (fileName.Length < 2)
            {
                fileName = Application.systemLanguage == SystemLanguage.Russian ? "Rus" : "Eng";
            }
            _localizedText = GetLocalization(fileName);

            if (_localizedText != null && _localizedText.Count > 1)
            {
                Debug.Log("[Localization] Data loaded, dictionary contains: " + _localizedText.Count + " entries");
                SwitchLangEvent?.Invoke();
            }
            else
            {
                Debug.LogError("[Localization] Cannot find file!");
            }

        }

        #endregion

        #region Private static method

        private static Dictionary<string, string> GetLocalization(string fileName)
        {
            var result = new Dictionary<string, string>();

            var assets = Resources.LoadAll<TextAsset>(PATH_TO_FILE_LANG + fileName);
            Resources.UnloadUnusedAssets();

            if (assets == null || assets.Length == 0)
            {
                Debug.LogError($"[Localization] File: {PATH_TO_FILE_LANG + fileName}  not found.");
                return null;
            }

            foreach (var data in assets)
            {
                Debug.Log(data);
                var loadedData = JsonUtility.FromJson<LocalizationData>(data.text);
                Debug.Log(loadedData.ToString());
                Debug.Log(loadedData.items.Length);
                for (int i = 0; i < loadedData.items.Length; i++)
                {
                    if (result.ContainsKey(loadedData.items[i].key))
                    {
                        Debug.LogWarning($"[Localization] Duplicate found: {loadedData.items[i].key}");
                        continue;
                    }
                    result.Add(loadedData.items[i].key, loadedData.items[i].value);
                }

            }
            return result;
        }

        #endregion
    }
}