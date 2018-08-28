using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlowmapGenerator : MonoBehaviour {
	[SerializeField]
	public Texture2D maskMap, flowMap;

	[SerializeField]
	LayerMask mask;

	[SerializeField]
	float cutFactor = .05f;

	Material mat;

	// Use this for initialization
	void Start () {
		maskMap = new Texture2D(512, 512);
		for (int i = 0; i < maskMap.width; i++)
		{
			for (int j = 0; j < maskMap.height; j++)
			{
				RaycastHit inf;
				Physics.Raycast(new Ray(new Vector3(i * 100.0f / maskMap.width, 50, j * 100.0f / maskMap.height), Vector3.down), out inf, 100f);
				if (inf.transform == null || inf.transform.tag == "Terrain"){
					maskMap.SetPixel(i, j, Color.black);
				} else {
					maskMap.SetPixel(i, j, Color.white);
				}
			}
		}

		flowMap = new Texture2D(512, 512);
		for (int i = 0; i < flowMap.width; i++){
			for (int j = 0; j < flowMap.height; j++){
				if (maskMap.GetPixel(i, j) == Color.white)
				{
					RaycastHit inf;
					Physics.Raycast(new Ray(new Vector3(i * 100.0f / flowMap.width, 40, j * 100.0f / flowMap.height), Vector3.down), out inf, 100f, mask);
					Vector3 normDir = inf.normal;
					normDir.y = 0;
					normDir.Normalize();
					flowMap.SetPixel(i, j, new Color(Mathf.Clamp01(.5f + -normDir.x * cutFactor), Mathf.Clamp01(.5f + -normDir.z * cutFactor), 0));
				} else {
					flowMap.SetPixel(i, j, new Color(.5f, .5f, 0));
				}
			}
		}
		flowMap.Apply();

		mat = GetComponent<MeshRenderer>().material;
		mat.SetTexture("_FlowTex", flowMap);
	}

	// Update is called once per frame
	void Update () {

	}
}
