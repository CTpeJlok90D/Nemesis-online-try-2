using System;
using Core;
using Core.Missions;
using UnityEngine;

namespace UI
{
    public class MissionContainer : MonoBehaviour, IContainsMission
    {
        public ReactiveField<Mission> MissionReference;
        public Mission Mission => MissionReference.Value;

        public MissionContainer Instantiate(Mission mission, Transform parent = null)
        {
            gameObject.SetActive(false);
            MissionContainer result = Instantiate(this, parent);
            gameObject.SetActive(true);

            result.MissionReference = new ReactiveField<Mission>(mission);
            result.gameObject.SetActive(true);
            
            return result;
        }
    }
}