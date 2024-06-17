using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fishing : MonoBehaviour
{
    public GameObject bobberPrefab; // Prefab for the bobber
    public Transform bobberSpawnPoint; // Specific spawn point for the bobber
    public List<Fish> fishList; // List of fish
    public GameObject fishCaughtUI; // UI element to display caught fish
    public Image fishIcon; // UI element for the fish icon
    public Text fishNameText; // UI element for the fish name

    private GameObject currentBobber; // Current bobber instance
    private bool isFishBiting = false; // Flag for fish biting
    private bool isReelingIn = false; // Flag for reeling in
    private float timeToBite; // Time until the fish bites
    private float reactionTime = 2f; // Time for the player to react
    private float holdTime = 2f; // Time required to reel in the fish
    private Fish currentFish; // The fish that is currently being caught
    private Coroutine bobbingCoroutine; // Coroutine for bobber bobbing

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentBobber == null)
        {
            SpawnBobber();
        }

        if (isFishBiting && Input.GetKeyDown(KeyCode.E) && !isReelingIn)
        {
            isReelingIn = true;
            StartCoroutine(HoldToCatchFish());
        }

        if (isReelingIn && Input.GetKeyUp(KeyCode.E))
        {
            isReelingIn = false;
        }
    }

    void SpawnBobber()
    {
        Vector3 bobberSpawnPosition = bobberSpawnPoint.position; // Specific spawn position for the bobber
        currentBobber = Instantiate(bobberPrefab, bobberSpawnPosition, Quaternion.identity);
        bobbingCoroutine = StartCoroutine(BobberBobbing());
        StartCoroutine(FishBiteCoroutine());
    }

    Fish SelectRandomFish()
    {
        float totalChance = 0f;
        foreach (Fish fish in fishList)
        {
            totalChance += fish.spawnChance;
        }

        float randomPoint = Random.value * totalChance;

        foreach (Fish fish in fishList)
        {
            if (randomPoint < fish.spawnChance)
            {
                return fish;
            }
            else
            {
                randomPoint -= fish.spawnChance;
            }
        }

        return null;
    }

    IEnumerator FishBiteCoroutine()
    {
        // Random time until the fish bites (between 1 and 5 seconds)
        timeToBite = Random.Range(1f, 5f);
        yield return new WaitForSeconds(timeToBite);

        // Fish is biting
        currentFish = SelectRandomFish();
        if (currentFish != null)
        {
            isFishBiting = true;
            Debug.Log("Fish is biting! Press and hold E to catch!");
            StopCoroutine(bobbingCoroutine);
            bobbingCoroutine = StartCoroutine(BobberBobbing(true));

            // Wait for the player to react
            yield return new WaitForSeconds(reactionTime);

            if (isFishBiting && !isReelingIn)
            {
                // If the player didn't react in time, the fish gets away
                Debug.Log("Fish got away...");
                isFishBiting = false;
                Destroy(currentBobber);
                currentBobber = null;
            }
        }
    }

    IEnumerator HoldToCatchFish()
    {
        float holdTimer = 0f;
        Vector3 startPosition = currentBobber.transform.position;
        Vector3 endPosition = new Vector3(transform.position.x, startPosition.y, startPosition.z);

        while (isReelingIn && holdTimer < holdTime)
        {
            holdTimer += Time.deltaTime;

            // Move the bobber towards the player only horizontally
            if (currentBobber != null)
            {
                currentBobber.transform.position = Vector3.MoveTowards(
                    currentBobber.transform.position,
                    endPosition,
                    Time.deltaTime * (1f / holdTime) * Vector3.Distance(startPosition, endPosition)
                );

                // Check if the difference in X positions is less than 1.0
                if (Mathf.Abs(currentBobber.transform.position.x - transform.position.x) < 1.0f)
                {
                    CatchFish();
                    yield break;
                }
            }

            yield return null;
        }

        if (holdTimer >= holdTime)
        {
            CatchFish();
        }
        else if (!isReelingIn)
        {
            Debug.Log("Stopped reeling in.");
            isFishBiting = false;
            Destroy(currentBobber);
            currentBobber = null;
        }
    }

    void CatchFish()
    {
        Debug.Log("Fish caught! Caught: " + currentFish.name);
        isFishBiting = false;
        isReelingIn = false;
        Destroy(currentBobber);
        currentBobber = null;

        // Display the fish's name and icon
        fishNameText.text = currentFish.name;
        fishIcon.sprite = currentFish.prefab.GetComponent<SpriteRenderer>().sprite;
        fishCaughtUI.SetActive(true);
        StartCoroutine(HideFishCaughtUI());
    }

    IEnumerator HideFishCaughtUI()
    {
        yield return new WaitForSeconds(2f);
        fishCaughtUI.SetActive(false);
    }

    IEnumerator BobberBobbing(bool fishBiting = false)
    {
        float amplitude = fishBiting ? 0.5f : 0.2f;
        float frequency = fishBiting ? 2f : 1f;
        Vector3 startPosition = currentBobber.transform.position;

        while (true)
        {
            float bobbingOffset = Mathf.Sin(Time.time * frequency) * amplitude;
            currentBobber.transform.position = new Vector3(
                startPosition.x,
                startPosition.y + bobbingOffset,
                startPosition.z
            );
            yield return null;
        }
    }
}
