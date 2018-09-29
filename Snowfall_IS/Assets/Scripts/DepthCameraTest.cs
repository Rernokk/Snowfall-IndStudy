using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthCameraTest : MonoBehaviour {
	[SerializeField]
	RenderTexture rTex;

	[SerializeField]
	LayerMask mask;

	[SerializeField]
	FlowmapGenerator river;

	void Start()
	{
		rTex = new RenderTexture(512, 512, 24);

		Camera cam = GetComponent<Camera>();
		cam.depthTextureMode = DepthTextureMode.Depth;

		cam.cullingMask = mask;
		cam.targetTexture = rTex;
		cam.Render();
		RenderTexture.active = rTex;

		Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
		tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
		
		tex.Apply();
		cam.targetTexture = null;
		RenderTexture.active = null;

		if (transform.name == "OverheadA"){
			//river.texA = tex;
			river.texA2D = tex;
		} else {
			//river.texB = tex;
			river.texB2D = tex;
		}
		//river.CalculateFlowMap();
	}
}
