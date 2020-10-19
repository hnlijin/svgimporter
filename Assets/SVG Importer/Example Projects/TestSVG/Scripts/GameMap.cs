using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGImporter;
using SVGImporter.Utils;

public class GameMap : MonoBehaviour {

	public static Color EmptyColor = new Color(0, 0, 0, 0);
	public GameCameraCtrl cameraCtrl;
	public GameObject contentLayer;
	public GameObject colliderLayer;
	public GameObject tipLayer;
	public HUDLayer hudLayer;
	public List<SVGAsset> svgAssets {
		get {
			return _svgAssets;
		}
	}
	public System.Action<GameMap> onGameMapChanged;
	protected List<SVGAsset> _svgAssets = new List<SVGAsset>();
	public SVGRenderer svgPreview;
	private List<GameObject> previewPool = new List<GameObject>();
	private List<GameObject> colliderPool = new List<GameObject>();
	private int _sliceLayerNum = 100;
	private Color _fillColor = new Color(0, 1, 0);

	public void setFillColor(Color color) 
	{
		this._fillColor = color;
	}

	public void ParseSVGAsset(string svgText) 
	{
		hudLayer.showLoading();
		ClearSvgAsset();
		StartCoroutine(ParseSVGAssetProgress(svgText));	
	}

	IEnumerator ParseSVGAssetProgress(string svgText)
	{
		yield return new WaitForEndOfFrame();
		yield return StartCoroutine(SVGAsset.Load(svgText, _svgAssets, null, this._sliceLayerNum));
		hudLayer.hideLoading();
		for (int i = 1; i < _svgAssets.Count; i++) {
			GameObject svgGameObject = null;
			if (previewPool.Count > 0) {
				svgGameObject = previewPool[0];
				svgGameObject.transform.parent = contentLayer.transform;
				svgGameObject.transform.position = svgPreview.transform.position;
				previewPool.RemoveAt(0);
			}
			if (svgGameObject == null) {
				svgGameObject = Instantiate(svgPreview.gameObject, svgPreview.transform.position, svgPreview.transform.rotation, contentLayer.transform);
			}
			svgGameObject.GetComponent<SVGRenderer>().vectorGraphics = _svgAssets[i];
			yield return new WaitForEndOfFrame();
		}
		svgPreview.vectorGraphics = _svgAssets[0];
		if (onGameMapChanged != null) {
			onGameMapChanged(this);
		}
	}

	public void AddCollider2D(int targetLayerIndex) {
		if (_svgAssets == null || _svgAssets.Count <= 0) return;
		int verticeStartIndex = 0;
		int spliceLayerIndex = targetLayerIndex / this._sliceLayerNum;
		int startLayerIndex = spliceLayerIndex * this._sliceLayerNum;
		for (int i = startLayerIndex; i < targetLayerIndex; i++) {
			int totalShapes = _svgAssets[0].layers[i].shapes.Length;
			for(int j = 0; j < totalShapes; j++) {
				verticeStartIndex += _svgAssets[0].layers[i].shapes[j].vertexCount;
			}
		}
		GameObject obj = null;
		PolygonCollider2D polygonCollider2D = null;
		if(colliderPool.Count > 0) {
			obj = colliderPool[0];
			colliderPool.RemoveAt(0);
			polygonCollider2D = obj.GetComponent<PolygonCollider2D>();
			obj.name = "collider_" + targetLayerIndex;
		}
		if (obj == null) {
			obj = new GameObject("collider_" + targetLayerIndex);
		}
		obj.transform.parent = colliderLayer.transform;
		obj.transform.localScale = svgPreview.transform.localScale;
		obj.transform.position = svgPreview.transform.position;
		if (polygonCollider2D == null)
			polygonCollider2D = obj.AddComponent<PolygonCollider2D>();
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
		if (_svgAssets == null || _svgAssets.Count <= 0) return;
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

	public void ClearSvgAsset() {
		if(_svgAssets != null) {
			while(_svgAssets.Count > 0) {
				Destroy(_svgAssets[0]);
				_svgAssets.RemoveAt(0);
			}	
        }
		svgPreview.vectorGraphics = null;
		this.ClearContentLayer();
		this.ClearColliderLayer();
	}

	private void ClearContentLayer() {
		while (contentLayer.transform.childCount > 0) {
			GameObject obj = contentLayer.transform.GetChild (0).gameObject;
			SVGRenderer svgRenderer = obj.GetComponent<SVGRenderer>();
			if (svgRenderer != null) svgRenderer.vectorGraphics = null;
			obj.transform.parent = null;
			previewPool.Add(obj);
		}
	}

	private void ClearColliderLayer() {
		while (colliderLayer.transform.childCount > 0) {
			GameObject obj = colliderLayer.transform.GetChild (0).gameObject;
			obj.transform.parent = null;
			colliderPool.Add(obj);
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) && cameraCtrl.isTouchMoved() == false)
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
