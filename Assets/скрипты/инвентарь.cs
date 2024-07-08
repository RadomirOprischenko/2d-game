using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class инвентарь : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("HHHHiiii");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            gameObject.SetActive(false);
        }
    }
}