using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 1)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;
        Dictionary<CharacterClass, Dictionary<Stat, StatAttribute>> classLookup = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLoookup();
            StatAttribute sa = classLookup[characterClass][stat];
            if(sa == null) return 0;
            float statValue = sa.GetStartingValue();
            for (int i = 0; i < level; i++)
            {
                statValue += (i * sa.GetBaseAdded()) + (statValue * sa.GetPercentageIncrease());
            }
            return statValue;
        }

        public void BuildLoookup()
        {
            if(classLookup != null) return;
            classLookup = new Dictionary<CharacterClass, Dictionary<Stat, StatAttribute>>();
            foreach (ProgressionCharacterClass pcc in characterClasses)
            {
                Dictionary<Stat, StatAttribute> statTable = new Dictionary<Stat, StatAttribute>();
                foreach (StatAttribute sa in pcc.stats)
                {
                    statTable.Add(sa.stat, sa); 
                }
                classLookup.Add(pcc.characterClass, statTable);
            }
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public StatAttribute[] stats;
        }

        [System.Serializable]
        class StatAttribute
        {
            public Stat stat;
            [SerializeField] int startingValue;
            [SerializeField] float percentageIncrease;
            [SerializeField] int baseAdded;

            public int GetStartingValue()
            {
                return startingValue;
            }

            public float GetPercentageIncrease()
            {
                return percentageIncrease;
            }

            public float GetBaseAdded()
            {
                return baseAdded;
            }
        }
    }
}
