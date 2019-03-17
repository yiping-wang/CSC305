﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hailing : MonoBehaviour {

    public HailingManager hailingManager;
    float speed;

	void Start () 
    {
        speed = Random.Range(hailingManager.minSpeed, hailingManager.maxSpeed);
	}
	
	void Update () 
    {
        transform.Translate(0, 0, Time.deltaTime * speed);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(transform.position, this.transform.forward, out hit))
        {
            Vector3 pos = hit.collider.transform.position;
            hit.collider.transform.Translate(transform.forward * 0.1f);
            Debug.DrawRay(this.transform.position, this.transform.forward, Color.yellow);
            Destroy(gameObject);
        }
	}

	void OnBecameInvisible()
	{
		Destroy (gameObject);
	}
}