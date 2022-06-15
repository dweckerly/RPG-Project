using System.Collections;
using UnityEngine;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        SavingSystem savingSystem;
        [SerializeField] float fadeInTime = 0.2f;

        private void Awake() 
        {
            savingSystem = GetComponent<SavingSystem>();
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {
            yield return savingSystem.LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.S)) Save();
            if (Input.GetKeyDown(KeyCode.L)) Load();
            if (Input.GetKeyDown(KeyCode.Delete)) Delete();
        }

        public void Save() 
        {
            savingSystem.Save(defaultSaveFile);
        }

        public void Load()
        {
            savingSystem.Load(defaultSaveFile);
        }

        public void Delete()
        {
            savingSystem.Delete(defaultSaveFile);
        }
    }
}
