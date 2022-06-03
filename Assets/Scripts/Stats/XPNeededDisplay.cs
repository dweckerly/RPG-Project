using System;
using UnityEngine;
using TMPro;

namespace RPG.Stats
{
    public class XPNeededDisplay : MonoBehaviour
    {
        BaseStats baseStats;

        void Awake()
        {
            baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        }

        void Update()
        {
            GetComponent<TextMeshProUGUI>().text = String.Format("{0}", baseStats.GetExperienceNeeded());
        }
    }
}

