using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.Attributes;

namespace RPG.Movement 
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 7f;
        Animator animator;
        NavMeshAgent agent;
        Health health;

        void Start() 
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            agent.enabled = !health.isDead();
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 dest, float speedFraction) 
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(dest, speedFraction);
        }

        public void MoveTo(Vector3 dest, float speedFraction)
        {
            agent.destination = dest;
            agent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            agent.isStopped = false;
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }

        void UpdateAnimator()
        {
            Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
            float speedPercent = localVelocity.z / maxSpeed;
            animator.SetFloat("forwardSpeed", speedPercent);
        }

        public object CaptureState()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> data = (Dictionary<string, object>) state;
            SerializableVector3 position = (SerializableVector3) data["position"];
            GetComponent<NavMeshAgent>().Warp(position.ToVector());
            SerializableVector3 rotation = (SerializableVector3)data["rotation"];
            transform.eulerAngles = rotation.ToVector();
        }
    }
}
