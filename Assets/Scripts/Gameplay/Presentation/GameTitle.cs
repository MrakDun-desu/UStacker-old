
/************************************
GameTitle.cs -- created by Marek Dančo (xdanco00)
*************************************/
using TMPro;
using UnityEngine;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Gameplay.Presentation
{
    public class GameTitle : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private TMP_Text _titleLabel;

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set => _titleLabel.text = value.Presentation.Title;
        }
    }
}
/************************************
end GameTitle.cs
*************************************/
