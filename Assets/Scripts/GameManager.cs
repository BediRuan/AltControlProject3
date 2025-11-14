using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<GameObject> wallPrefabs = new List<GameObject>();
    public GameObject currentWall = null;

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
        
    }

    public void SpawnNewWall(GameObject wallToSpawn)
    {
        Vector3 newPos = new Vector3(currentWall.transform.position.x, currentWall.transform.position.y + 5, currentWall.transform.position.z);
        GameObject newWall = Instantiate(wallToSpawn, newPos, Quaternion.identity);

        currentWall.GetComponent<Wall>().old = true;
        currentWall = newWall;
    }
}
