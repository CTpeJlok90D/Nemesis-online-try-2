using UnityEngine;

namespace Core.Scenarios
{
    public class EnableGameObjectChapter : IChapter
    {
        private GameObject _cameraMovement;
        public EnableGameObjectChapter(GameObject cameraMovement)
        {
            _cameraMovement = cameraMovement;
        }

        public event IChapter.EndedListener Ended;

        public void Begin()
        {
            _cameraMovement.gameObject.SetActive(true);
            Ended?.Invoke(this);
        }
    }
}
