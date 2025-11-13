using UnityEngine;

public class ClimbHand : MonoBehaviour
{
    public bool isLeftHand;
    public HandGripDetector detector;

    void Start()
    {
        if (detector == null)
        {
            detector = FindObjectOfType<HandGripDetector>();
        }

        if (isLeftHand)
        {
            detector.OnLeftGrabStart += HandleGrabStart;
            detector.OnLeftGrabEnd += HandleGrabEnd;
        }
        else
        {
            detector.OnRightGrabStart += HandleGrabStart;
            detector.OnRightGrabEnd += HandleGrabEnd;
        }
    }

    void OnDestroy()
    {
        if (detector == null) return;

        if (isLeftHand)
        {
            detector.OnLeftGrabStart -= HandleGrabStart;
            detector.OnLeftGrabEnd -= HandleGrabEnd;
        }
        else
        {
            detector.OnRightGrabStart -= HandleGrabStart;
            detector.OnRightGrabEnd -= HandleGrabEnd;
        }
    }

    void HandleGrabStart()
    {
        
        
        Debug.Log((isLeftHand ? "Left" : "Right") + " Start Grabbing");
    }

    void HandleGrabEnd()
    {
        
        Debug.Log((isLeftHand ? "Left" : "Right") + "Free");
    }
}
