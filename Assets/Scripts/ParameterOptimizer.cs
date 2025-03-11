using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterOptimizer : MonoBehaviour
{
    public GameObject particlePrefab;
    public int numParticles = 50;
    public float simulationTime = 10f; // Time to run each simulation

    // Parameter ranges
    public float[] forceMagValues = { 1.0f, 2.0f, 3.0f };
    public float[] noiseStrengthValues = { 1.0f, 2.0f, 3.0f };
    public float[] detectionRadiusValues = { 0.5f, 1.0f, 1.5f };
    public float[] separationWeightValues = { 0.5f, 1.0f, 1.5f };
    public float[] alignmentWeightValues = { 0.5f, 1.0f, 1.5f };
    public float[] cohesionWeightValues = { 0.5f, 1.0f, 1.5f };

    private float bestTime = float.MaxValue;
    private float[] bestParameters;

    void Start()
    {
        StartCoroutine(RunOptimization());
    }

    IEnumerator RunOptimization()
    {
        foreach (float forceMag in forceMagValues)
        {
            foreach (float noiseStrength in noiseStrengthValues)
            {
                foreach (float detectionRadius in detectionRadiusValues)
                {
                    foreach (float separationWeight in separationWeightValues)
                    {
                        foreach (float alignmentWeight in alignmentWeightValues)
                        {
                            foreach (float cohesionWeight in cohesionWeightValues)
                            {
                                float time = RunSimulation(forceMag, noiseStrength, detectionRadius, separationWeight, alignmentWeight, cohesionWeight);
                                if (time < bestTime)
                                {
                                    bestTime = time;
                                    bestParameters = new float[] { forceMag, noiseStrength, detectionRadius, separationWeight, alignmentWeight, cohesionWeight };
                                }
                                yield return null; // Wait for the next frame
                            }
                        }
                    }
                }
            }
        }

        Debug.Log("Best Time: " + bestTime);
        Debug.Log("Best Parameters: " + string.Join(", ", bestParameters));
    }

    float RunSimulation(float forceMag, float noiseStrength, float detectionRadius, float separationWeight, float alignmentWeight, float cohesionWeight)
    {
        // Instantiate particles
        List<GameObject> particles = new List<GameObject>();
        for (int i = 0; i < numParticles; i++)
        {
            GameObject particle = Instantiate(particlePrefab, Random.insideUnitCircle * 5f, Quaternion.identity);
            Particle particleScript = particle.GetComponent<Particle>();
            particleScript.forceMag = forceMag;
            particleScript.noiseStrength = noiseStrength;
            particleScript.detectionRadius = detectionRadius;
            particleScript.separationWeight = separationWeight;
            particleScript.alignmentWeight = alignmentWeight;
            particleScript.cohesionWeight = cohesionWeight;
            particles.Add(particle);
        }

        // Run the simulation for a fixed amount of time
        float startTime = Time.time;
        while (Time.time - startTime < simulationTime)
        {
            // Check if all particles have reached their targets
            bool allAtTarget = true;
            foreach (GameObject particle in particles)
            {
                Particle particleScript = particle.GetComponent<Particle>();
                if (!particleScript.IsAtTarget())
                {
                    allAtTarget = false;
                    break;
                }
            }
            if (allAtTarget)
            {
                break;
            }
        }

        // Calculate the time taken to reach the goal
        float timeTaken = Time.time - startTime;

        // Destroy particles
        foreach (GameObject particle in particles)
        {
            Destroy(particle);
        }

        return timeTaken;
    }
}
