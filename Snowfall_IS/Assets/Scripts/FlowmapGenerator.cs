using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlowmapGenerator : MonoBehaviour {
	[SerializeField]
	Texture2D maskMap, flowMap;

	[SerializeField]
	LayerMask mask;

	// Use this for initialization
	void Start () {
		maskMap = new Texture2D(1024, 1024);
		for (int i = 0; i < maskMap.width; i++)
		{
			for (int j = 0; j < maskMap.height; j++)
			{
				RaycastHit inf;
				Physics.Raycast(new Ray(new Vector3(i * 100.0f / maskMap.width, 40, j * 100.0f / maskMap.height), Vector3.down), out inf, 100f);
				if (inf.transform == null || inf.transform.tag == "Terrain"){
					maskMap.SetPixel(i, j, Color.black);
				} else {
					maskMap.SetPixel(i, j, Color.white);
				}
			}
		}

		//Blur Alpha
		//Color[,] colArray = new Color[maskMap.width, maskMap.height];
		//for (int i = 0; i < maskMap.width; i++)
		//{
		//	for (int j = 0; j < maskMap.height; j++)
		//	{
		//		colArray[i, j] = maskMap.GetPixel(i, j);
		//	}
		//}
		//for (int i = 0; i < maskMap.width; i++){
		//	for (int j = 0; j < maskMap.height; j++){
		//		Color temp = new Color(0, 0, 0);
		//		for (int a = -1; a <= 1; a++)
		//		{
		//			for (int b = -1; b <= 1; b++){
		//				if (a + i >= 0 && a + i < maskMap.width && b + j >= 0 && b + j < maskMap.height){
		//					temp += colArray[i + a, j + b];
		//				}
		//			}
		//		}
		//		temp /= 9.0f;
		//		maskMap.SetPixel(i, j, temp);
		//	}
		//}

		//maskMap.Apply();

		flowMap = new Texture2D(1024, 1024);
		for (int i = 0; i < flowMap.width; i++){
			for (int j = 0; j < flowMap.height; j++){
				if (maskMap.GetPixel(i, j) == Color.white)
				{
					RaycastHit inf;
					Physics.Raycast(new Ray(new Vector3(i * 100.0f / flowMap.width, 40, j * 100.0f / flowMap.height), Vector3.down), out inf, 100f, mask);
					Vector3 normDir = inf.normal;
					normDir.y = 0;
					normDir.Normalize();
					flowMap.SetPixel(i, j, new Color(Mathf.Clamp01(.5f + -normDir.x * .05f), Mathf.Clamp01(.5f + -normDir.z * .05f), 0));
				} else {
					flowMap.SetPixel(i, j, new Color(.5f, .5f, 0));
				}
			}
		}

		//Blur Flow
		//colArray = new Color[flowMap.width, flowMap.height];
		//for (int i = 0; i < flowMap.width; i++)
		//{
		//	for (int j = 0; j < flowMap.height; j++)
		//	{
		//		colArray[i, j] = flowMap.GetPixel(i, j);
		//	}
		//}

		//for (int k = 0; k < 1; k++)
		//{
		//	for (int i = 0; i < flowMap.width; i++)
		//	{
		//		for (int j = 0; j < flowMap.height; j++)
		//		{
		//			Color temp = new Color(0, 0, 0);
		//			for (int a = -1; a <= 1; a++)
		//			{
		//				for (int b = -1; b <= 1; b++)
		//				{
		//					if (a + i >= 0 && a + i < flowMap.width && b + j >= 0 && b + j < flowMap.height)
		//					{
		//						temp += colArray[i + a, j + b];
		//					}
		//				}
		//			}
		//			temp /= 9.0f;
		//			colArray[i, j] = temp;
		//		}
		//	}
		//}

		//for (int i = 0; i < flowMap.width; i++){
		//	for (int j = 0; j < flowMap.height; j++){
		//		flowMap.SetPixel(i, j, colArray[i,j]);
		//	}
		//}

		//byte[] bytes = flowMap.EncodeToPNG();
		//File.WriteAllBytes(Application.dataPath + "/../GeneratedFlowMap.png", bytes);
		flowMap.Apply();

		GetComponent<MeshRenderer>().material.SetTexture("_MaskTex", maskMap);
		GetComponent<MeshRenderer>().material.SetTexture("_FlowTex", flowMap);
		//GetComponent<MeshRenderer>().material.SetTexture("_MainTex", flowMap);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
