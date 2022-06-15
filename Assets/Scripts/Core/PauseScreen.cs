using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.SceneManagement;

namespace RPG.Core 
{
    public class PauseScreen : MonoBehaviour
    {
        public GameObject persistentObjectPrefab = null;
        SavingWrapper savingWrapper;
        CanvasGroup canvasGroup;

        void Awake()
        {
            savingWrapper = GetComponent<SavingWrapper>();
            canvasGroup = GetComponent<CanvasGroup>();            
        }

        private void Start() 
        {
            UnPause();
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if(canvasGroup.alpha == 0) 
                {
                    Pause();
                    return;
                }
                UnPause();
            }
        }

        void Pause() 
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            Time.timeScale = 0;
        }

        void UnPause()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            Time.timeScale = 1;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

