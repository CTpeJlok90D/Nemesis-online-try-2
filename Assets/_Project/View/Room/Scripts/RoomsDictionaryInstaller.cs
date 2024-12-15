using Core.Common;
using UnityEngine;
using Zenject;

namespace View.Rooms
{
    internal class RoomsDictionaryInstaller : MonoInstaller
    {
        [SerializeField] private GameObjectsDictionary _roomsDictionary;

        public override void InstallBindings()
        {
            Container.Bind<GameObjectsDictionary>().FromInstance(_roomsDictionary);
        }
    }
}
