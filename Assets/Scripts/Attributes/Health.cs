using UnityEngine;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        float healthPoints = -1f;
        bool dead = false;

        BaseStats baseStats;

        private void Awake() 
        {
            baseStats = GetComponent<BaseStats>();
        }

        private void Start() 
        {
            baseStats.onLevelUp += RegenerateHealth;
            if(healthPoints < 0) healthPoints = baseStats.GetHealth();            
        }

        public bool isDead()
        {
            return dead;
        }

        public void TakeDamage(GameObject source, float amount)
        {
            print(gameObject.name + " took " + amount + " damage from " + source.name);
            if (dead) return;
            healthPoints = Mathf.Max(healthPoints - amount, 0);
            if (healthPoints == 0)
            {
                Die();
                AwardExperience(source);
            }
        }

        public float GetCurrentHealthPoints()
        {
            return healthPoints;
        }

        public float GetMaxHealthPoints()
        {
            return baseStats.GetHealth();
        }

        public float GetHealthPercentage()
        {
            return 100 * (healthPoints / baseStats.GetHealth());
        }

        private void AwardExperience(GameObject source)
        {
            Experience exp = source.GetComponent<Experience>();
            if(exp == null) return;
            exp.GainExperience(baseStats.GetExperienceReward());
        }

        private void Die()
        {
            dead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();            
        }

        private void RegenerateHealth()
        {
            healthPoints = baseStats.GetHealth();
        }

        public object CaptureState()
        {
            return healthPoints;
        }

        public void RestoreState(object state)
        {
            healthPoints = (float) state;
            if (healthPoints == 0)
            {
                Die();
            }
        }
    }
}
