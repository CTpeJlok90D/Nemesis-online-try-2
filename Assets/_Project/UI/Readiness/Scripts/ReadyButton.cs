using Core.Players;
using Core.Readiness;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Readiness
{
    public class ReadyButton : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            Preparation preparation = Player.Local.GetComponent<Preparation>();
            preparation.IsReady.Value = preparation.IsReady.Value == false;
        }
    }
}
