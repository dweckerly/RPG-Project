using UnityEngine;
using UnityEngine.Playables;

namespace RPG.Cinematics 
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool played = false;
        private void OnTriggerEnter(Collider col) 
        {
            if(!played && col.gameObject.CompareTag("Player")) 
            {
                GetComponent<PlayableDirector>().Play();
                played = true;
            }
        }
    }
}
