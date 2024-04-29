using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceMine : MonoBehaviour
{
    [SerializeField]
    public GameObject resource;

    [SerializeField]
    public float time = 2.0f;

    private ProgressBar progressBar;

    private void Start()
    {
        progressBar = GameObject.FindObjectOfType<ProgressBar>(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMining>().SetCurrentMine(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMining>().ClearCurrentMine();
        }
    }

    public void StartMining(PlayerMining mining)
    {
        // Optionally, disable the collider to prevent re-triggering
        GetComponent<Collider2D>().enabled = false;
        progressBar.StartProgress(time, () => FinishMining(mining));
    }

    private void FinishMining(PlayerMining mining)
    {
        // Spawn resources
        for (int i = 0; i < Random.Range(3, 6); i++)
        {
            Instantiate(resource, transform.position + Random.onUnitSphere, Quaternion.identity);
        }

        mining.EndMining();

        // Destroy the ore object
        Destroy(gameObject);

        // Destroy the progress bar
        progressBar.gameObject.SetActive(false);
    }
}
