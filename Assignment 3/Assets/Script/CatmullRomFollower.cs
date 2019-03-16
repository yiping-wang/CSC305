using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatmullRomFollower : MonoBehaviour {

    public Transform[] controlPointsList;

    int routeToGo;
    float t;
    Vector3 pos;
    float speed;
    bool coroutineAllowed;
    int posIndex = 0;

	void Start () 
    {
        t = 0;
        speed = 0.5f;
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
        float rate = 0.1f;

        coroutineAllowed = false;

        if (posIndex == 4)
        {
            posIndex = 0;
        }

        Vector3 p0 = controlPointsList[ClampListPos(posIndex - 1)].position;
        Vector3 p1 = controlPointsList[posIndex].position;
        Vector3 p2 = controlPointsList[ClampListPos(posIndex + 1)].position;
        Vector3 p3 = controlPointsList[ClampListPos(posIndex + 2)].position;
        posIndex += 1;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            pos = GetCatmullRomPosition(t, p0, p1, p2, p3);
            transform.position = pos;
            yield return new WaitForEndOfFrame();
        }

        transform.Rotate(Vector3.up, 90f);

        t = 0;

        coroutineAllowed = true;
    }


    Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //The coefficients of the cubic polynomial (except the 0.5f * which I added later for performance)
        Vector3 a = 2f * p1;
        Vector3 b = p2 - p0;
        Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
        Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;

        //The cubic polynomial: a + b * t + c * t^2 + d * t^3
        return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
    }

    int ClampListPos(int pos)
    {
        if (pos < 0)
        {
            pos = controlPointsList.Length - 1;
        }

        if (pos > controlPointsList.Length)
        {
            pos = 1;
        }
        else if (pos > controlPointsList.Length - 1)
        {
            pos = 0;
        }

        return pos;
    }
}
