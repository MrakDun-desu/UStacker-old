using UnityEngine;
using UnityEngine.UI;

namespace Blockstacker.Common.Extensions
{
    public static class ScrollRectExtensions
    {
        public static void ScrollTo(this ScrollRect self, 
            RectTransform target, 
            bool minimalMovement = true,
            bool scrollSmooth = true, 
            float time = 0.5f, 
            AnimationCurve easeCurve = null)
        {
            var contentPanel = self.content;

            var targetRect = target.GetWorldSpaceRect();
            var targetPos = new Vector2();
            if (minimalMovement)
            {
                var viewportRect = self.viewport.GetWorldSpaceRect();

                var setX = true;
                var setY = true;

                if (targetRect.xMin < viewportRect.xMin)
                    targetPos.x = targetRect.xMin;
                else if (targetRect.xMax > viewportRect.xMax)
                {
                    targetPos.x = targetRect.xMax + viewportRect.width - targetRect.width;
                }
                else setX = false;

                if (targetRect.yMin < viewportRect.yMin)
                {
                    targetPos.y = targetRect.yMin + viewportRect.height - targetRect.height;
                }
                else if (targetRect.yMax > viewportRect.yMax)
                    targetPos.y = targetRect.yMax;
                else setY = false;

                if (!setX && !setY) return;
            }
            else targetPos = targetRect.max;

            Vector2 contentInverse = self.transform.InverseTransformPoint(contentPanel.position);
            Vector2 targetInverse = self.transform.InverseTransformPoint(targetPos);

            var newAnchoredPos = contentInverse - targetInverse;

            var currentAnchoredPos = contentPanel.anchoredPosition;
            newAnchoredPos = self switch
            {
                {horizontal: true, vertical: false} => new Vector2(newAnchoredPos.x, currentAnchoredPos.y),
                {horizontal: false, vertical: true} => new Vector2(currentAnchoredPos.x, newAnchoredPos.y),
                _ => newAnchoredPos
            };

            if (!scrollSmooth)
            {
                contentPanel.anchoredPosition = newAnchoredPos;
                return;
            }

            var posDifference = newAnchoredPos - currentAnchoredPos;

            var newPos = contentPanel.localPosition + (Vector3) posDifference;
            if (easeCurve is null)
                LeanTween.move(contentPanel, newPos, time).setEaseInOutSine();
            else
                LeanTween.move(contentPanel, newPos, time).setEase(easeCurve);
        }
    }
}