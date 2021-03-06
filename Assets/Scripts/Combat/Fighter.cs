using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using RPG.Inventories;

namespace RPG.Combat 
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        float timeSinceLastAttack = Mathf.Infinity;
        Health target;
        Equipment equipment;
        BaseStats stats;
        Mover mover;
        Animator animator;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;

        void Awake()
        {
            stats = GetComponent<BaseStats>();
            mover = GetComponent<Mover>();
            animator = GetComponent<Animator>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment) equipment.equipmentUpdated += UpdateWeapon;
        }

        private Weapon SetDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }

        private void Start() 
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;
            if(target == null || target.isDead()) return;
            if(IsInRange(target.transform))
            {
                mover.Cancel();
                AttackBehaviour();
            }
            else 
            {
                mover.MoveTo(target.transform.position, 1f);
            }
        }

        private void AttackBehaviour()
        {
            transform.LookAt(target.transform);
            if(timeSinceLastAttack > currentWeaponConfig.GetSpeed())
            {
                // this will trigger Hit() event
                TriggerAttack();
                timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        // Animation Event
        void Hit()
        {
            if(target == null) return;
            float damage = stats.GetDamage();
            if(currentWeapon.value != null) currentWeapon.value.OnHit();
            if (currentWeaponConfig.HasProjectile()) 
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else 
            {
                target.TakeDamage(gameObject, damage);
            }
            
        }

        // Animation Event
        void Shoot()
        {
            Hit();
        }

        private bool IsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.transform.position) < currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if(combatTarget == null || (!mover.CanMoveTo(combatTarget.transform.position) && !IsInRange(combatTarget.transform))) return false;
            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.isDead();
        }

        public void Attack(GameObject combatTarget) 
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            mover.Cancel();
        }

        private void StopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            WeaponConfig weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null) EquipWeapon(defaultWeapon);
            else EquipWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
            return target;
        }

        public object CaptureState() 
        {
            return currentWeaponConfig == null ? defaultWeapon.name : currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = state.ToString();
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }
} 

