using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Inventories;

namespace RPG.Control
{
    [RequireComponent(typeof(Pickup))]
    public class ClickablePickup : MonoBehaviour, IRaycastable
    {
        Pickup pickup;

        private void Awake() 
        {
            pickup = GetComponent<Pickup>();   
        }

        public CursorType GetCursorType()
        {
            if(pickup.CanBePickedUp()) return CursorType.Pickup;
            else return CursorType.None;
        }

        public bool HandleRaycast(PlayerController playerController)
        {
            if(Input.GetMouseButtonDown(0)) pickup.PickupItem();
            return true;
        }
    }

}
