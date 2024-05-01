using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    [SerializeField]
    public KeyCode miningKey = KeyCode.E;
    private ResourceMine currentMine;
    private Move movementScript;

    private void Awake()
    {
        movementScript = GetComponent<Move>();
    }

    private void Update()
    {
        if (currentMine != null && Input.GetKeyDown(miningKey))
        {
            StartMining();
            Debug.Log("Hi");
        }
    }

    public void SetCurrentMine(ResourceMine mine)
    {
        currentMine = mine;
    }

    public void ClearCurrentMine()
    {
        currentMine = null;
    }

    private void StartMining()
    {
        movementScript.enabled = false;
        currentMine.StartMining(this);
    }

    public void EndMining()
    {
        movementScript.enabled = true;
    }
}
