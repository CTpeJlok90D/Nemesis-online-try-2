using System.Linq;
using System.Threading;
using Core.Characters.Health;
using Core.Common;
using Core.PlayerTablets;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LocalPlayerHeavyIngureIndicator : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private SpriteByID _sprites;
        [SerializeField] private Sprite _noIngureImage;
        [SerializeField] private Sprite _treatedHeavyInjureImage;
        [SerializeField] private int _heavyInjureIndex = 0;

        private CancellationTokenSource _cancellationTokenSource;
        private CharacterHealth Health => PlayerTablet.Local.CharacterPawn.Health;

        private void Awake()
        {
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private void OnEnable()
        {
            _ = UpdateActive();
            Health.HeavyDamagesCountChanged += OnHeavyDamagesCountChanged;
            
        }

        private void OnDisable()
        {
            if (PlayerTablet.Local != null)
            { 
                Health.HeavyDamagesCountChanged -= OnHeavyDamagesCountChanged;
            }
        }

        private void OnDestroy()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
        }

        private void OnHeavyDamagesCountChanged(NetworkListEvent<NetworkObjectReference> changeEvent) => _ = UpdateActive();

        private async UniTask UpdateActive()
        {
            if (Health.HeavyDamages.Count() <= _heavyInjureIndex)
            {
                _image.sprite = _noIngureImage;
                return;
            }

            HeavyDamage heavyDamage = Health.HeavyDamages.ElementAt(_heavyInjureIndex);
            if (heavyDamage.IsTreated.Value)
            {
                _image.sprite = _treatedHeavyInjureImage;
                return;
            }

            _image.sprite = await _sprites.LoadAssetAsync(heavyDamage.ID, _cancellationTokenSource.Token);
        }
    }
}