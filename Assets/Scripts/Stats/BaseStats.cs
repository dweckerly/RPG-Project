using System;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect;

        public event Action onLevelUp;

        int currentLevel = 0;
        const int MAX_LEVEL = 100;

        private void Start() 
        {
            currentLevel = GetLevel();
            Experience experience = GetComponent<Experience>();
            if(experience != null) experience.onExperienceGained += UpdateLevel;
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel) 
            {
                currentLevel = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpEffect, transform.position, Quaternion.identity);
        }

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
            if(currentLevel < 1) currentLevel = CalculateLevel();
            return currentLevel;
        }

        public int CalculateLevel()
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

