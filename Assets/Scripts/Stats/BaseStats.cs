using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;

        const int MAX_LEVEL = 100;

        public float GetHealth()
        {
            return progression.GetStat(Stat.Health, characterClass, GetLevel());
        }

        public float GetExperienceReward()
        {
            return progression.GetStat(Stat.ExperienceReward, characterClass, GetLevel());
        }

        public float GetExperienceNeeded()
        {
            float experienceNeeded = progression.GetStat(Stat.ExperienceToLevel, characterClass, 0);
            for (int level = 1; level <= GetLevel(); level++)
            {
                experienceNeeded += progression.GetStat(Stat.ExperienceToLevel, characterClass, level);
            }
            return experienceNeeded;
        }

        public int GetLevel()
        {
            Experience experience = GetComponent<Experience>();
            if(experience == null) return startingLevel;
            float currentXP = experience.GetExperiencePoints();
            float experienceNeeded = progression.GetStat(Stat.ExperienceToLevel, characterClass, 0);
            for(int level = 1; level < MAX_LEVEL; level++) 
            {
                experienceNeeded += progression.GetStat(Stat.ExperienceToLevel, characterClass, level);
                if(experienceNeeded > currentXP) return level;
            }
            return MAX_LEVEL;
        }
    }
}

