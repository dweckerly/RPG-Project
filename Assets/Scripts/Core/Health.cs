using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Core
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float health = 100f;
        bool dead = false;

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
