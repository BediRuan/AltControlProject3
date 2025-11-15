using UnityEngine;

public class ClimbHand : MonoBehaviour
{
    [Header("Which hand")]
    public bool isLeftHand = true;

    [Header("References")]
    public HandGripDetector gripDetector;
    public Transform handVisual;      // visual hand that you see in the scene
    public PlayerManager player;

    [Header("Grab settings")]
    public float grabRadius = 0.15f;
    public LayerMask holdLayerMask;

    [Header("World movement")]
    public Transform worldRoot;       // parent of all scene objects you want to move
    public bool dragWorldWhenGrabbing = true;

    [Header("Debug")]
    public ClimbHold currentHold;

    [Header("World movement sensitivity")]
    public float worldDragMultiplier = 3f;   // Adjust this in inspector

    private ClimbHold lastHold;
    private float lastReleaseTime;
    public float minRegrabDelay = 0.3f;
    public float minRegrabDistance = 0.1f;

    // Tracking hand movement between frames
    private Vector3 lastHandPos;
    private bool hasLastHandPos = false;

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

        if (handVisual == null)
        {
            handVisual = this.transform;
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

        // Initialize hand position tracking
        lastHandPos = handVisual.position;
        hasLastHandPos = true;
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

    /// <summary>
    /// We use LateUpdate so that any input / Leap motion updates have already applied to handVisual.
    /// Then we decide whether to move the world or keep the hand free.
    /// </summary>
    void LateUpdate()
    {
        if (handVisual == null) return;

        Vector3 currentPos = handVisual.position;

        if (!hasLastHandPos)
        {
            lastHandPos = currentPos;
            hasLastHandPos = true;

            // If we are already holding something, make sure the visual hand snaps to it
            if (currentHold != null)
            {
                handVisual.position = currentHold.transform.position;
                lastHandPos = handVisual.position;
            }
            return;
        }

        Vector3 delta = currentPos - lastHandPos;

        if (currentHold != null && worldRoot != null && dragWorldWhenGrabbing)
        {
            // Hand is grabbing a hold: move the world instead of the hand

            // If you only want vertical movement, zero out x/z:
            // delta = new Vector3(0f, delta.y, 0f);

            delta = new Vector3(0f, delta.y, 0f);
			worldRoot.position += delta;


            // Keep the visual hand locked to the hold position
            handVisual.position = currentHold.transform.position;

            // After moving the world, the hold position changed, so update lastHandPos
            lastHandPos = handVisual.position;
        }
        else
        {
            // Not grabbing, or no worldRoot: let the hand move freely
            // (we do not override handVisual.position here)

            // If we are still attached to a hold but cannot move worldRoot,
            // you can optionally snap to hold:
            if (currentHold != null && (worldRoot == null || !dragWorldWhenGrabbing))
            {
                handVisual.position = currentHold.transform.position;
            }

            lastHandPos = handVisual.position;
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

				// 使用碰撞体最近点与手的距离避免误判
				float distToLast = lastHold.GetClosestDistanceTo(handVisual.position);
                if (distToLast < minRegrabDistance)
                    continue;
            }

			// 使用 Collider.ClosestPoint 计算真实最近距离
			float d = hold.GetClosestDistanceTo(handVisual.position);
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
            lastHandPos = handVisual.position;
            hasLastHandPos = true;
        }

		// 通知抓点：被抓握
		if (currentHold != null)
		{
			currentHold.OnGrabbed(this);
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
			// 通知抓点：被释放
			currentHold.OnReleased(this);

            lastHold = currentHold;
            lastReleaseTime = Time.time;
        }

        currentHold = null;

        if (player != null)
        {
            if (isLeftHand) player.isGrabbingLeft = false;
            else player.isGrabbingRight = false;
        }

        // On release, reset tracking so we don't get a huge one-frame delta
        hasLastHandPos = false;
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
