using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 1)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        public float GetStat(CharacterClass characterClass, int level, Stat stat)
        {
            foreach(ProgressionCharacterClass pcc in characterClasses) 
            {
                if(pcc.characterClass == characterClass)
                {
                    foreach (StatAttribute sa in pcc.stats)
                    {
                        if(sa.stat == stat)
                        {
                            StatAttribute targetSA = sa;
                            float startingValue = targetSA.GetStartingValue();
                            for (int i = 0; i < level; i++)
                            {
                                startingValue += (i * targetSA.GetBaseAdded()) + (startingValue * targetSA.GetPercentageIncrease());
                            }
                            return startingValue;
                        }
                        
                    }
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
