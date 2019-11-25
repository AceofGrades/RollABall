using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Mirror;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 1.0f; // Time needed to wait until enemy Spawns again

    public override void OnStartServer()
    {
        InvokeRepeating("SpawnEnemy", this.spawnInterval, this.spawnInterval); // Calls the enemy spawn, waiting for spawnInterval until next spawn
    }

    void SpawnEnemy()
    {
        Vector3 spawnPosition = new Vector3(Random.Range(-4.0f, 4.0f), this.transform.position.y, Random.Range(-4f, 4f)); // Dictates where enemy will spawn in the map
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity) as GameObject; // Instantiates enemy relative to the spawnPosition
        NetworkServer.Spawn(enemy); // Updates server to show that enemy spawns
        Destroy(enemy, 10); // Destroy enemy after set amount of time
    }
}
