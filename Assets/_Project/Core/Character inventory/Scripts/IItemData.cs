using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Core.CharacterInventorys
{
    public interface IItemData
    {
        public SerializedDictionary<string, string> StartItemData { get; }
    }
}
