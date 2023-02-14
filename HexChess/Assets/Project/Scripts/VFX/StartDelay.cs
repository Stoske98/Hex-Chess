using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartDelay : MonoBehaviour
{

    public GameObject ActivatedGameObject;
    public float Delay = 1;

    private float currentTime = 0;
    private bool isEnabled;

    void OnEnable()
    {
        ActivatedGameObject.SetActive(false);
        isEnabled = false;
        currentTime = 0;
    }

    void Update()
    {
        currentTime += Time.deltaTime;
        if (!isEnabled && currentTime >= Delay)
        {
            isEnabled = true;
            ActivatedGameObject.SetActive(true);

        }
    }
}
