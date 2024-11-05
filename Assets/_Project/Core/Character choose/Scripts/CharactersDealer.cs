using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Players;
using Unity.Netcode;
using UnityEngine;
using CharacterReference = UnityEngine.AddressableAssets.AssetReferenceT<Core.Characters.Character>;

namespace Core.CharacterChoose
{
    public class CharactersDealer : NetworkBehaviour
    {
        public delegate void СhoiceIsProvidedListener();

        [SerializeField] private CharacterReference[] _allCharacters;

        private List<CharacterReference> _characters;

        public event СhoiceIsProvidedListener ChoiceIsProvided;

        private void Awake()
        {
            _characters = _allCharacters.ToList();
        }

        public async Task StartDeal()
        {            
            
        }

        private CharacterReference[] GetRandomCharacters(int count)
        {
            List<CharacterReference> result = _characters.ToList();

            if (result.Count < count)
            {
                return result.ToArray();
            }

            while (result.Count != count)
            {
                result.Remove(result[Random.Range(0, result.Count)]);
            }

            return result.ToArray();
        }

        private void RemoveCharacter(CharacterReference character)
        {
            _characters.Remove(character);
        }
    }
}
