using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health health = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas canvas;

        void Update()
        {
            float healthFraction = health.GetFraction();
            if(Mathf.Approximately(healthFraction, 1) || Mathf.Approximately(healthFraction, 0))
            { 
                canvas.enabled = false;
                return;
            }
            canvas.enabled = true;
            foreground.localScale = new Vector3(healthFraction, 1f, 1f);
        }
    }
}
