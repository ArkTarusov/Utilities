using UnityEngine;
using UnityEngine.UI;

namespace AiryCat.UtilitiesForUnity.Localization
{
    [RequireComponent(typeof(Text))]
    public class LocalizedText : MonoBehaviour
    {

        [SerializeField] private string _key;
        [SerializeField] private LocalizationManager.TextStyle _style = LocalizationManager.TextStyle.AsItIsNow;
        private Text _text;

        private void Start()
        {
            _text = GetComponent<Text>();
            SetText();
        }

        private void SetText()
        {
            _text.text = LocalizationManager.Localized(_key, style: _style);
        }
    }
}