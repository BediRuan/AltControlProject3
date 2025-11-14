using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    
    public bool isGrabbingLeft = false;
    public bool isGrabbingRight = false;

    
    public float staminaLeft = 0;
    public float staminaRight = 0;
    public float maxStamina = 100f;

    [SerializeField] private float staminaRegenRate = 20f; 
    [SerializeField] private float staminaLossRate = 15f;  

    
    public ClimbHand leftHand;
    public ClimbHand rightHand;

    
    public float bodyMoveSpeed = 5f;                    
    public Vector3 bodyOffsetFromHands = new Vector3(0f, -0.5f, 0f);
    public CapsuleCollider col;
    private float initialZ;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        staminaLeft = maxStamina;
        staminaRight = maxStamina;

        isGrabbingLeft = false;
        isGrabbingRight = false;

        if (col == null)
        {
            col = GetComponent<CapsuleCollider>();
        }

        //
        initialZ = transform.position.z;
    }

    private void Update()
    {
        SyncGrabbingStateFromHands();
        HandleStamina();
        HandleBodyMovement();
    }

    
    private void SyncGrabbingStateFromHands()
    {
        if (leftHand != null)
        {
            isGrabbingLeft = leftHand.currentHold != null;
        }

        if (rightHand != null)
        {
            isGrabbingRight = rightHand.currentHold != null;
        }
    }

    private void HandleStamina()
    {
        
        if (isGrabbingLeft)
        {
            if (staminaLeft > 0f)
            {
                staminaLeft -= Time.deltaTime * staminaLossRate;
                staminaLeft = Mathf.Max(staminaLeft, 0f);
            }

            
            if (staminaLeft <= 0f && leftHand != null)
            {
                leftHand.ForceReleaseFromStamina();
            }
        }
        else
        {
            if (staminaLeft < maxStamina)
            {
                staminaLeft += Time.deltaTime * staminaRegenRate;
                staminaLeft = Mathf.Min(staminaLeft, maxStamina);
            }
        }

        
        if (isGrabbingRight)
        {
            if (staminaRight > 0f)
            {
                staminaRight -= Time.deltaTime * staminaLossRate;
                staminaRight = Mathf.Max(staminaRight, 0f);
            }

            if (staminaRight <= 0f && rightHand != null)
            {
                rightHand.ForceReleaseFromStamina();
            }
        }
        else
        {
            if (staminaRight < maxStamina)
            {
                staminaRight += Time.deltaTime * staminaRegenRate;
                staminaRight = Mathf.Min(staminaRight, maxStamina);
            }
        }
    }

    
    private void HandleBodyMovement()
    {
        int grabCount = 0;
        Vector3 sumPos = Vector3.zero;

        if (leftHand != null && leftHand.currentHold != null)
        {
            sumPos += leftHand.currentHold.transform.position;
            grabCount++;
        }

        if (rightHand != null && rightHand.currentHold != null)
        {
            sumPos += rightHand.currentHold.transform.position;
            grabCount++;
        }

        if (grabCount > 0)
        {
            Vector3 handCenter = sumPos / grabCount;
            Vector3 targetBodyPos = handCenter + bodyOffsetFromHands;

            

            Vector3 newPos = Vector3.Lerp(
                transform.position,
                targetBodyPos,
                bodyMoveSpeed * Time.deltaTime
            );

            
            newPos.z = initialZ;

            transform.position = newPos;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Wall wall = other.GetComponent<Wall>();
        if (wall != null && wall.gameObject == GameManager.Instance.currentWall && wall.old == false)
        {
            GameManager.Instance.SpawnNewWall();
        }
    }
}
