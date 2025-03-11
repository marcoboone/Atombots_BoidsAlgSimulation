using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour
{
    public float forceMag = 2.0f;  // Active motion speed
    public float noiseStrength = 3f;  // Brownian motion intensity
    public float detectionRadius = 1.0f;  // Radius for detecting neighbors
    public float separationWeight = 1.5f;  // Weight for separation force
    public float alignmentWeight = 1.0f;  // Weight for alignment force
    public float cohesionWeight = 1.0f;  // Weight for cohesion force
    private Rigidbody2D rb;
    private Vector2 targetPosition;
    private Transform[] targets;
    private int targetIndex = 0; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        CollectTargets(); 
        SetTargetVector();
    }

    void FixedUpdate()
    {
        MoveRobot();
        if(IsAtTarget())
        {
            if (targetIndex < targets.Length-1)
            { targetIndex++;
            }
            SetTargetVector(); 
        }
    }

    void MoveRobot()
    {
        // Compute direction toward the doorway
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Boids behaviors
        Vector2 separation = ComputeSeparation() * separationWeight;
        Vector2 alignment = ComputeAlignment() * alignmentWeight;
        Vector2 cohesion = ComputeCohesion() * cohesionWeight;

        // Combine all forces
        Vector2 force = direction * forceMag + separation + alignment + cohesion;

        // Add Brownian noise (random jitter)
        Vector2 noise = new Vector2(
            Random.Range(-noiseStrength, noiseStrength),
            Random.Range(-noiseStrength, noiseStrength)
        );

        // Apply force to the Rigidbody
        rb.AddForce(force + noise);

      
    }

    Vector2 ComputeSeparation()
    {
        Vector2 separationForce = Vector2.zero;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (Collider2D neighbor in neighbors)
        {
            if (neighbor.gameObject != gameObject && neighbor.gameObject.tag != "bounds")
            {
                Vector2 awayFromNeighbor = (Vector2)transform.position - (Vector2)neighbor.transform.position;
                separationForce += awayFromNeighbor.normalized / awayFromNeighbor.magnitude;
            }
        }
        return separationForce;
    }

    Vector2 ComputeAlignment()
    {
        Vector2 alignmentForce = Vector2.zero;
        int neighborCount = 0;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (Collider2D neighbor in neighbors)
        {
            if (neighbor.gameObject != gameObject && neighbor.gameObject.tag != "bounds")
            {
                alignmentForce += neighbor.GetComponent<Rigidbody2D>().velocity;
                neighborCount++;
            }
        }
        if (neighborCount > 0)
        {
            alignmentForce /= neighborCount;
            alignmentForce = alignmentForce.normalized;
        }
        return alignmentForce;
    }

    Vector2 ComputeCohesion()
    {
        Vector2 cohesionForce = Vector2.zero;
        int neighborCount = 0;
        Collider2D[] neighbors = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        foreach (Collider2D neighbor in neighbors)
        {
            if (neighbor.gameObject != gameObject && neighbor.gameObject.tag != "bounds")
            {
                cohesionForce += (Vector2)neighbor.transform.position;
                neighborCount++;
            }
        }
        if (neighborCount > 0)
        {
            cohesionForce /= neighborCount;
            cohesionForce = (cohesionForce - (Vector2)transform.position).normalized;
        }
        return cohesionForce;
    }
    void SetTargetVector()
    {
        targetPosition = targets[targetIndex].transform.position;
    }
    public bool IsAtTarget()
    {
        return Vector2.Distance(transform.position, targetPosition) < 0.5f;
    }
    void CollectTargets()
    {
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag("target");
        targets = new Transform[targetObjects.Length];
        for (int i = 0; i < targetObjects.Length; i++)
        {
            targets[i] = targetObjects[i].transform;
        }

        // Sort targets by name
        System.Array.Sort(targets, (t1, t2) => t1.name.CompareTo(t2.name));
    }


}
