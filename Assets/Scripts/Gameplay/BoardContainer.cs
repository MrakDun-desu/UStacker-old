
/************************************
BoardContainer.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;
using UnityEngine.InputSystem;
using UStacker.Gameplay.Initialization;
using UStacker.GameSettings;
using UStacker.GlobalSettings;
using UStacker.GlobalSettings.Appliers;

namespace UStacker.Gameplay
{
    public class BoardContainer : MonoBehaviour, IGameSettingsDependency
    {
        [Tooltip("Zoom percentage change with one scroll unit")] [Range(0, 1)] [SerializeField]
        private float _boardZoomFactor = .05f;

        [Range(0.00001f, 1)] [SerializeField] private float _minimumBoardScale = 0.1f;
        [SerializeField] private Camera _camera;

        private Vector3 _dragStartPosition;
        private Vector3 _dragStartTransformPosition;
        private uint _height;
        private Vector3 _offset;

        private uint _width;

        private uint Width
        {
            get => _width;
            set
            {
                _width = value;
                var mytransform = transform;
                var myPos = mytransform.position;
                mytransform.position =
                    new Vector3(-value * .5f * mytransform.localScale.x + _offset.x, myPos.y, myPos.z);
            }
        }

        private uint Height
        {
            get => _height;
            set
            {
                _height = value;
                var mytransform = transform;
                var myPos = mytransform.position;
                mytransform.position =
                    new Vector3(myPos.x, -value * .5f * mytransform.localScale.y + _offset.y, myPos.z);
            }
        }

        private Vector2 CurrentOffset => new(
            transform.position.x + Width * .5f * transform.localScale.x,
            transform.position.y + Height * .5f * transform.localScale.y
        );

        private void Awake()
        {
            _offset = AppSettings.Gameplay.BoardOffset;
            transform.position += _offset;

            ChangeBoardZoom(AppSettings.Gameplay.BoardZoom);
            BoardZoomApplier.BoardZoomChanged += ChangeBoardZoom;
        }

        private void Update()
        {
            HandleBoardZooming();
            HandleBoardDrag();
        }

        private void OnDestroy()
        {
            BoardZoomApplier.BoardZoomChanged -= ChangeBoardZoom;
        }

        public GameSettingsSO.SettingsContainer GameSettings
        {
            set
            {
                Height = value.BoardDimensions.BoardHeight;
                Width = value.BoardDimensions.BoardWidth;
            }
        }

        private void HandleBoardZooming()
        {
            const float ONE_SCROLL_UNIT = 1 / 120f;

            if (!AppSettings.Gameplay.CtrlScrollToChangeBoardZoom) return;

            if (!Keyboard.current.ctrlKey.isPressed) return;

            var mouseScroll = Mouse.current.scroll.ReadValue().y * ONE_SCROLL_UNIT;
            var newScale = transform.localScale.x + mouseScroll * _boardZoomFactor;
            var newZoom = newScale < _minimumBoardScale ? _minimumBoardScale : newScale;
            ChangeBoardZoom(newZoom);
            AppSettings.Gameplay.BoardZoom = newZoom;
            AppSettings.Gameplay.BoardOffset = CurrentOffset;
        }

        private void HandleBoardDrag()
        {
            if (!AppSettings.Gameplay.DragMiddleButtonToRepositionBoard) return;

            var mouse = Mouse.current;
            var middleButton = mouse.middleButton;
            if (middleButton.wasPressedThisFrame)
            {
                _dragStartPosition = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
                _dragStartTransformPosition = transform.position;
            }
            else if (middleButton.isPressed)
            {
                var currentPosition = _camera.ScreenToWorldPoint(mouse.position.ReadValue());
                var positionDifference = currentPosition - _dragStartPosition;
                transform.position = _dragStartTransformPosition + positionDifference;
                AppSettings.Gameplay.BoardOffset = CurrentOffset;
                _offset = CurrentOffset;
            }
        }

        private void ChangeBoardZoom(float zoom)
        {
            if (Mathf.Abs(zoom - transform.localScale.x) < float.Epsilon) return;
            var mytransform = transform;
            mytransform.localScale = new Vector3(zoom, zoom, 1);
            mytransform.position = new Vector3(-zoom * .5f * Width, -zoom * .5f * Height, 1);
        }
    }
}
/************************************
end BoardContainer.cs
*************************************/
