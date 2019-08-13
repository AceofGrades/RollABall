using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.TestTools;
using NUnit.Framework;

public class TestSuite
{
    private GameObject game;
    private GameManager gameManager;
    private Player player;
    private int oldScore;
    private int newScore;

    [SetUp]
    public void Setup()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Game");
        game = Object.Instantiate(prefab);
        // Get GameManager
        gameManager = GameManager.Instance;
        player = Object.FindObjectOfType<Player>();
        player = game.GetComponentInChildren<Player>();
    }

    [UnityTest]
    public IEnumerator GamePrefabLoaded()
    {
        yield return new WaitForEndOfFrame();

        // Game object should exist at this point in time
        Assert.NotNull(game, "Ya goofed up, kiddo");
    }

    [UnityTest]
    public IEnumerator PlayerExists()
    {
        yield return new WaitForEndOfFrame();

        Assert.NotNull(player, "You've been played by the player, cause he ain't here...");
    }

    [UnityTest]
    public IEnumerator ItemCollidesWithPlayer()
    {
        //Item item = gameManager.itemManager.GetItem(0);

        //item.transform.position = player.transform.position;
        GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Entities/Item");

        Vector3 playerPosition = player.transform.position;
        GameObject item = Object.Instantiate(itemPrefab, playerPosition, Quaternion.identity);
        yield return new WaitForSeconds(.1f);

        Assert.IsTrue(item == null);
    }

    [UnityTest]
    public IEnumerator ItemCollectedAndScoreAdded()
    {
        // Spawn an item
        GameObject itemPrefab = Resources.Load<GameObject>("Prefabs/Entities/Item");
        Vector3 playerPosition = player.transform.position;
        GameObject item = Object.Instantiate(itemPrefab, playerPosition, Quaternion.identity);
        // Record old score in int
        int oldScore = gameManager.score;

        // WaitForFixedUpdate
        yield return new WaitForFixedUpdate();
        // WaitForEndOfFrame
        yield return new WaitForEndOfFrame();

        int newScore = gameManager.score;
        // Assert IsTrue old score != new score
        Assert.IsTrue(oldScore != newScore);
    }
    [TearDown]
    public void Teardown()
    {
        Object.Destroy(gameManager.gameObject);
    }
}
