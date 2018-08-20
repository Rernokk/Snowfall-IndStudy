using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowmapGenerator : MonoBehaviour {
	[SerializeField]
	Texture2D maskMap, flowMap;
	// Use this for initialization
	void Start () {
		maskMap = new Texture2D(400, 400);
		for (int i = 0; i < maskMap.width; i++)
		{
			for (int j = 0; j < maskMap.height; j++)
			{
				RaycastHit inf;
				Physics.Raycast(new Ray(new Vector3(i * .25f, 40, j * .25f), Vector3.down), out inf, 100f);
				if (inf.transform == null || inf.transform.tag == "Terrain"){
					maskMap.SetPixel(i, j, Color.black);
				} else {
					maskMap.SetPixel(i, j, Color.white);
				}
			}
		}
		Color[,] colArray = new Color[maskMap.width, maskMap.height];
		for (int i = 0; i < maskMap.width; i++){
			for (int j = 0; j < maskMap.height; j++){
				colArray[i, j] = maskMap.GetPixel(i, j);
			}
		}

		//Blur
		for (int i = 0; i < maskMap.width; i++){
			for (int j = 0; j < maskMap.height; j++){
				Color temp = new Color(0, 0, 0);
				for (int a = -1; a <= 1; a++)
				{
					for (int b = -1; b <= 1; b++){
						if (a + i >= 0 && a + i < maskMap.width && b + j >= 0 && b + j < maskMap.height){
							temp += colArray[i + a, j + b];
						}
					}
				}
				temp /= 9.0f;
				maskMap.SetPixel(i, j, temp);
			}
		}

		maskMap.Apply();

		flowMap = new Texture2D(400, 400);
		for (int i = 0; i < flowMap.width; i++){
			for (int j = 0; j < flowMap.height; j++){
				if (maskMap.GetPixel(i, j) == Color.white || true)
				{
					RaycastHit inf;
					Physics.Raycast(new Ray(new Vector3(i * .25f, 40, j * .25f), Vector3.down), out inf, 100f, LayerMask.NameToLayer("Terrain"));
					Vector3 normDir = inf.normal;
					normDir.y = 0;
					normDir.Normalize();
					flowMap.SetPixel(i, j, new Color(.5f + -normDir.x * .02f, .5f + -normDir.z * .02f, 0));
				}
			}
		}
		flowMap.Apply();

		GetComponent<MeshRenderer>().material.SetTexture("_MaskTex", maskMap);
		GetComponent<MeshRenderer>().material.SetTexture("_FlowTex", flowMap);
		//GetComponent<MeshRenderer>().material.SetTexture("_MainTex", maskMap);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
