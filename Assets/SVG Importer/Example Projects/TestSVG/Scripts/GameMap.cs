using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGImporter;
using SVGImporter.Utils;

public class GameMap : MonoBehaviour {

	public static Color EmptyColor = new Color(0, 0, 0, 0);

	public GameObject contentLayer;
	public GameObject colliderLayer;
	public GameObject tipLayer;
	public List<SVGAsset> svgAssets {
		get {
			return _svgAssets;
		}
	}
	protected List<SVGAsset> _svgAssets;
	public SVGRenderer svgPreview;
	private int _sliceLayerNum = 100;
	private Color _fillColor = new Color(0, 1, 0);

	public void setFillColor(Color color) 
	{
		this._fillColor = color;
	}

	public void ParseSVGAsset(string svgText) 
	{
		ClearSvgAsset();
		StartCoroutine(ParseSVGAssetProgress(svgText));	
	}

	IEnumerator ParseSVGAssetProgress(string svgText)
	{
		yield return new WaitForEndOfFrame();
		_svgAssets = SVGAsset.Load(svgText, null, this._sliceLayerNum);
		for (int i = 1; i < _svgAssets.Count; i++) {
			GameObject svgGameObject = Instantiate(svgPreview.gameObject, svgPreview.transform.position, svgPreview.transform.rotation, contentLayer.transform);
			svgGameObject.GetComponent<SVGRenderer>().vectorGraphics = _svgAssets[i];
		}
		svgPreview.vectorGraphics = _svgAssets[0];
	}

	public void AddCollider2D(int targetLayerIndex) {
		if (_svgAssets == null) return;
		int verticeStartIndex = 0;
		int spliceLayerIndex = targetLayerIndex / this._sliceLayerNum;
		int startLayerIndex = spliceLayerIndex * this._sliceLayerNum;
		for (int i = startLayerIndex; i < targetLayerIndex; i++) {
			int totalShapes = _svgAssets[0].layers[i].shapes.Length;
			for(int j = 0; j < totalShapes; j++) {
				verticeStartIndex += _svgAssets[0].layers[i].shapes[j].vertexCount;
			}
		}
		GameObject obj = new GameObject("collider_" + targetLayerIndex);
		obj.transform.parent = colliderLayer.transform;
		obj.transform.localScale = svgPreview.transform.localScale;
		obj.transform.localPosition = Vector3.zero;
		PolygonCollider2D polygonCollider2D = obj.AddComponent<PolygonCollider2D>();
		SVGShape svgShape = _svgAssets[0].layers[targetLayerIndex].shapes[0];
		Vector2[] points = new Vector2[svgShape.triangles.Length];
		int vindex = 0;
		for (int i = 0; i < svgShape.triangles.Length; i++) {
			vindex = svgShape.triangles[i];
			points[i] = new Vector2(svgShape.vertices[vindex].x, svgShape.vertices[vindex].y);
		}
		polygonCollider2D.SetPath(0, points);
	}

	public void FillColor(int targetLayerIndex, Color color) {
		if (_svgAssets == null) return;
		int verticeStartIndex = 0;
		int spliceLayerIndex = targetLayerIndex / this._sliceLayerNum;
		int startLayerIndex = spliceLayerIndex * this._sliceLayerNum;
		for (int i = startLayerIndex; i < targetLayerIndex; i++) {
			int totalShapes = _svgAssets[0].layers[i].shapes.Length;
			for(int j = 0; j < totalShapes; j++) {
				verticeStartIndex += _svgAssets[0].layers[i].shapes[j].vertexCount;
			}
		}
		int vertexCount = _svgAssets[0].layers[targetLayerIndex].shapes[0].vertexCount;
		Color[] colors2 = _svgAssets[spliceLayerIndex].sharedMesh.colors;
		for (int j = 0; j < vertexCount; j++) {
			// Color color = colors2[verticeStartIndex + j];
			colors2[verticeStartIndex + j] = color;
		}
		_svgAssets[spliceLayerIndex].sharedMesh.colors = colors2;
	}

	private void SelectFillObject(string selectName)
	{
		Debug.Log ("SelectFillObject: selectName = " + selectName);
		string num = selectName.Replace("collider_", "");
		int targetLayerIndex = int.Parse(num);
		if (targetLayerIndex >= 0) {
			FillColor(targetLayerIndex, _fillColor);
		}
	}

	public void ClearContentLayer() {
		while (contentLayer.transform.childCount > 0) {
			GameObject go = contentLayer.transform.GetChild (0).gameObject;
			go.transform.parent = null;
			Destroy (go);
		}
	}

	public void ClearSvgAsset() {
		if(_svgAssets != null) {
			for (int i = 0; i < _svgAssets.Count; i++) {
				Destroy(_svgAssets[i]);
			}
			_svgAssets = null;	
        }
		svgPreview.vectorGraphics = null;
		this.ClearContentLayer();
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
			if (hit.collider != null)
			{
				if (hit.collider.name.IndexOf("collider_") >= 0) {
					this.SelectFillObject(hit.collider.name);
				}
			}
		}
	}
}
