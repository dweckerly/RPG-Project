using System;
using UnityEngine;
using RPG.Utils;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;
        [SerializeField] GameObject levelUpEffect;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;
        const int MAX_LEVEL = 100;

        Experience experience;

        private void Awake() 
        {
            experience = GetComponent<Experience>();
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void OnEnable()
        {
            if (experience != null) experience.onExperienceGained += UpdateLevel;
        }

        private void OnDisable() 
        {
            if (experience != null) experience.onExperienceGained -= UpdateLevel;
        }

        private void Start() 
        {
            currentLevel.ForceInit();
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if(newLevel > currentLevel.value) 
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpEffect, transform);
        }

        public float GetHealth()
        {
            return progression.GetStat(Stat.Health, characterClass, GetLevel());
        }

        public float GetDamage()
        {
            return (progression.GetStat(Stat.Damage, characterClass, GetLevel()) + GetAdditiveModifier(Stat.Damage)) * (1 + GetPercentageModifier(Stat.Damage));
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
            return currentLevel.value;
        }

        public int CalculateLevel()
        {
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

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach(IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach(float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat) 
        {
            if (!shouldUseModifiers) return 0;
            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total / 100;
        }
    }
}

