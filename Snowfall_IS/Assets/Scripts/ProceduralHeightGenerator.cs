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
	void Start()
	{
		myNoise = new FastNoise((int)Time.time);
		myNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
		if (terrain != null)
		{
			terrainTexture = new Texture2D((int)terrain.terrainData.bounds.size.x * 4, (int)terrain.terrainData.bounds.size.z * 4);
			terrainTexture.filterMode = FilterMode.Bilinear;
			
			float max = terrain.terrainData.bounds.size.y;
			for (int i = 0; i < terrainTexture.width; i++)
			{
				for (int j = 0; j < terrainTexture.height; j++)
				{
					Vector3 val = new Vector3(.5f, .5f);
					if (includeTerrain)
					{
						Vector3 res = Vector3.zero;
						for (int a = -5; a <= 5; a++)
						{
							for (int b = -5; b <= 5; b++)
							{
								res += terrain.terrainData.GetInterpolatedNormal((float)(i + a) / terrainTexture.width, (float)(j + b) / terrainTexture.height) * 1f;
							}
						}
						res *= .01f;
						val.x -= res.x;
						val.y -= res.z;
					}
					if (includeGradientBias)
					{
						Vector3 gradientBias = transform.up;
						val.x -= gradientBias.x;
						val.y -= gradientBias.z;
					}

					terrainTexture.SetPixel(i, j, new Color(val.x, val.y, 0));
				}
			}
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
