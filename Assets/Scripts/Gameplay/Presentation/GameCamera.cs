
/************************************
GameCamera.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;

namespace UStacker.Gameplay.Presentation
{
    public class GameCamera : MonoBehaviour, IGameSettingsDependency
    {
        [SerializeField] private Camera _camera;
        private float _boardAspectRatio;

        private Vector2 _boardSize;
        private float _gamePadding;
        private bool _settingsSet;

        private void Update()
        {
            if (!_settingsSet)
                return;

            var cameraAspectRatio = _camera.pixelWidth / (float) _camera.pixelHeight;

            if (_boardAspectRatio <= cameraAspectRatio)
            {
                _camera.orthographicSize = _boardSize.y * .5f + _gamePadding;
            }
            else
            {
                var targetWidth = _boardSize.x + _gamePadding * 2f;
                var cameraHeight = targetWidth / cameraAspectRatio;
                _camera.orthographicSize = cameraHeight * .5f;
            }
        }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set
            {
                _settingsSet = true;
                _boardSize = new Vector2(value.BoardDimensions.BoardWidth, value.BoardDimensions.BoardHeight);
                _gamePadding = value.Presentation.GamePadding;
                if (value.Controls.AllowHold)
                    _boardSize.x += PieceContainer.WIDTH;
                if (value.General.NextPieceCount > 0)
                    _boardSize.x += PieceContainer.WIDTH;

                _boardAspectRatio = _boardSize.x / _boardSize.y;
            }
        }
    }
}
/************************************
end GameCamera.cs
*************************************/
