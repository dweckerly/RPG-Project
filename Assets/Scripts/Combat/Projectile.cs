using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Stats;

namespace RPG.Combat 
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1f;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeTime = 10f;
        Health target = null;
        GameObject source = null;
        float damage = 0;

        void Start() 
        {
            if (target == null) return;
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if(target == null) return;
            if(isHoming && !target.isDead()) transform.LookAt(GetAimLocation());
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject source, float damage)
        {

            this.target = target;
            this.source = source;
            this.damage = damage;

            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCollider = target.GetComponent<CapsuleCollider>();
            return targetCollider == null ? target.transform.position : target.transform.position + (Vector3.up * (targetCollider.height / 2));
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(target == null) return;
            if(other.GetComponent<Health>() != target) return;
            if(target.isDead()) return;
            target.TakeDamage(source, damage); 
            if(hitEffect != null) Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            Destroy(gameObject);  
        }
    }
}

