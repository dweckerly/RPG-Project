using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using RPG.Movement;
using RPG.Combat;
using RPG.Attributes;
using RPG.Inventories;
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
        ActionStore actionStore;

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;

        }
        [SerializeField] CursorMapping[] cursorMappings = null;

        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;
        
        bool isDragging = false;

        private void Awake() 
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            actionStore = GetComponent<ActionStore>();
        }

        void Update()
        {
            if(InteractWithUI()) return;
            if(health.isDead()) 
            {
                SetCursor(CursorType.None);
                return;
            }
            CheckHotkeys();
            if(InteractWithComponent()) return;
            if(InteractWithMovement()) return;
            SetCursor(CursorType.None);
        }

        private void CheckHotkeys()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) actionStore.Use(0, gameObject);
            if (Input.GetKeyDown(KeyCode.Alpha2)) actionStore.Use(1, gameObject);
            if (Input.GetKeyDown(KeyCode.Alpha3)) actionStore.Use(2, gameObject);
            if (Input.GetKeyDown(KeyCode.Alpha4)) actionStore.Use(3, gameObject);
            if (Input.GetKeyDown(KeyCode.Alpha5)) actionStore.Use(4, gameObject);
            if (Input.GetKeyDown(KeyCode.Alpha6)) actionStore.Use(5, gameObject);
        }

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0)) isDragging = false;
            if (EventSystem.current.IsPointerOverGameObject()) 
            {
                if (Input.GetMouseButtonDown(0)) isDragging = true;
                SetCursor(CursorType.UI);
                return true;
            }
            if (isDragging) return true;
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
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
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
                if (Input.GetMouseButton(0) && mover.CanMoveTo(target))
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
            return mover.CanMoveTo(target);
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


