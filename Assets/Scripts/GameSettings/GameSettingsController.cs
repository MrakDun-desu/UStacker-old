using System;
using UnityEngine;

public class GameSettingsController : MonoBehaviour
{
    [SerializeField] private GameObject _firstOpenTab;
    [SerializeField] private GameObject[] _settingsTabs = Array.Empty<GameObject>();

    private GameObject _currentOpenTab;

    private void Start()
    {
        _currentOpenTab = _firstOpenTab;
        foreach (var settingsTab in _settingsTabs)
        {
            if (settingsTab != _firstOpenTab) settingsTab.SetActive(false);
        }
        _currentOpenTab.SetActive(true);
    }

    public void ChangeOpenTab(GameObject newTab)
    {
        _currentOpenTab.SetActive(false);
        _currentOpenTab = newTab;
        _currentOpenTab.SetActive(true);
    }
}