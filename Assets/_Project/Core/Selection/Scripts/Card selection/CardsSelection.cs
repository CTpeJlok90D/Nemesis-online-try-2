using System;
using System.Linq;
using Core.ActionsCards;
using Core.SelectionBase;
using UnityEngine;
using Zenject;

namespace Core.Selection.Cards
{
    public class CardsSelection : Selection<ActionCard>
    {
        public override bool OnlyUniqueItems => false;
    }
}
