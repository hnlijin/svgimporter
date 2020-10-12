using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour {

	public GameObject svgObject;

	// Use this for initialization
	void Start () {
		for (int i = -20; i < 20; i++) {
			for (int j = -20; j < 20; j++) {
				Vector3 p3 = this.svgObject.transform.position;
				p3.x = 3.5f * i;
				p3.y = 3.5f * j;
				Instantiate(this.svgObject, p3, this.svgObject.transform.rotation, this.transform);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
