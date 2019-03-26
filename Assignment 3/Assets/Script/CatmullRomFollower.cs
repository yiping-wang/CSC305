using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomFollower : MonoBehaviour {

    public Transform[] controlPointsList;

    float t;
    Vector3 pos;
    float speed;
    bool coroutineAllowed;
    int controlPointsIndex;
    float distance_to_travel = 0.01f;
    float very_small_amount = 0.05f;
	void Start () 
    {
        t = 0;
        controlPointsIndex = 0;
        speed = 0.2f;
        coroutineAllowed = true;
	}
	
	void Update () 
    {
        if (coroutineAllowed)
        {
            StartCoroutine(Mover());
        }
	}

    IEnumerator Mover()
    {
        coroutineAllowed = false;

        if (controlPointsIndex == 4)
        {
            controlPointsIndex = 0;
        }

        Vector3 p0 = controlPointsList[CatmullRomSpline.ClampListPos(controlPointsIndex - 1, controlPointsList)].position;
        Vector3 p1 = controlPointsList[controlPointsIndex].position;
        Vector3 p2 = controlPointsList[CatmullRomSpline.ClampListPos(controlPointsIndex + 1, controlPointsList)].position;
        Vector3 p3 = controlPointsList[CatmullRomSpline.ClampListPos(controlPointsIndex + 2, controlPointsList)].position;
        controlPointsIndex += 1;
        float travelled = 0;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            pos = CatmullRomSpline.GetCatmullRomPosition(t, p0, p1, p2, p3);

            // Numerical Methods for Arc Length
            while (travelled < distance_to_travel)
            {
                t += very_small_amount;
                travelled += (CatmullRomSpline.GetCatmullRomPosition(t, p0, p1, p2, p3) - pos).magnitude;
                pos = CatmullRomSpline.GetCatmullRomPosition(t, p0, p1, p2, p3);
            }

            Vector3 newDir = Vector3.RotateTowards(transform.forward, (pos - transform.position).normalized, Time.deltaTime * speed * 10, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.position = pos;
            Debug.DrawRay(transform.position, newDir, Color.green);
            yield return new WaitForEndOfFrame();
        }

        t = 0;

        coroutineAllowed = true;
    }
}
