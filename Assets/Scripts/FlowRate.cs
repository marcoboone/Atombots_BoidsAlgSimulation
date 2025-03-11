using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlowRate : MonoBehaviour
{
    private int atombotCount = 0;
    private float timeElapsed = 0f;
    public float measurementInterval = .1f; // Time interval to measure flow rate
    private int atombotsPassed = 0;
    public Text flowRateText; // Reference to the legacy text component

    void Start()
    {
        // Ensure the game object has a trigger collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = true;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;

        if (timeElapsed >= measurementInterval)
        {
            float flowRate = atombotsPassed / timeElapsed;
            Debug.Log("Flow Rate: " + flowRate + " atombots per second");

            // Update the legacy text with the calculated flow rate
            if (flowRateText != null)
            {
                flowRateText.text = "Flow Rate: " + flowRate.ToString("F2") + " atombots per second";
            }

            // Reset the counters
            timeElapsed = 0f;
            atombotsPassed = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("particle"))
        {
            atombotsPassed++;
        }
    }
}
