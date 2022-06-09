using UnityEngine;
using UnityEngine.Events;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [System.Serializable]
        class TakeDamageEvent : UnityEvent<float> { }
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        LazyValue<float> healthPoints;
        bool dead = false;

        BaseStats baseStats;

        private void Awake() 
        {
            baseStats = GetComponent<BaseStats>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return baseStats.GetHealth();
        }

        private void OnEnable() 
        {
            baseStats.onLevelUp += RegenerateHealth; 
        }

        private void OnDisable() 
        {
            baseStats.onLevelUp -= RegenerateHealth;    
        }

        private void Start() 
        {
            healthPoints.ForceInit();    
        }

        public bool isDead()
        {
            return dead;
        }

        public void TakeDamage(GameObject source, float amount)
        {
            print(gameObject.name + " took " + amount + " damage from " + source.name);
            takeDamage.Invoke(amount);
            if (dead) return;
            healthPoints.value = Mathf.Max(healthPoints.value - amount, 0);
            if (healthPoints.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(source);
            }
        }

        public float GetCurrentHealthPoints()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints()
        {
            return baseStats.GetHealth();
        }

        public float GetHealthPercentage()
        {
            return 100 * GetFraction();
        }

        public float GetFraction()
        {
            return (healthPoints.value / baseStats.GetHealth());
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
            healthPoints.value = baseStats.GetHealth();
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float) state;
            if (healthPoints.value == 0) Die();
        }
    }
}
