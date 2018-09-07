using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralHeightGenerator : MonoBehaviour
{
	[SerializeField]
	Terrain terrain;

	[SerializeField]
	Texture2D terrainTexture;

	[SerializeField]
	bool includeTerrain, includeGradientBias;

	Texture2D noiseTex;
	FastNoise myNoise;

	[SerializeField]
	float weightScale = 1f, gradientScale = 1f;

	void Start()
	{
		myNoise = new FastNoise((int)Time.time);
		myNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
		if (terrain != null)
		{
			terrainTexture = new Texture2D((int)terrain.terrainData.bounds.size.x, (int)terrain.terrainData.bounds.size.z);
			terrainTexture.filterMode = FilterMode.Trilinear;
			
			float max = terrain.terrainData.bounds.size.y;
			for (int i = 0; i < terrainTexture.width; i++)
			{
				for (int j = 0; j < terrainTexture.height; j++)
				{
					Vector3 val = new Vector3(.5f, .5f);
					if (includeTerrain)
					{
						Vector3 res = Vector3.zero;
						res += terrain.terrainData.GetInterpolatedNormal((float) Mathf.Clamp((i), 0, terrainTexture.width) / terrainTexture.width,
							(float)Mathf.Clamp((j), 0, terrainTexture.height) / terrainTexture.height) * weightScale;
						val.x -= res.x;
						val.y -= res.z;
					}
					if (includeGradientBias)
					{
						Vector3 gradientBias = transform.up * gradientScale;
						val.x -= gradientBias.x;
						val.y -= gradientBias.z;
					}
					terrainTexture.SetPixel(i, j, new Color(val.x, val.y, 0));
				}
			}

			//Blur Inertial Motion
			//Color[,] colArray = new Color[terrainTexture.width, terrainTexture.height];
			//for (int i = 0; i < terrainTexture.width; i++)
			//{
			//	for (int j = 0; j < terrainTexture.height; j++)
			//	{
			//		colArray[i, j] = new Color(.5f, .5f, 0f);
			//	}
			//}
			//for (int i = 0; i < terrainTexture.width; i++)
			//{
			//	for (int j = 0; j < terrainTexture.height; j++)
			//	{
			//		Color origCol = terrainTexture.GetPixel(i, j);
			//		for (int k = 0; k <= 5; k++)
			//		{
			//			int tarX = i + (int)(k * Mathf.Sign(origCol.r - .5f));
			//			int tarY = j + (int)(k * Mathf.Sign(origCol.g - .5f));
			//			if (tarX > 0 && tarX < terrainTexture.width && tarY > 0 && tarY < terrainTexture.height)
			//			{
			//				colArray[tarX, tarY] += .25f * ((origCol - new Color(.5f, .5f, 0f)) / k);
			//				colArray[tarX, tarY] = new Color(Mathf.Clamp01(colArray[tarX, tarY].r), Mathf.Clamp01(colArray[tarX, tarY].g), Mathf.Clamp01(colArray[tarX, tarY].b));
			//			}
			//		}
			//	}
			//}

			//for (int i = 0; i < terrainTexture.width; i++)
			//{
			//	for (int j = 0; j < terrainTexture.height; j++)
			//	{
			//		terrainTexture.SetPixel(i, j, colArray[i, j]);
			//	}
			//}

			terrainTexture.Apply();
			GetComponent<MeshRenderer>().material.SetTexture("_FlowTex", terrainTexture);
		}

		//noiseTex = new Texture2D(32, 32);
		//for (int i = 0; i < noiseTex.width; i++)
		//{
		//	for (int j = 0; j < noiseTex.height; j++)
		//	{
		//		float val = myNoise.GetNoise(i, j);
		//		noiseTex.SetPixel(i, j, new Color(val, val, val, 1));
		//	}
		//}
		//noiseTex.Apply();

		//GetComponent<MeshRenderer>().material.SetTexture("_MainTex", noiseTex);
		GetComponent<MeshRenderer>().enabled = true;
		//terrain.terrainData
	}

	// Update is called once per frame
	void Update()
	{
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
