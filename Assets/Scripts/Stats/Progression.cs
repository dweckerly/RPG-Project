using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 1)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            foreach(ProgressionCharacterClass pcc in characterClasses) 
            {
                if(pcc.characterClass != characterClass) continue;
                foreach (StatAttribute sa in pcc.stats)
                {
                    if(sa.stat != stat) continue;
                    StatAttribute targetSA = sa;
                    float statValue = targetSA.GetStartingValue();
                    for (int i = 0; i < level; i++)
                    {
                        statValue += (i * targetSA.GetBaseAdded()) + (statValue * targetSA.GetPercentageIncrease());
                    }
                    return statValue;
                }
            }
            return 0;
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
