using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.SceneManagement
{
    public class Fader : MonoBehaviour
    {
        CanvasGroup canvasGroup;
        Coroutine currentlyActiveFade = null;

        private void Awake() 
        {
            canvasGroup = GetComponent<CanvasGroup>();    
        }

        public void FadeOutImmediate()
        {
            canvasGroup.alpha = 1;
        }

        public IEnumerator FadeOut(float time)
        {
            if(currentlyActiveFade != null) StopCoroutine(currentlyActiveFade);
            currentlyActiveFade = StartCoroutine(FadeRoutine(1, time));
            yield return currentlyActiveFade;
        }

        public IEnumerator FadeIn(float time)
        {
            if (currentlyActiveFade != null) StopCoroutine(currentlyActiveFade);
            currentlyActiveFade = StartCoroutine(FadeRoutine(0, time));
            yield return currentlyActiveFade;
        }

        private IEnumerator FadeRoutine(float target, float time)
        {
            while (!Mathf.Approximately(canvasGroup.alpha, target))
            {
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
                yield return null;
            }
        }
    }

}
