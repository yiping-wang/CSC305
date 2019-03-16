using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    public FlockManager myManager;
    float speed;
    bool turning = false;

	// Use this for initialization
	void Start () {
        speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
	}
	
	// Update is called once per frame
	void Update () {
        Bounds bounds = new Bounds(myManager.goalPos, myManager.moveLimits * 3);
        if (!bounds.Contains(transform.position))
        {
            turning = true;
        }
        else
        {
            turning = false;
        }

        if (turning)
        {
            Vector3 direction = myManager.goalPos - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                 Quaternion.LookRotation(direction),
                                                  myManager.rotationSpeed * Time.deltaTime);
        }
        else 
        {
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(myManager.minSpeed, myManager.maxSpeed);
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
        GameObject[] gos = myManager.allBoid;
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
                if (nDistance <= myManager.neighbourDistance)
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
                vcentre = vcentre / groupSize + (myManager.goalPos);
                speed = gSpeed / groupSize;
                Vector3 diretion = (vcentre + vavoid) - transform.position;
                if (diretion != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation,
                                                         Quaternion.LookRotation(diretion),
                                                          myManager.rotationSpeed * Time.deltaTime);
                }
            }
        }
    }
}
