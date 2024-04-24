
/************************************
PlayerNameLabel.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Multiplayer.GameUi
{
    [RequireComponent(typeof(RectTransform))]
    public class PlayerNameLabel : MonoBehaviour, IGameSettingsDependency
    {
        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = (RectTransform) transform;
        }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set => _rectTransform.sizeDelta = new Vector2(value.BoardDimensions.BoardWidth, _rectTransform.sizeDelta.y);
        }
    }
}
/************************************
end PlayerNameLabel.cs
*************************************/
