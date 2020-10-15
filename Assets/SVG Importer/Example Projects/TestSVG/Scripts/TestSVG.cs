using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSVG : MonoBehaviour {

	private bool _colorflag = false;

	// Use this for initialization
	void Start () {
		SVGImporter.SVGRenderer svgRender = GetComponent<SVGImporter.SVGRenderer> ();
        MeshRenderer meshRenderer = svgRender.meshRenderer;
	}
	
	// Update is called once per frame
	void Update () {
		SVGImporter.SVGRenderer svgRender = GetComponent<SVGImporter.SVGRenderer> ();
		Material[] arr = svgRender.vectorGraphics.sharedMaterials;
		if (svgRender.vectorGraphics.layers.GetLength(0) > 0 && _colorflag == false) {
			Color32 finalColor = new Color32 ();
			finalColor.a = 1;
			finalColor.r = 1;
			finalColor.g = 1;
			finalColor.b = 0;
			arr [0].color = finalColor;
			svgRender.vectorGraphics.layers [0].shapes [0].colors [0].r = 1;
			_colorflag = true;
		}

		if (Input.GetMouseButtonDown(0))
		{
			Vector2 mouseV2 = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast(mouseV2, Vector2.zero);
			if (hit.collider!=null)
			{
				Debug.Log ("click");
			}
		}
	}
}
