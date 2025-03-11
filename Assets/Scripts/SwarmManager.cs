using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmManager : MonoBehaviour
{
    public GameObject particlePrefab;
    public int numRobots = 50;
    public float spawnRadius = 4f;  // Radius around the game object
  

    void Start()
    {
        SpawnRobots();
    }

    void SpawnRobots()
    {
        for (int i = 0; i < numRobots; i++)
        {
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            Instantiate(particlePrefab, spawnPos, Quaternion.identity);
        }
    }
}
