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

        svgAsset = SVGAsset.Load(TxtFile.text);
        preview.vectorGraphics = svgAsset[0];
    }

}
