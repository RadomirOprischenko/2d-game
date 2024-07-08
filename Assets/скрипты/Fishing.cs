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
    public LineRenderer fishingLineRenderer; // Line renderer for the fishing line
    public KeyCode reelingKey = KeyCode.E; // Key for reeling in

    private GameObject currentBobber; // Current bobber instance
    private bool isFishBiting = false; // Flag for fish biting
    private bool isReelingIn = false; // Flag for reeling in
    private float timeToBite; // Time until the fish bites
    private float reactionTime = 2f; // Time for the player to react
    private float holdTime = 2f; // Time required to reel in the fish
    private Fish currentFish; // The fish that is currently being caught
    private Coroutine bobbingCoroutine; // Coroutine for bobber bobbing
    private bool isPlayerInRange = false; // Flag to check if the player is in range
    private Move playerMoveScript; // Reference to the player's Move script
    private bool canCancelFishing = false; // Flag to check if fishing can be canceled
    private float cancelDelay = 0.5f; // Delay after spawning the bobber before it can be canceled




    void Start()
    {
        // Hide fish caught UI at the start
        fishCaughtUI.SetActive(false);

        // Hide line renderer at the start
        if (fishingLineRenderer != null)
        {
            fishingLineRenderer.enabled = false;
            fishingLineRenderer.SetPosition(0, new Vector3(1000f, 1000f, 0f)); // Set start position to a distant point
            fishingLineRenderer.SetPosition(1, new Vector3(1000f, 1000f, 0f)); // Set end position to a distant point
        }

        // Find the player object by tag and get the Move component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMoveScript = player.GetComponent<Move>();
        }
    }


    void Update()
    {
        if (Input.GetKeyDown(reelingKey) && currentBobber == null && isPlayerInRange)
        {
            SpawnBobber();
            playerMoveScript.enabled = false; // Disable player movement
        }

        if (Input.GetKeyDown(reelingKey) && currentBobber != null && !isFishBiting && canCancelFishing)
        {
            Debug.Log("No fish biting. Fishing failed.");
            Destroy(currentBobber);
            currentBobber = null;

            // Re-enable player movement
            playerMoveScript.enabled = true;

            // Hide line renderer when fishing fails
            if (fishingLineRenderer != null)
            {
                fishingLineRenderer.enabled = false;
                fishingLineRenderer.SetPosition(0, new Vector3(1000f, 1000f, 0f)); // Set start position to a distant point
                fishingLineRenderer.SetPosition(1, new Vector3(1000f, 1000f, 0f)); // Set end position to a distant point
            }
        }

        if (isFishBiting && Input.GetKeyDown(reelingKey) && !isReelingIn)
        {
            isReelingIn = true;
            StartCoroutine(HoldToCatchFish());
        }

        if (isReelingIn && Input.GetKeyUp(reelingKey))
        {
            isReelingIn = false;
        }

        // Update line renderer position
        if (fishingLineRenderer != null && currentBobber != null)
        {
            fishingLineRenderer.SetPosition(1, currentBobber.transform.position);
        }
    }





    void SpawnBobber()
    {
        Vector3 bobberSpawnPosition = bobberSpawnPoint.position; // Specific spawn position for the bobber
        currentBobber = Instantiate(bobberPrefab, bobberSpawnPosition, Quaternion.identity);
        bobbingCoroutine = StartCoroutine(BobberBobbing());
        StartCoroutine(FishBiteCoroutine());
        StartCoroutine(CancelFishingDelay());

        // Show line renderer
        if (fishingLineRenderer != null)
        {
            fishingLineRenderer.enabled = true;
            fishingLineRenderer.SetPosition(0, transform.position); // Set line renderer start position to player
            fishingLineRenderer.SetPosition(1, currentBobber.transform.position); // Set line renderer end position to bobber
        }
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
            Debug.Log("Fish is biting! Press and hold " + reelingKey.ToString() + " to catch!");
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

                // Re-enable player movement
                playerMoveScript.enabled = true;

                // Hide line renderer when the fish gets away
                if (fishingLineRenderer != null)
                {
                    fishingLineRenderer.enabled = false;
                    fishingLineRenderer.SetPosition(0, new Vector3(1000f, 1000f, 0f)); // Set start position to a distant point
                    fishingLineRenderer.SetPosition(1, new Vector3(1000f, 1000f, 0f)); // Set end position to a distant point
                }
            }
        }
    }

    IEnumerator HoldToCatchFish()
    {
        float holdTimer = 0f;
        Vector3 startPosition = currentBobber.transform.position;

        while (isReelingIn && holdTimer < holdTime)
        {
            holdTimer += Time.deltaTime;
            Vector3 endPosition = new Vector3(transform.position.x, currentBobber.transform.position.y, startPosition.z);

            // Move the bobber towards the player only horizontally
            if (currentBobber != null)
            {
                currentBobber.transform.position = new Vector3(
                    Mathf.MoveTowards(currentBobber.transform.position.x, endPosition.x, Time.deltaTime * Vector3.Distance(startPosition, endPosition) / holdTime),
                    currentBobber.transform.position.y,
                    currentBobber.transform.position.z
                );

                // Check if the difference in X positions is less than 0.1 (to account for floating point imprecision)
                if (Mathf.Abs(currentBobber.transform.position.x - transform.position.x) < 0.1f)
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

            // Re-enable player movement
            playerMoveScript.enabled = true;

            // Hide line renderer when stopping reeling in
            if (fishingLineRenderer != null)
            {
                fishingLineRenderer.enabled = false;
                fishingLineRenderer.SetPosition(0, new Vector3(1000f, 1000f, 0f)); // Set start position to a distant point
                fishingLineRenderer.SetPosition(1, new Vector3(1000f, 1000f, 0f)); // Set end position to a distant point
            }
        }


    }

    IEnumerator CancelFishingDelay()
    {
        canCancelFishing = false;
        yield return new WaitForSeconds(cancelDelay);
        canCancelFishing = true;
    }


    void CatchFish()
    {
        Debug.Log("Fish caught! Caught: " + currentFish.name);
        isFishBiting = false;
        isReelingIn = false;
        Destroy(currentBobber);
        currentBobber = null;

        // Re-enable player movement
        playerMoveScript.enabled = true;

        // Hide line renderer after catching fish
        if (fishingLineRenderer != null)
        {
            fishingLineRenderer.enabled = false;
            fishingLineRenderer.SetPosition(0, new Vector3(1000f, 1000f, 0f)); // Set start position to a distant point
            fishingLineRenderer.SetPosition(1, new Vector3(1000f, 1000f, 0f)); // Set end position to a distant point
        }

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
        if (currentBobber == null) yield return null;
        else
        {
            float amplitude = fishBiting ? 0.8f : 0.4f; // Increased amplitude when reeling
            float frequency = fishBiting ? 3f : 1.5f; // Increased frequency when reeling
            Vector3 startPosition = currentBobber.transform.position;

            while (currentBobber != null)
            {
                float bobbingOffset = Mathf.Sin(Time.time * frequency) * amplitude;
                if (currentBobber != null)
                {
                    currentBobber.transform.position = new Vector3(
                        currentBobber.transform.position.x, // Maintain the current x position
                        startPosition.y + bobbingOffset,
                        currentBobber.transform.position.z
                    );
                }
                yield return null;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

}
