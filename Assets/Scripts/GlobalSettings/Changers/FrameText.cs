using System;
using System.Globalization;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class FrameText : MonoBehaviour
{
    [SerializeField] private float _framesPerSecond = 60;

    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    public void ShowFrames(float source)
    {
        _text.text = Math.Round(source * _framesPerSecond, 2).ToString(CultureInfo.InvariantCulture) + "F";
    }
}