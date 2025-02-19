using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public List<GameObject> items = new List<GameObject>(); // List of items to spawn
    public int numberItems = 5; // Number of items to spawn
    public int range = 20;
    public Transform spawnArea; // Define an area for spawning

    void Start()
    {
        SpawnItems();
    }

    void SpawnItems()
    {
        if (items.Count == 0)
        {
            Debug.LogWarning("Item list is empty! No items to spawn.");
            return;
        }

        for (int i = 0; i < numberItems; i++)
        {
            int randomIndex = Random.Range(0, items.Count);
            GameObject itemToSpawn = items[randomIndex];

            // Set a random position within the defined spawn area
            Vector3 spawnPosition = spawnArea != null
                ? spawnArea.position + new Vector3(Random.Range(-range, range), 0, Random.Range(-range, range))
                : transform.position; // Default to spawner's position

            Instantiate(itemToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
