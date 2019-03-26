using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    public FlockManager flockManager;
    float speed;
    bool turning = false;

	// Use this for initialization
	void Start () {
        speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
	}
	
	// Update is called once per frame
	void Update () {
        Bounds bounds = new Bounds(flockManager.goalPos, flockManager.moveLimits * 3);

        RaycastHit hit = new RaycastHit();
        Vector3 direction = flockManager.goalPos - transform.position;

        if (!bounds.Contains(transform.position))
        {
            turning = true;
        }
        else if (Physics.Raycast(transform.position, this.transform.forward * 50, out hit))
        {
            turning = true;
            // direction = Vector3.Reflect(this.transform.forward, hit.normal).normalized;
            if(Random.Range(0, 1f) < 0.5)
            {
                direction = Vector3.left;
            }
            else 
            {
                direction = Vector3.right;
            }

            Debug.DrawRay(this.transform.position, this.transform.forward * 5, Color.red);
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                 Quaternion.LookRotation(direction),
                                                  flockManager.rotationSpeed * 10 * Time.deltaTime);
        }
        else 
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(flockManager.minSpeed, flockManager.maxSpeed);
            }
            if (Random.Range(0, 100) < 20)
            {
                ApplyRules();
            }
        }

        transform.Translate(0, 0, Time.deltaTime * speed);
	}

    void ApplyRules()
    {
        GameObject[] gos = flockManager.allBoid;
        Vector3 vcentre = Vector3.zero;
        Vector3 vavoid = Vector3.zero;
        float gSpeed = 0.01f;
        float nDistance;
        int groupSize = 0;

        foreach(GameObject go in gos)
        {
            if (go != this.gameObject)
            {
                
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                if (nDistance <= flockManager.neighbourDistance)
                {
                    vcentre += go.transform.position;
                    groupSize++;

                    if (nDistance < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }

            if (groupSize > 0)
            {
                vcentre = vcentre / groupSize + (flockManager.goalPos);
                speed = gSpeed / groupSize;
                Vector3 diretion = (vcentre + vavoid) - transform.position;
                if (diretion != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                                         Quaternion.LookRotation(diretion),
                                                          flockManager.rotationSpeed * Time.deltaTime);
                }
            }
        }
    }

	void OnCollisionEnter(Collision collision)
	{
        if (collision.gameObject.tag == "Boid")
        {
            Collider collider = GetComponent<Collider>();
            Physics.IgnoreCollision(collision.collider, collider);
        }
	}
}
