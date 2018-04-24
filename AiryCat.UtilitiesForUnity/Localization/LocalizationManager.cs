using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AiryCat.UtilitiesForUnity.Localization
{
    public static class LocalizationManager
    {
        #region Private Variable

        private static Dictionary<string, string> _localizedText;
        private const string MISSING_TEXT_STRING = "Localized text not found";

        /*
         * Multiple paths to files with localization
         *  {
         *      "Localization/ui/",
         *      "Localization/log/",
         *      "Localization/dialog/",
         *  }
         */
        private static readonly string[] PathToFileLang =
        {
            "Localization/"
        };

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
        private struct LocalizationData
        {
            public LocalizationItem[] Items;
        }

        [Serializable]
        private struct LocalizationItem
        {
            public string Key;
            public string Value;
        }

        #endregion

        #region Events

        public static Action SwitchLangEvent;

        #endregion

        #region Public static method
        /// <summary>
        /// Returns a localized string.
        /// </summary>
        public static string Localized(this string key, TextStyle style = TextStyle.AsItIsNow, params object[] args)
        {
            if (_localizedText == null || _localizedText.Count < 2)
            {
                LoadLocalizedText();
            }

            if (!_localizedText.ContainsKey(key)) return MISSING_TEXT_STRING;
            var result = args.Length < 1 ? _localizedText[key] : string.Format(_localizedText[key], args);

            return FormatingText(result, style);
        }

        public static void LoadLocalizedText(string lang = "")
        {
            _localizedText = new Dictionary<string, string>();

            if (lang.Length < 2)
            {
                lang = Application.systemLanguage == SystemLanguage.Russian ? "Rus" : "Eng";
            }
            foreach (var it in PathToFileLang)
            {
                // This variable is only needed to output the log.
                // If this bothers you or does not require log - delete.
                var itemCount = 0;
                // This "if" accelerates the loading of the first or only dictionary.
                if (_localizedText.Count < 1)
                {
                    foreach (var value in GetLocalization(it + lang))
                    {
                        _localizedText.Add(value[0], value[1]);
                        itemCount++;
                    }
                }
                else
                {
                    foreach (var value in GetLocalization(it + lang))
                    {
                        if (_localizedText.ContainsKey(value[0]))
                        {
                            Debug.LogWarning($"[Localization] Dublicate key found: {value[0]}");
                            continue;
                        }
                        _localizedText.Add(value[0], value[1]);
                        itemCount++;
                    }
                }

                if (itemCount > 0)
                {
                    Debug.Log($"[Localization] Data loaded, dictionary {it + lang} contains: {itemCount} entries");
                }
            }
            SwitchLangEvent?.Invoke();
        }

        #endregion

        #region Private static method

        private static List<string[]> GetLocalization(string pathFileName)
        {
            var result = new List<string[]>();

            var assets = Resources.LoadAll<TextAsset>(pathFileName);
            Resources.UnloadUnusedAssets();

            if (assets == null || assets.Length == 0)
            {
                Debug.LogError($"[Localization] File: {pathFileName}  not found.");
                return null;
            }

            foreach (var data in assets)
            {
                var loadedData = JsonUtility.FromJson<LocalizationData>(data.text);
                result.AddRange(loadedData.Items.Select(t => new[] { t.Key, t.Value }));
            }
            return result;
        }

        private static string FormatingText(string text, TextStyle style)
        {
            var result = text;
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
        #endregion
    }
}