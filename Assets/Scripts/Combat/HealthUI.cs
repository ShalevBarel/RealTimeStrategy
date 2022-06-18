using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    private void Awake() // this method is being called when this script is being loaded
    {
        health.HealthUpdated += HandleHealthUpdated; // listening the the event from the health script to know when to update the UI
    }

    private void OnDestroy() // this method is being called when this script is not used anymore
    {
        health.HealthUpdated -= HandleHealthUpdated; // stop listening to the event
    }

    private void OnMouseEnter() // being called whenever the mouse is touching the object that this script is on
    {
        healthBarParent.SetActive(true); // showing the health bar
    }

    private void OnMouseExit() // being called whenever the mouse is no longer touching the object that this script is on
    {
        healthBarParent.SetActive(false); // stops showing the health bar
    }

    private void HandleHealthUpdated(int currentHealth, int maxHealth) // this function is being called whenever the health changes
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth; // updates the UI
    }
}
