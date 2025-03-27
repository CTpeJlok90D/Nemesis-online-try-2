using System;
using Core.PlayerActions;
using TMPro;
using UnityEngine;

namespace UI.GameActions.MoveActionButtons
{
    public class ActionPrice : MonoBehaviour
    {
        [SerializeField] private TMP_Text _caption;
        [SerializeField] private GameActionContainer _gameActionContainer;

        private void OnEnable()
        {
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            if (_gameActionContainer.GameAction.Value is IGameActionWithPayment gameActionWithPayment)
            {
                _caption.text = gameActionWithPayment.RequaredPaymentCount.ToString();
            }
            else
            {
                _caption.text = "0";
            }
        }
    }
}
