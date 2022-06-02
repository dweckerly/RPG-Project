using System;
using UnityEngine;
using TMPro;

namespace RPG.Attributes 
{
    public class HealthDisplay : MonoBehaviour
    {
        Health playerHealth;

        private void Awake() 
        {
            playerHealth = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        private void Update() 
        {
            GetComponent<TextMeshProUGUI>().text = String.Format("{0:0}%", playerHealth.GetHealthPercentage());
        }
    }
}

