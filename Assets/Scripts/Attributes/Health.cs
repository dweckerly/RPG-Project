using UnityEngine;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float health = 100f;
        bool dead = false;

        private void Start() 
        {
            health = GetComponent<BaseStats>().GetHealth();
        }

        public bool isDead()
        {
            return dead;
        }

        public void TakeDamage(float amount)
        {
            if (dead) return;
            health = Mathf.Max(health - amount, 0);
            CheckDeathState();
        }

        public float GetHealthPercentage()
        {
            return 100 * (health / GetComponent<BaseStats>().GetHealth());
        }

        private void CheckDeathState()
        {
            if (health == 0)
            {
                Die();
            }
        }

        private void Die()
        {
            dead = true;
            GetComponent<Animator>().SetTrigger("die");
            GetComponent<ActionScheduler>().CancelCurrentAction();            
        }

        public object CaptureState()
        {
            return health;
        }

        public void RestoreState(object state)
        {
            health = (float) state;
            CheckDeathState();
        }
    }
}
