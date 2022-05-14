using System;
using Blockstacker.Common.Extensions;
using Blockstacker.GlobalSettings;
using Blockstacker.GlobalSettings.Appliers;
using UnityEngine;

namespace Blockstacker.Gameplay
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GridPiece : MonoBehaviour
    {
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();

            _renderer.color = _renderer.color.WithAlpha(AppSettings.Gameplay.GridVisibility);

            GridVisibilityApplier.VisibilityChanged += ChangeVisibility;
        }

        private void OnDestroy()
        {
            GridVisibilityApplier.VisibilityChanged += ChangeVisibility;
        }

        private void ChangeVisibility(float newAlpha)
        {
            _renderer.color = _renderer.color.WithAlpha(newAlpha);
        }
    }
}