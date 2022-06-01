using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 1)]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgressionCharacterClass[] characterClasses = null;

        public float GetHealth(CharacterClass characterClass, int level)
        {
            foreach(ProgressionCharacterClass pcc in characterClasses) 
            {
                if(pcc.characterClass == characterClass)
                {
                    HealthProperties hp = pcc.healthProperties;
                    float healthStat = hp.GetStartingValue();
                    for(int i = 0; i < level; i++)
                    {
                        healthStat += (i * hp.GetBaseAdded()) + (healthStat * hp.GetPercentageIncrease());
                    }
                    return healthStat;
                }
            }
            return 0;
        }

        [System.Serializable]
        class ProgressionCharacterClass
        {
            public CharacterClass characterClass;
            public HealthProperties healthProperties;
        }

        [System.Serializable]
        class HealthProperties
        {
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
