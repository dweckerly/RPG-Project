using System;
using UnityEngine;
using TMPro;

namespace RPG.Stats
{
    public class XPDisplay : MonoBehaviour
    {
        Experience exp;

        private void Awake()
        {
            exp = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            GetComponent<TextMeshProUGUI>().text = String.Format("{0}", exp.GetExperiencePoints());
        }
    }
}

