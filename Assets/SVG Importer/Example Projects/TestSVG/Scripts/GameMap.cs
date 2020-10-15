using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap : MonoBehaviour {

	public GameObject svgObject;

	public GameObject contentLayer;

	public void ClearContentLayer() {
		while (contentLayer.transform.childCount > 0) {
			GameObject go = contentLayer.transform.GetChild (0).gameObject;
			go.transform.parent = null;
			Destroy (go);
		}
	}

	public void TestSVG_40_40() {
		ClearContentLayer();
		for (int i = -20; i < 20; i++) {
			for (int j = -20; j < 20; j++) {
				Vector3 p3 = this.svgObject.transform.position;
				p3.x = 0.5f * i;
				p3.y = 0.5f * j;
				GameObject go = Instantiate(this.svgObject, p3, this.svgObject.transform.rotation, contentLayer.transform);
				go.SetActive (true);
			}
		}
	}

	public void TestSVG_20_20() {
		ClearContentLayer();
		Vector3 vec3 = new Vector3 (0.2f, 0.2f);
		for (int i = -10; i < 10; i++) {
			for (int j = -10; j < 10; j++) {
				Vector3 p3 = this.svgObject.transform.position;
				p3.x = 1f * i;
				p3.y = 1f * j;
				GameObject go = Instantiate(this.svgObject, p3, this.svgObject.transform.rotation, contentLayer.transform);
				go.transform.localScale = vec3;
				go.SetActive (true);
			}
		}
	}

	public void TestSVG_10_10() {
		ClearContentLayer();
		Vector3 vec3 = new Vector3 (0.3f, 0.3f);
		for (int i = -5; i < 5; i++) {
			for (int j = -5; j < 5; j++) {
				Vector3 p3 = this.svgObject.transform.position;
				p3.x = 1.5f * i;
				p3.y = 1.5f * j;
				GameObject go = Instantiate(this.svgObject, p3, this.svgObject.transform.rotation, contentLayer.transform);
				go.transform.localScale = vec3;
				go.SetActive (true);
			}
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
