using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;
using RPG.Utils;
using System;

namespace RPG.Control 
{
    public class AIController : MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] float shoutDistance = 10f;
        [SerializeField] float aggroCoolDownTime = 7.5f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 3f;
        [SerializeField] float waypointDwellTime = 3f;
        int currentWaypointIndex = 0;

        GameObject player;
        Fighter fighter;
        Health health;
        Mover mover;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceAggravated = Mathf.Infinity;
        
        private void Awake() 
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start() 
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            if (health.isDead()) return;
            if ((InAttackRange(player) || isAggravated()) && fighter.CanAttack(player))
            {                
                AttackBehaviour();
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }
            UpdateTimers();
        }

        public void Aggravate()
        {
            timeSinceAggravated = 0;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceAggravated += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            Vector3 nextPosition = guardPosition.value;
            if(patrolPath != null)
            {
                if(AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if(timeSinceArrivedAtWaypoint > waypointDwellTime) mover.StartMoveAction(nextPosition, patrolSpeedFraction);
        }

        private bool AtWaypoint()
        {   
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

        private void CycleWaypoint()
        {            
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint() 
        {
            return patrolPath.GetWaypointPosition(currentWaypointIndex);
        }

        private void SuspicionBehaviour()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehaviour()
        {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
            AggravateNearbyEnemies();
        }

        private void AggravateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up);
            foreach(RaycastHit hit in hits)
            {
                AIController aIController = hit.collider.GetComponent<AIController>();
                if(aIController != null) aIController.Aggravate();
            }
        }

        private bool isAggravated()
        {
            return timeSinceAggravated < aggroCoolDownTime;
        }

        private bool InAttackRange(GameObject target)
        {
            float distanceToTarget = Vector3.Distance(target.transform.position, transform.position);
            return distanceToTarget < chaseDistance;
        }

        private void OnDrawGizmosSelected() 
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}

