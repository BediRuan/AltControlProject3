using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public List<GameObject> wallPrefabs = new List<GameObject>();
    public GameObject currentWall = null;

    public void SpawnNewWall(GameObject wallToSpawn)
    {
        Vector3 newPos = new Vector3(currentWall.transform.position.x, 5f, currentWall.transform.position.z);
        GameObject newWall = Instantiate(wallToSpawn, newPos, Quaternion.identity);
        currentWall = newWall;
    }
}
