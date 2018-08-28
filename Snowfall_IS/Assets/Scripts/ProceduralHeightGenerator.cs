using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralHeightGenerator : MonoBehaviour {
	[SerializeField]
	Terrain terrain;

	[SerializeField]
	Texture2D terrainTexture;
	Texture2D noiseTex;
	FastNoise myNoise;
	void Start () {
		myNoise = new FastNoise((int)Time.time);
		myNoise.SetNoiseType(FastNoise.NoiseType.Perlin);

		terrainTexture = new Texture2D((int)terrain.terrainData.bounds.size.x * 2, (int)terrain.terrainData.bounds.size.z * 2);
		print(terrainTexture.width + ", " + terrainTexture.height);
		float max = terrain.terrainData.bounds.size.y;
		for (int i = 0; i < terrainTexture.width; i++)
		{
			for (int j = 0; j < terrainTexture.height; j++)
			{
				if (terrain.terrainData.GetInterpolatedHeight((float)i / terrainTexture.width, (float)j / terrainTexture.height) <= transform.position.y)
				{
					Vector3 val = new Vector3(.5f, .5f, 0f);
					Vector3 res = terrain.terrainData.GetInterpolatedNormal((float)i / terrainTexture.width, (float)j / terrainTexture.height) * .5f;
					res.y = res.z;
					res.z = 0;
					res.Normalize();
					val -= res;
					terrainTexture.SetPixel(i, j, new Color(val.x, val.y, 0));
				} else {
					terrainTexture.SetPixel(i, j, new Color(.5f, .5f, 0));
				}
			}
		}
		terrainTexture.Apply();

		GetComponent<MeshRenderer>().material.SetTexture("_FlowTex", terrainTexture);

		noiseTex = new Texture2D(32, 32);
		for (int i = 0; i < noiseTex.width; i++){
			for (int j = 0; j < noiseTex.height; j++){
				float val = myNoise.GetNoise(i, j);
				noiseTex.SetPixel(i, j, new Color(val, val, val, 1));
			}
		}
		noiseTex.Apply();

		//GetComponent<MeshRenderer>().material.SetTexture("_MainTex", noiseTex);
		GetComponent<MeshRenderer>().enabled = true;
		//terrain.terrainData
	}
	
	// Update is called once per frame
	void Update () {
		//for (int i = 0; i < noiseTex.width; i++)
		//{
		//	for (int j = 0; j < noiseTex.height; j++)
		//	{
		//		float val = myNoise.GetNoise(i, j);
		//		noiseTex.SetPixel(i, j, new Color(val, val, val, 1));
		//	}
		//}
		//noiseTex.Apply();
	}
}
