using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using System;

namespace RPG.Control 
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Mover))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class PlayerController : MonoBehaviour
    {
        Mover mover;
        Fighter fighter;
        Health health;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;

        }
        [SerializeField] CursorMapping[] cursorMappings = null;

        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float maxNavPathLength = 40f;

        private void Awake() 
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        void Update()
        {
            if(InteractWithUI()) return;
            if(health.isDead()) 
            {
                SetCursor(CursorType.None);
                return;
            }
            if(InteractWithComponent()) return;
            if(InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject()) {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithComponent()
        {
            foreach (RaycastHit hit in SortedRaycastHits())
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach(IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        private RaycastHit[] SortedRaycastHits()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            Array.Sort(hits, delegate(RaycastHit x, RaycastHit y) 
            {
                return x.distance.CompareTo(y.distance);
            });
            return hits;
        }

        private bool InteractWithMovement()
        {            
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if(!hasHit) return false;
            
            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if(!hasCastToNavMesh) return false;

            target = navMeshHit.position;
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
            
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;
            
            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;
            for (int i = 0; i < path.corners.Length - 1; i++) 
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach(CursorMapping mapping in cursorMappings)
            {
                if(mapping.type == type) return mapping;
            }
            return cursorMappings[0];
        }

        private static Ray GetMouseRay() 
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}


