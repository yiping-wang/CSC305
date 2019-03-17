using UnityEngine;
using System.Collections;

public class CatmullRomSpline : MonoBehaviour
{
    public Transform[] controlPointsList;

	void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        for (int i = 0; i < controlPointsList.Length; i++)
        {
            DisplayCatmullRomSpline(i);
        }
    }

    void DisplayCatmullRomSpline(int pos)
    {
        Vector3 p0 = controlPointsList[ClampListPos(pos - 1)].position;
        Vector3 p1 = controlPointsList[pos].position;
        Vector3 p2 = controlPointsList[ClampListPos(pos + 1)].position;
        Vector3 p3 = controlPointsList[ClampListPos(pos + 2)].position;

        Vector3 lastPos = p1;

        float resolution = 0.1f;

        int loops = Mathf.FloorToInt(1f / resolution);

        for (int i = 1; i <= loops; i++)
        {
            float t = i * resolution;

            Vector3 newPos = GetCatmullRomPosition(t, p0, p1, p2, p3);

            Gizmos.DrawLine(lastPos, newPos);

            lastPos = newPos;
        }
    }

    public static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
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

    public static int ClampListPos(int pos, Transform[] controlPointsList)
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