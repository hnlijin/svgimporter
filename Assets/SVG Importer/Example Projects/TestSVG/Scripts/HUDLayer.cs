using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SVGImporter;

public class HUDLayer : MonoBehaviour {
	public GameMap gameMap;
 	public SVGRenderer preview;
    public TextAsset TxtFile1;
	public TextAsset TxtFile2;
	public TextAsset TxtFile3;
	public TextAsset TxtFile4;
	public TextAsset TxtFile5;
	public TextAsset TxtFile6;
	public TextAsset TxtFile7;
    protected List<SVGAsset> svgAsset;

	private void clearSvgAsset() {
		if(svgAsset != null) {
			for (int i = 0; i < svgAsset.Count; i++) {
				Destroy(svgAsset[i]);
			}
			svgAsset = null;
			preview.vectorGraphics = null;
			gameMap.ClearContentLayer();
        }
	}

	public void LoadTest1()
    {
		if (TxtFile1 == null) return;
		clearSvgAsset();
        svgAsset = SVGAsset.Load(TxtFile1.text);
        preview.vectorGraphics = svgAsset[0];
    }

	public void LoadTest2()
    {
		if (TxtFile2 == null) return;
		clearSvgAsset();
        svgAsset = SVGAsset.Load(TxtFile2.text);
        preview.vectorGraphics = svgAsset[0];
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
