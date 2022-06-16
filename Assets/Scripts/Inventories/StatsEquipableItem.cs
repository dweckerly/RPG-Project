
using System.Collections.Generic;
using UnityEngine;
using RPG.Stats;

namespace RPG.Inventories
{
    [CreateAssetMenu(menuName = ("Inventory System/Equipable Item with Stats"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] Modifier[] additiveModifiers;
        [SerializeField] Modifier[] percentageModifiers;

        [System.Serializable]
        struct Modifier
        {
            public Stat stat;
            public float value;
        }

        IEnumerable<float> IModifierProvider.GetAdditiveModifiers(Stat stat)
        { 
            foreach(Modifier mod in additiveModifiers)
            {
                if(mod.stat == stat) yield return mod.value;
            }
        }

        IEnumerable<float> IModifierProvider.GetPercentageModifiers(Stat stat)
        {
            foreach (Modifier mod in percentageModifiers)
            {
                if (mod.stat == stat) yield return mod.value;
            }
        }
    }
}

