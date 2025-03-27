using System.Linq;
using System.Threading.Tasks;
using Core.ActionsCards;
using Core.Maps;
using UnityEditor;

namespace Core.PlayerActions
{
    public interface IGameActionWithPayment
    {
        public int RequaredPaymentCount { get; }
    }

    public interface INeedNoiseContainers
    {
        public INoiseContainer[] SelectedNoiseContainers { get; set; }

        public INoiseContainer[] NoiseContainerSelectionSource { get; }
        
        public int RequiredNoiseContainerCount { get; }
    }
}
