using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<GameObject> wallPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> usedWalls = new List<GameObject>();
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

    public void SpawnNewWall()
    {
        Vector3 newPos = new Vector3(currentWall.transform.position.x, currentWall.transform.position.y + 5, currentWall.transform.position.z);

        var availableWalls = wallPrefabs.Except(usedWalls).ToList();

        if (availableWalls.Count == 0)
        {
            usedWalls.Clear();
            availableWalls = wallPrefabs.ToList();
        }

        GameObject newWall = Instantiate(availableWalls[Random.Range(0, availableWalls.Count)], newPos, Quaternion.identity);
        usedWalls.Add(newWall);

        currentWall.GetComponent<Wall>().old = true;
        currentWall = newWall;
    }
}
