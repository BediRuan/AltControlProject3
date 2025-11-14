using UnityEngine;
using UnityEngine.UI;

public class GrabbableUIButton : MonoBehaviour
{
    public Button targetButton;

    private void Reset()
    {
        // Auto-find Button on this GameObject if not set
        if (targetButton == null)
        {
            targetButton = GetComponent<Button>();
        }

        Collider col = GetComponent<Collider>();
        col.isTrigger = true; // we want trigger for overlap detection
    }

    /// <summary>
    /// Called when a hand starts gripping this UI object.
    /// </summary>
    public void OnGrab()
    {
        if (targetButton != null)
        {
            targetButton.onClick.Invoke();
        }
        else
        {
            Debug.LogWarning("[GrabbableUIButton] No Button assigned.");
        }
    }
}