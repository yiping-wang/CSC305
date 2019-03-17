using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour {

    public GameObject boidPrefab;
    public GameObject leader;
    public int numBoid = 50;
    public GameObject[] allBoid;
    public Vector3 moveLimits = new Vector3(3, 3, 3);
    public Vector3 goalPos;

    [Header("Boid Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 5.0f)]
    public float maxSpeed;
    [Range(1.0f, 10.0f)]
    public float neighbourDistance;
    [Range(0.0f, 5.0f)]
    public float rotationSpeed;

	void Start () 
    {
        goalPos = leader.transform.position;
        allBoid = new GameObject[numBoid];  
        for (int i = 0; i < numBoid; i++)
        {
            Vector3 pos = this.transform.position + new Vector3(Random.Range(-moveLimits.x, moveLimits.x),
                                                                Random.Range(-moveLimits.y, moveLimits.y),
                                                                Random.Range(-moveLimits.z, moveLimits.z));
            allBoid[i] = (GameObject)Instantiate(boidPrefab, pos, Quaternion.identity);
            allBoid[i].transform.parent = transform;
            allBoid[i].GetComponent<Flock>().flockManager = this;
        }

	}
	
	// Update is called once per frame
	void Update () {
        goalPos = leader.transform.position;
	}
}
