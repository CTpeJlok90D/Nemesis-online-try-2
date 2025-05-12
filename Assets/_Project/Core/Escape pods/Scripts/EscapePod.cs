using Core.Entities;
using Unity.Netcode.Custom;
using UnityEngine;

namespace Core.EscapePods
{
    [Icon("Assets/_Project/Core/Escape pods/Editor/icons8-escape-96.png")]
    public class EscapePod : NetEntity<EscapePod>
    {
        [field: SerializeField] public EscapeZone EscapeZone { get; private set; }
        [field: SerializeField] public int Number { get; private set; }
        [field: SerializeField] private int _defualtFreePlaces = 2;

        private NetVariable<bool> _isLocked;
        private NetVariable<int> _freePlaces;

        private void Awake()
        {
            _isLocked = new(false);
            _freePlaces = new(_defualtFreePlaces);
        }
    }
}