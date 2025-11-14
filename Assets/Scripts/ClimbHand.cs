using UnityEngine;

public class ClimbHand : MonoBehaviour
{
    [Header("which hand")]
    public bool isLeftHand = true;

    [Header("refs")]
    public HandGripDetector gripDetector;
    public Transform handVisual;
    public PlayerManager player;          

    [Header("grab settings")]
    public float grabRadius = 0.15f;
    public LayerMask holdLayerMask;

    [Header("debug")]
    public ClimbHold currentHold;

    private ClimbHold lastHold;
    private float lastReleaseTime;
    public float minRegrabDelay = 0.3f;      
    public float minRegrabDistance = 0.1f;   
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

        if (player == null)
        {
            player = PlayerManager.Instance;
        }
    }

    void Start()
    {
        if (gripDetector == null)
        {
            gripDetector = FindObjectOfType<HandGripDetector>();
        }

        if (player == null)
        {
            player = PlayerManager.Instance;
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
        
        if (player != null)
        {
            if (isLeftHand && player.staminaLeft <= 0f) return;
            if (!isLeftHand && player.staminaRight <= 0f) return;
        }

        if (currentHold != null) return;

        ClimbHold nearest = FindNearestHold();
        if (nearest != null)
        {
            AttachToHold(nearest);
        }
        else
        {
            
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

            
            if (hold == lastHold)
            {
                if (Time.time - lastReleaseTime < minRegrabDelay)
                    continue;

                
                float distToLast = Vector3.Distance(handVisual.position, lastHold.transform.position);
                if (distToLast < minRegrabDistance)
                    continue;
            }

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

        
        if (player != null)
        {
            if (isLeftHand) player.isGrabbingLeft = true;
            else player.isGrabbingRight = true;
        }
    }


    void ReleaseHold()
    {
        if (currentHold != null)
        {
            lastHold = currentHold;
            lastReleaseTime = Time.time;
        }

        currentHold = null;

        if (player != null)
        {
            if (isLeftHand) player.isGrabbingLeft = false;
            else player.isGrabbingRight = false;
        }
    }



    public void ForceReleaseFromStamina()
    {
        if (currentHold != null)
        {
            ReleaseHold();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (handVisual == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(handVisual.position, grabRadius);
    }
}
