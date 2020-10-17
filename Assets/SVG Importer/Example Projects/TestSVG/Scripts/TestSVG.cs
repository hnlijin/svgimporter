using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSVG : MonoBehaviour {

	public GameMap gameMap;
	public GameObject svgObject;
	public TextAsset TxtFile1;
	public TextAsset TxtFile2;
	public TextAsset TxtFile3;
	public TextAsset TxtFile4;
	public TextAsset TxtFile5;
	public TextAsset TxtFile7;
	public TextAsset TxtFile8;

	public void LoadTest1()
    {
		if (TxtFile1 == null) return;
		gameMap.ParseSVGAsset(TxtFile1.text);
    }

	public void LoadTest2()
    {
		if (TxtFile2 == null) return;
		gameMap.ParseSVGAsset(TxtFile2.text);
    }

	public void LoadTest3()
	{
		if (TxtFile2 == null) return;
		gameMap.ParseSVGAsset(TxtFile3.text);
		StartCoroutine(DoFillFunction());
	}

	IEnumerator DoFillFunction() 
	{
		yield return new WaitForEndOfFrame();
		gameMap.FillColor(483, GameMap.EmptyColor);
		gameMap.FillColor(484, GameMap.EmptyColor);
		gameMap.FillColor(485, GameMap.EmptyColor);
		gameMap.FillColor(495, GameMap.EmptyColor);
		gameMap.AddCollider2D(483);
		gameMap.AddCollider2D(484);
		gameMap.AddCollider2D(485);
		gameMap.AddCollider2D(495);
	}

	public void LoadTest4()
	{
		if (TxtFile2 == null) return;
		gameMap.ParseSVGAsset(TxtFile4.text);
	}

	public void LoadTest5()
	{
		if (TxtFile2 == null) return;
		gameMap.ParseSVGAsset(TxtFile5.text);
	}

	public void LoadTest6() {
		Mesh mesh = gameMap.svgAssets[0].sharedMesh;
		Vector3[] vertices = mesh.vertices;
		Color[] colors = mesh.colors;
		colors[0] = Color.red;
		colors[1] = Color.red;
		colors[2] = Color.red;
		mesh.colors = colors;
	}

	public void LoadTest7()
	{
	}

	public void LoadTest8()
	{
	}

	public void LoadTest9()
	{
	}

	public void TestSVG_40_40() {
		gameMap.ClearContentLayer();
		for (int i = -20; i < 20; i++) {
			for (int j = -20; j < 20; j++) {
				Vector3 p3 = svgObject.transform.position;
				p3.x = 0.5f * i;
				p3.y = 0.5f * j;
				GameObject go = Instantiate(svgObject, p3, svgObject.transform.rotation, gameMap.contentLayer.transform);
				go.SetActive (true);
			}
		}
	}

	public void TestSVG_20_20() {
		gameMap.ClearContentLayer();
		Vector3 vec3 = new Vector3 (0.2f, 0.2f);
		for (int i = -10; i < 10; i++) {
			for (int j = -10; j < 10; j++) {
				Vector3 p3 = svgObject.transform.position;
				p3.x = 1f * i;
				p3.y = 1f * j;
				GameObject go = Instantiate(svgObject, p3, svgObject.transform.rotation, gameMap.contentLayer.transform);
				go.transform.localScale = vec3;
				go.SetActive (true);
			}
		}
	}

	public void TestSVG_10_10() {
		gameMap.ClearContentLayer();
		Vector3 vec3 = new Vector3 (0.3f, 0.3f);
		for (int i = -5; i < 5; i++) {
			for (int j = -5; j < 5; j++) {
				Vector3 p3 = svgObject.transform.position;
				p3.x = 1.5f * i;
				p3.y = 1.5f * j;
				GameObject go = Instantiate(svgObject, p3, svgObject.transform.rotation, gameMap.contentLayer.transform);
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
