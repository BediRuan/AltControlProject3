using System.Security.Cryptography;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance { get; private set; }

    public bool isGrabbingLeft = false;
    public bool isGrabbingRight = false;

    public float staminaLeft = 0;
    public float staminaRight = 0;

    private float maxStamina = 100;

    [SerializeField] private float staminaRegenRate;
    [SerializeField] private float staminaLossRate;

    public CapsuleCollider col;

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

        col = GetComponent<CapsuleCollider>();
    }

    private void Update()
    {
        HandleStamina();

        if (Input.GetKey(KeyCode.A))
        {
            isGrabbingLeft = true;
        }
        if (Input.GetKeyUp(KeyCode.A))
        {
            isGrabbingLeft = false;
        }

        if (Input.GetKey(KeyCode.D))
        {
            isGrabbingRight = true;
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            isGrabbingRight = false;
        }
    }

    private void HandleStamina()
    {
        if (isGrabbingLeft)
        {
            if (staminaLeft <= maxStamina && staminaLeft > 0)
            {
                staminaLeft -= Time.deltaTime * staminaLossRate;
            }
            else
            {
                staminaLeft = 0f;
            }
        }
        else
        {
            if (staminaLeft < maxStamina)
            {
                staminaLeft += Time.deltaTime * staminaRegenRate;
            }
            else
            {
                staminaLeft = maxStamina;
            }
        }

        if (isGrabbingRight)
        {
            if (staminaRight <= maxStamina && staminaRight > 0)
            {
                staminaRight -= Time.deltaTime * staminaLossRate;
            }
            else
            {
                staminaRight = 0f;
            }
        }
        else
        {
            if (staminaRight < maxStamina)
            {
                staminaRight += Time.deltaTime * staminaRegenRate;
            }
            else
            {
                staminaRight = maxStamina;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Wall>().gameObject == GameManager.Instance.currentWall && other.GetComponent<Wall>().old == false)
        {
            GameManager.Instance.SpawnNewWall();
        }
    }
}
