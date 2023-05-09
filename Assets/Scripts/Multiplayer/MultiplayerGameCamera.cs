
/************************************
MultiplayerGameCamera.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UStacker.Gameplay;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Multiplayer
{
    public class MultiplayerGameCamera : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Camera _camera;
        private float _gameAspectRatio;
        private float _gamePadding;

        private Vector2 _gameSize;
        private bool _settingsSet;

        private void Update()
        {
            if (!_settingsSet)
                return;

            var cameraAspectRatio = _camera.pixelWidth / (float) _camera.pixelHeight;

            if (_gameAspectRatio <= cameraAspectRatio)
            {
                _camera.orthographicSize = _gameSize.y * .5f + _gamePadding;
            }
            else
            {
                var targetWidth = _gameSize.x + _gamePadding * 2f;
                var cameraHeight = targetWidth / cameraAspectRatio;
                _camera.orthographicSize = cameraHeight * .5f;
            }
        }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set
            {
                _settingsSet = true;
                _gameSize = new Vector2(value.BoardDimensions.BoardWidth * 2, value.BoardDimensions.BoardHeight);
                _gamePadding = value.Presentation.GamePadding;
                if (value.Controls.AllowHold)
                    _gameSize.x += PieceContainer.WIDTH * 2;
                if (value.General.NextPieceCount > 0)
                    _gameSize.x += PieceContainer.WIDTH * 2;

                _gameAspectRatio = _gameSize.x / _gameSize.y;
            }
        }
    }
}
/************************************
end MultiplayerGameCamera.cs
*************************************/
