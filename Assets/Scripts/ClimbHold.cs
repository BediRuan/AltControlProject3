using UnityEngine;

public class ClimbHold : MonoBehaviour
{
    [Header("Stamina Cost")]
    public float staminaCostMultiplier = 1f;

	[Header("Runtime State")]
	public bool isGrabbed = false;
	public ClimbHand grabbedByHand;

	/// <summary>
	/// 返回从给定点到本抓点（包含自身与子物体所有 Colliders）的最近距离。
	/// 使用 Collider.ClosestPoint 以应对不规则碰撞体。
	/// </summary>
	public float GetClosestDistanceTo(Vector3 fromPoint)
	{
		float bestSqr = Mathf.Infinity;

		// 收集所有 colliders（含子物体），以覆盖复杂抓取体
		var colliders = GetComponentsInChildren<Collider>();
		for (int i = 0; i < colliders.Length; i++)
		{
			var c = colliders[i];
			if (c == null || !c.enabled) continue;

			Vector3 closest = c.ClosestPoint(fromPoint);
			float sqr = (closest - fromPoint).sqrMagnitude;
			if (sqr < bestSqr) bestSqr = sqr;
		}

		// 如果没有任何可用的 collider，则退回到 transform 位置的距离
		if (float.IsInfinity(bestSqr))
		{
			return Vector3.Distance(transform.position, fromPoint);
		}

		return Mathf.Sqrt(bestSqr);
	}

	/// <summary>
	/// Called by ClimbHand when this hold gets grabbed.
	/// </summary>
	public void OnGrabbed(ClimbHand hand)
	{
		isGrabbed = true;
		grabbedByHand = hand;
	}

	/// <summary>
	/// Called by ClimbHand when this hold gets released.
	/// </summary>
	public void OnReleased(ClimbHand hand)
	{
		// 仅当当前记录的手释放时才清除
		if (grabbedByHand == hand)
		{
			isGrabbed = false;
			grabbedByHand = null;
		}
	}

    public bool isRestPoint = false;


    public float gizmoRadius = 0.05f;

    private void OnDrawGizmos()
    {
		// 被抓握时高亮为红色，否则根据休息点与否显示不同颜色
		if (isGrabbed)
		{
			Gizmos.color = Color.red;
		}
		else
		{
			Gizmos.color = isRestPoint ? Color.green : Color.yellow;
		}
        Gizmos.DrawSphere(transform.position, gizmoRadius);
    }
}
