using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject health_bar_parent = null;
    [SerializeField] private Image health_bar_image = null;

    private void Awake()
    {
        health.onClientHealthUpdated += handleHealthUpdated;
    }

    private void OnMouseEnter()
    {
        health_bar_parent.SetActive(true);
    }

    private void OnMouseExit()
    {
        health_bar_parent.SetActive(false);
    }

    private void OnDestroy()
    {
        health.onClientHealthUpdated -= handleHealthUpdated;
    }

    private void handleHealthUpdated(int current_health, int max_health)
    {
        health_bar_image.fillAmount = (float)current_health / max_health;
    }
}
