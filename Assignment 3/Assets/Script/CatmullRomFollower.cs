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
        float rate = 0.1f;

        coroutineAllowed = false;

        if (posIndex == 4)
        {
            posIndex = 0;
        }

        Vector3 p0 = controlPointsList[CatmullRomSpline.ClampListPos(posIndex - 1, controlPointsList)].position;
        Vector3 p1 = controlPointsList[posIndex].position;
        Vector3 p2 = controlPointsList[CatmullRomSpline.ClampListPos(posIndex + 1, controlPointsList)].position;
        Vector3 p3 = controlPointsList[CatmullRomSpline.ClampListPos(posIndex + 2, controlPointsList)].position;
        posIndex += 1;

        while (t < 1)
        {
            t += Time.deltaTime * speed;
            pos = CatmullRomSpline.GetCatmullRomPosition(t, p0, p1, p2, p3);
            transform.position = pos;
            yield return new WaitForEndOfFrame();
        }

        transform.Rotate(Vector3.up, 90f);

        t = 0;

        coroutineAllowed = true;
    }
}
