using System;
using UnityEngine;
using UnityEngine.UI;

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
        _text.text = LocalizationManager.GetLocalizedValue(_key, style: _style);
    }
}