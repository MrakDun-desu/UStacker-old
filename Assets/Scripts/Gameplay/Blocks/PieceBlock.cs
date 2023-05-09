
/************************************
PieceBlock.cs -- created by Marek Dančo (xdanco00)
*************************************/
using UnityEngine;

namespace UStacker.Gameplay.Blocks
{
    public class PieceBlock : ClearableBlock
    {
        [SerializeField] private Vector2 _initialPosition;

        private void OnValidate()
        {
            ResetPosition();
        }

        public void ResetPosition()
        {
            var myTransform = transform;
            myTransform.localPosition = new Vector3(
                _initialPosition.x,
                _initialPosition.y,
                myTransform.localPosition.z
            );
        }

        [ContextMenu("Set initial position")]
        private void SetInitialPosition()
        {
            var localPosition = transform.localPosition;
            _initialPosition.x = localPosition.x;
            _initialPosition.y = localPosition.y;
        }
    }
}
/************************************
end PieceBlock.cs
*************************************/
