using UnityEngine;

public class ClimbHand : MonoBehaviour
{
    [Header("which hand")]
    public bool isLeftHand = true;

    [Header("quo")]
    public HandGripDetector gripDetector;   
    public Transform handVisual;           

    [Header("grab settings")]
    public float grabRadius = 0.15f;       // radius of grab detection
    public LayerMask holdLayerMask;        // this layer only

    [Header("debug")]
    public ClimbHold currentHold;

    void Reset()
    {
        if (gripDetector == null)
        {
            gripDetector = FindObjectOfType<HandGripDetector>();
        }

        if (handVisual == null)
        {
            handVisual = this.transform;
        }
    }

    void Start()
    {
        if (gripDetector == null)
        {
            gripDetector = FindObjectOfType<HandGripDetector>();
        }

        if (isLeftHand)
        {
            gripDetector.OnLeftGrabStart += HandleGrabStart;
            gripDetector.OnLeftGrabEnd += HandleGrabEnd;
        }
        else
        {
            gripDetector.OnRightGrabStart += HandleGrabStart;
            gripDetector.OnRightGrabEnd += HandleGrabEnd;
        }
    }

    void OnDestroy()
    {
        if (gripDetector == null) return;

        if (isLeftHand)
        {
            gripDetector.OnLeftGrabStart -= HandleGrabStart;
            gripDetector.OnLeftGrabEnd -= HandleGrabEnd;
        }
        else
        {
            gripDetector.OnRightGrabStart -= HandleGrabStart;
            gripDetector.OnRightGrabEnd -= HandleGrabEnd;
        }
    }

    void Update()
    {
        
        if (currentHold != null && handVisual != null)
        {
            handVisual.position = currentHold.transform.position;
        }
    }

    
    void HandleGrabStart()
    {
        
        if (currentHold != null) return;

        ClimbHold nearest = FindNearestHold();
        if (nearest != null)
        {
            AttachToHold(nearest);
        }
        else
        {
            
            // Debug.Log((isLeftHand ? "left" : "right") + "fail to grab");
        }
    }

   
    void HandleGrabEnd()
    {
        if (currentHold != null)
        {
            ReleaseHold();
        }
    }

    
    ClimbHold FindNearestHold()
    {
        if (handVisual == null) return null;

        Collider[] hits = Physics.OverlapSphere(
            handVisual.position,
            grabRadius,
            holdLayerMask
        );

        float bestDist = Mathf.Infinity;
        ClimbHold bestHold = null;

        foreach (var col in hits)
        {
            ClimbHold hold = col.GetComponent<ClimbHold>();
            if (hold == null) continue;

            float d = Vector3.Distance(handVisual.position, hold.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                bestHold = hold;
            }
        }

        return bestHold;
    }

    
    void AttachToHold(ClimbHold hold)
    {
        currentHold = hold;

        if (handVisual != null)
        {
            handVisual.position = hold.transform.position;
        }

        // TODO: Stamina
    }

    
    void ReleaseHold()
    {
        //TODO: Stamina
        currentHold = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (handVisual == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(handVisual.position, grabRadius);
    }
}
