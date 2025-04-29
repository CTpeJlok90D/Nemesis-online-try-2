using Core.Common;
using UnityEngine;
using Zenject;

namespace View.Rooms
{
    internal class RoomsDictionaryInstaller : MonoInstaller
    {
        [SerializeField] private GameObjectByID _roomsDictionary;

        public override void InstallBindings()
        {
            Container.Bind<GameObjectByID>().FromInstance(_roomsDictionary);
        }
    }
}
