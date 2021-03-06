using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using RPG.Control;

namespace RPG.SceneManagement 
{
    public class Portal : MonoBehaviour
    {
        enum DestinationIdentifier
        {
            A, B, C, D, E, F
        }
        [SerializeField] int sceneToLoad = -1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;
        [SerializeField] float fadeOutTime = 0.5f;
        [SerializeField] float fadeInTime = 1f;
        [SerializeField] float fadeWaitTime = 0.5f;

        private void OnTriggerEnter(Collider other) 
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Transition());
            }
        } 

        private IEnumerator Transition()
        {
            DontDestroyOnLoad(gameObject);
            
            Fader fader = FindObjectOfType<Fader>();
            GameObject.FindWithTag("Player").GetComponent<PlayerController>().enabled = false;
            yield return fader.FadeOut(fadeOutTime);

            SavingWrapper saveWrapper = FindObjectOfType<SavingWrapper>();
            saveWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);
            PlayerController playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            playerController.enabled = false;
            saveWrapper.Load();

            Portal destinationPortal = GetDestinationPortal();
            UpdatePlayerPosition(destinationPortal);

            saveWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            fader.FadeIn(fadeInTime);
            playerController.enabled = true;
            Destroy(gameObject);
        }

        private Portal GetDestinationPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if(portal == this) continue;
                if(portal.destination == destination) return portal;
            }
            return null;
        }

        private void UpdatePlayerPosition(Portal destinationPortal) 
        {
            GameObject player = GameObject.FindWithTag("Player");
            player.GetComponent<NavMeshAgent>().Warp(destinationPortal.spawnPoint.position);
            player.transform.rotation = destinationPortal.spawnPoint.rotation;
        }
    }
}

