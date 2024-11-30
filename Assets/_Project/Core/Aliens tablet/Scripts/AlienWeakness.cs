using UnityEngine;

namespace Core.AliensTablets
{
    [Icon("Assets/_Project/Core/Aliens tablet/Editor/broken-link.png")]
    public abstract class AlienWeakness : ScriptableObject
    {
        public abstract bool IsActive { get; set; }
    }
}
