
/************************************
GameSettingsController.cs -- created by Marek Dančo (xdanco00)
*************************************/
using System;
using UnityEngine;

// TODO rename to represent functionality and move to the UStacker.Common namespace
public class GameSettingsController : MonoBehaviour
{
    [SerializeField] private GameObject _firstOpenTab;
    [SerializeField] private GameObject[] _settingsTabs = Array.Empty<GameObject>();

    private GameObject _currentOpenTab;

    private void Start()
    {
        foreach (var settingsTab in _settingsTabs)
            settingsTab.SetActive(settingsTab == _firstOpenTab);

        _currentOpenTab = _firstOpenTab;
    }

    public void ChangeOpenTab(GameObject newTab)
    {
        _currentOpenTab.SetActive(false);
        _currentOpenTab = newTab;
        _currentOpenTab.SetActive(true);
    }
}
/************************************
end GameSettingsController.cs
*************************************/
