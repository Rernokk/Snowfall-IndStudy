using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFlowGenerator : MonoBehaviour {
	List<Collider> colls = new List<Collider>();

	[SerializeField]
	Texture2D genMap;
	// Use this for initialization
	void Start () {
		genMap = new Texture2D(200, 200);
		genMap.wrapMode = TextureWrapMode.Repeat;
		GenerateTexture();
		GetComponent<MeshRenderer>().material.SetTexture("_MainTex", genMap);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.G))
		{
			RegisterCollisions(1);
		}
	}

	void OnTriggerEnter(Collider col){
		if (colls.IndexOf(col) == -1){
			colls.Add(col);
		}
	}

	void OnTriggerExit(Collider col){
		if (colls.IndexOf(col) != -1){
			colls.Remove(col);
		}
	}

	void OnTriggerStay(Collider col){
		if (colls.IndexOf(col) == -1){
			colls.Add(col);
		}
	}

	void RegisterCollisions(int lim){
		print(colls.Count);
		foreach (Collider coll in colls){
			Vector3 botLeft = transform.position - GetComponent<Collider>().bounds.extents;
			Vector3 topRight = transform.position + GetComponent<Collider>().bounds.extents;
			botLeft.y = 0;
			topRight.y = 0;
			Vector3 refVector = coll.transform.position;
			int resX = (int)(genMap.width * ((refVector.x - botLeft.x) / (topRight.x - botLeft.x)));
			int resY = (int)(genMap.height * ((refVector.z - botLeft.z) / (topRight.z - botLeft.z)));
			print("X: " + resX);
			print("Y: " + resY);
			for (int i = -5; i <= 5; i++)
			{
				for (int j = -5; j <= 5; j++)
				{
					genMap.SetPixel(resX + i, resY + j, Color.blue);
				}
			}
		}
		genMap.Apply();
	}

	void GenerateTexture(){
		for (int i = 0; i < genMap.width; i++){
			for (int j = 0; j < genMap.height; j++){
				genMap.SetPixel(i, j, new Color(.5f, .5f, 0));
				//genMap.SetPixel(i,j, new Color ((float) i / 100.0f, (float) j / 100.0f, 1));
			}
		}
		genMap.Apply();
	}
}
