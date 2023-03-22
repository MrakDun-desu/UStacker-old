using UnityEngine;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Gameplay.Presentation
{
    public class GameCamera : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Camera _camera;
        
        public GameSettingsSO.SettingsContainer GameSettings
        {
            set => _camera.orthographicSize = value.BoardDimensions.BoardHeight * .5f + value.BoardDimensions.BoardPadding;
        }
    }
}