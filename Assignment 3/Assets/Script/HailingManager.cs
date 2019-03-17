using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HailingManager : MonoBehaviour 
{

    public GameObject fallPrefab;
    public int numFalls;
    public float minSpeed;
    public float maxSpeed;
    public float height;
    public float spawnInterval;

	// Use this for initialization
	void Start () 
    {
        Transform[] controlPoints = FindObjectOfType<CatmullRomSpline>().controlPointsList;
        Vector3 p0 = controlPoints[0].position;
        Vector3 p1 = controlPoints[1].position;
        Vector3 p2 = controlPoints[2].position;
        Vector3 p3 = controlPoints[3].position;
        transform.position = ((p0 + p2) / 2f + (p1 + p3) / 2f) / 2f;
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
	}

	void Update()
	{
        int currFalls = GameObject.FindGameObjectsWithTag("FallingObject").Length;
        if (currFalls < numFalls)
        {
            for (int i = 0; i < numFalls; i++)
            {
                GameObject falling = (GameObject)Instantiate(fallPrefab, transform.position, Quaternion.identity);
                falling.GetComponent<Hailing>().hailingManager = this;
                Vector3 direction = new Vector3(Random.Range(-10, 10), -5, Random.Range(-10, 10));
                falling.transform.forward = direction.normalized;
            }
        }
	}
}
