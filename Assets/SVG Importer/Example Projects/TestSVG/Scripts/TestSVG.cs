using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGImporter;

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
	public TextAsset TxtFile3Data;

	public void LoadTest1()
    {
		if (TxtFile1 == null) return;
		gameMap.ParseSVGAsset(TxtFile1.text);
    }

	public void LoadTest2()
    {
		if (TxtFile2 == null) return;
		gameMap.ParseSVGAsset(TxtFile2.text);
		// 
    }

	public void LoadTest3()
	{
		if (TxtFile3 == null) return;
		gameMap.onGameMapChanged += LoadGameMapComplete3;
		gameMap.ParseSVGAsset(TxtFile3.text);
	}

	protected void LoadGameMapComplete3(GameMap map) 
	{
		gameMap.onGameMapChanged -= LoadGameMapComplete3;

		string jsonStr = "{\"list\":" + TxtFile3Data.text + "}";
		Response<FillDataItem> data = JsonUtility.FromJson<Response<FillDataItem>>(jsonStr);
		int index = 0;
		if (data.list != null && data.list.Count > 0) {
			for (int i = 0; i < data.list[index].p.Count; i++) {
				gameMap.FillColor(data.list[index].p[i], GameMap.EmptyColor);
				gameMap.AddCollider2D(data.list[index].p[i]);
			}
		}
		Color fillColor;
		ColorUtility.TryParseHtmlString(data.list[index].c, out fillColor);
		gameMap.setFillColor(fillColor);

		// gameMap.svgPreview.GetComponent<SVGRenderer>().onVectorGraphicsChanged(null);

		// gameMap.FillColor(483, GameMap.EmptyColor);
		// gameMap.FillColor(484, GameMap.EmptyColor);
		// gameMap.FillColor(485, GameMap.EmptyColor);
		// gameMap.FillColor(495, GameMap.EmptyColor);
		// gameMap.AddCollider2D(483);
		// gameMap.AddCollider2D(484);
		// gameMap.AddCollider2D(485);
		// gameMap.AddCollider2D(495);
	}

	public void LoadTest4()
	{
		if (TxtFile4 == null) return;
		gameMap.ParseSVGAsset(TxtFile4.text);
	}

	public void LoadTest5()
	{
		if (TxtFile5 == null) return;
		gameMap.onGameMapChanged += LoadGameMapComplete5;
		gameMap.ParseSVGAsset(TxtFile5.text);
	}

	protected void LoadGameMapComplete5(GameMap map) 
	{
		gameMap.onGameMapChanged -= LoadGameMapComplete5;
		gameMap.AddCollider2D(3);
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
		if (TxtFile8 == null) return;
		gameMap.ParseSVGAsset(TxtFile8.text);
	}

	public void LoadTest9()
	{
	}

	public void TestSVG_40_40() {
		gameMap.ClearSvgAsset();
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
		gameMap.ClearSvgAsset();
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
		gameMap.ClearSvgAsset();
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
