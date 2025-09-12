using UnityEngine;

namespace UI
{
    public class RectBorders : MonoBehaviour
    {
        [SerializeField] private RectTransform _target;
        [SerializeField] private RectTransform _moveBorder;

        private Vector2 _pointerOffset;
        private Rect _moveBorderRect;

        public void Init(RectTransform target, RectTransform moveBorder)
        {
            _target = target;
            _moveBorder = moveBorder;
            InitializeBorderRect();
        }

        private void InitializeBorderRect()
        {
            Vector3[] corners = new Vector3[4];
            _moveBorder.GetWorldCorners(corners);
            
            for (int i = 0; i < 4; i++)
                corners[i] = _target.parent.InverseTransformPoint(corners[i]);

            Vector2 min = corners[0];
            Vector2 max = corners[0];

            for (int i = 1; i < 4; i++)
            {
                min = Vector2.Min(min, corners[i]);
                max = Vector2.Max(max, corners[i]);
            }

            _moveBorderRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            _target.pivot = new Vector2(0.5f, 0.5f);
        }

        private void Start()
        {
            KeepInBorders();
        }

        private void KeepInBorders()
        {
            Vector3 localPosition = _target.localPosition;

            Vector3[] targetCorners = new Vector3[4];
            _target.GetWorldCorners(targetCorners);
                    
            for (int i = 0; i < 4; i++)
                targetCorners[i] = _target.parent.InverseTransformPoint(targetCorners[i]);
                    
            float halfWidth  = (targetCorners[2].x - targetCorners[0].x) * 0.5f;
            float halfHeight = (targetCorners[2].y - targetCorners[0].y) * 0.5f;

            localPosition.x = Mathf.Clamp(localPosition.x, _moveBorderRect.xMin + halfWidth, _moveBorderRect.xMax - halfWidth);
            localPosition.y = Mathf.Clamp(localPosition.y, _moveBorderRect.yMin + halfHeight, _moveBorderRect.yMax - halfHeight);

            _target.localPosition = localPosition;
        }
    }
}