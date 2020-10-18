using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using SVGImporter;
using System.Collections.Generic;

public class RealtimeImportDemo : MonoBehaviour {

    public SVGImage preview;
    public InputField svgInput;

    public TextAsset TxtFile;

    protected List<SVGAsset> svgAsset;

    public void Load()
    {
        // if(svgInput == null || string.IsNullOrEmpty(svgInput.text)) return;
        // if(svgAsset != null)
        // {
        //     Destroy(svgAsset[0]);
        // }

        svgAsset = new List<SVGAsset>();
        StartCoroutine(SVGAsset.Load(TxtFile.text, svgAsset, null, 100));
        Debug.Log("load svg asset count: " + svgAsset.Count);
        for (int i = 1; i < svgAsset.Count; i++) {
            SVGImage newSVGImage = Instantiate(preview, preview.transform.position, preview.transform.rotation, preview.transform.parent);
            newSVGImage.vectorGraphics = svgAsset[i];
        }
        preview.vectorGraphics = svgAsset[0];
        
    }

}
