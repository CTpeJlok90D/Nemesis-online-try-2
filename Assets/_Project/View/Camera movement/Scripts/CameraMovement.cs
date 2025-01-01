using UnityEngine;

namespace View
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private Transform _cameraRotationCenter;
        [SerializeField] private float _yPosition;
        [SerializeField] private Vector2 _borders;

        public (float min, float max) XBorders => (transform.position.x - _borders.x/2, transform.position.x + _borders.x/2);
        public (float min, float max) ZBorders => (transform.position.z - _borders.y/2, transform.position.z + _borders.y/2);

        public void Move(Vector2 offcet)
        {
            Vector3 position = _cameraRotationCenter.transform.position;

            position += new Vector3(offcet.x, _yPosition, offcet.y);

            position = new Vector3(
                Mathf.Clamp(position.x, XBorders.min, XBorders.max),
                _yPosition,
                Mathf.Clamp(position.z, ZBorders.min, ZBorders.max)
                );

            _cameraRotationCenter.position = position;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            Vector3 borders = new(_borders.x, 0, _borders.y);

            Gizmos.DrawWireCube(transform.position, borders);
        }
#endif
    }
}
