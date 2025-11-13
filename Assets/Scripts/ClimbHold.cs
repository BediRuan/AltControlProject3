using UnityEngine;

public class ClimbHold : MonoBehaviour
{
    [Header("Stamina Cost")]
    public float staminaCostMultiplier = 1f;  

    
    public bool isRestPoint = false;         

  
    public float gizmoRadius = 0.05f;

    private void OnDrawGizmos()
    {
        Gizmos.color = isRestPoint ? Color.green : Color.yellow;
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}
