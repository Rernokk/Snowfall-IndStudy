using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FlowmapGenerator : MonoBehaviour
{
	[SerializeField]
	public Texture2D maskMap, flowMap;

	[SerializeField]
	LayerMask mask;

	[SerializeField]
	float cutFactor = .05f;

	[SerializeField]
	bool applyBlur = false;

	[SerializeField]
	int amnt;

	Material mat;
	Vector2[,] vecArray;
	Vector2[,] backupArray;

	FastNoise myNoise;
	Texture2D perlinNoise;
	int perlinOffX = 0, perlinOffY = 0;

	public Texture2D texA2D;
	public Texture2D texB2D;

	public Texture2D generatedTexture;

	// Use this for initialization
	void Start()
	{
		maskMap = new Texture2D(512, 512);
		flowMap = new Texture2D(512, 512);
		for (int i = 0; i < maskMap.width; i++)
		{
			for (int j = 0; j < maskMap.height; j++)
			{
				RaycastHit inf;
				Physics.Raycast(new Ray(new Vector3(i * 100.0f / maskMap.width, 50, j * 100.0f / maskMap.height), Vector3.down), out inf, 100f);
				if (inf.transform == null || inf.transform.tag == "Terrain")
				{
					maskMap.SetPixel(i, j, Color.black);
				}
				else
				{
					maskMap.SetPixel(i, j, Color.white);
				}
			}
		}

		//CreateFlowMap();

		#region Adaptation
		vecArray = new Vector2[flowMap.width, flowMap.height];
		backupArray = new Vector2[flowMap.width, flowMap.height];

		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				vecArray[i, j] = Vector2.zero;
			}
		}

		CreateFlowMapVTwo();

		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				backupArray[i, j] = vecArray[i, j];
			}
		}

		if (applyBlur)
		{
			#region Wrongo
			//Wrongo
			//for (int k = 0; k < flowMap.width; k++)
			//{
			//	for (int j = 0; j < flowMap.height; j++)
			//	{
			//		Vector2 origCol = backupArray[k, j];
			//		if (maskMap.GetPixel(k,j) == Color.white)
			//		{
			//			float falloff = .9f;
			//			for (int i = 0; i < amnt; i++)
			//			{
			//				int indX = k - i * (int)(backupArray[k, j].x != 0 ? Mathf.Sign(backupArray[k, j].x) : 0);
			//				int indY = j - i * (int)(backupArray[k, j].y != 0 ? Mathf.Sign(backupArray[k, j].y) : 0);
			//				if (indY >= 0 && indX >= 0 && indX < flowMap.width && indY < flowMap.height)
			//				{
			//					//vecArray[indX, indY] += backupArray[k, j] * (amnt - i) / amnt;
			//					vecArray[indX, indY] += backupArray[k, j] * falloff;
			//					falloff *= .9f;
			//				}
			//			}
			//		}
			//	}
			//}
			#endregion

			//Blur Example, Graphics Shaders Theory & Practice
			for (int k = 0; k < 8; k++)
			{
				for (int i = 0; i < flowMap.width; i++)
				{
					for (int j = 0; j < flowMap.height; j++)
					{
						if (maskMap.GetPixel(i, j) == Color.white)
						{
							Vector2 tar = Vector2.zero;
							for (int x = -1; x <= 1; x++)
							{
								for (int y = -1; y <= 1; y++)
								{
									if (x + i >= 0 && x + i < flowMap.width && j + y >= 0 && j + y < flowMap.height)
									{
										if (x == 0 && y == 0)
										{
											tar += backupArray[i + x, j + y] * 4;
										}
										else if (x == 0 || y == 0)
										{
											tar += backupArray[i + x, j + y] * 2;
										}
										else
										{
											tar += backupArray[i + x, j + y];
										}
									}
								}
							}
							tar /= 16;
							vecArray[i, j] = tar;
						}
					}
				}
				backupArray = vecArray;
			}
		}

		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				Vector2 clamped = vecArray[i, j];
				clamped += Vector2.one;
				clamped *= .5f;
				Color col = new Color(Mathf.Clamp01(clamped.x), Mathf.Clamp01(clamped.y), 0);
				flowMap.SetPixel(i, j, col);
			}
		}
		flowMap.Apply();

		flowMap.filterMode = FilterMode.Point;
		//flowMap.filterMode = FilterMode.Trilinear;
		flowMap.wrapMode = TextureWrapMode.Repeat;
		#endregion

		mat = GetComponent<MeshRenderer>().material;
		//mat.SetTexture("_FlowTex", flowMap);

		perlinNoise = new Texture2D(1024, 1024);
		myNoise = new FastNoise();
		myNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
		//myNoise.SetFrequency(1f);
		UpdatePerlinNoise();
		//		mat.SetTexture("_MainTex", perlinNoise);
		//byte[] output = flowMap.EncodeToPNG();
		//File.WriteAllBytes(Application.dataPath + "/../Fileout_ModBlur.png", output);
	}


	void Update()
	{
		//UpdatePerlinNoise();
		perlinOffX++;
		perlinOffY++;
	}

	void UpdatePerlinNoise()
	{
		//return;
		for (int i = 0; i < perlinNoise.width; i++)
		{
			for (int j = 0; j < perlinNoise.height; j++)
			{
				float col = myNoise.GetPerlin(i * 3 + perlinOffX, j * 3 + perlinOffY);
				perlinNoise.SetPixel(i, j, new Color(col, col, col));
			}
		}
		perlinNoise.Apply();
	}

	void CreateFlowMap()
	{
		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				if (maskMap.GetPixel(i, j) == Color.white)
				{
					RaycastHit inf;
					Physics.Raycast(new Ray(new Vector3(i * 100.0f / flowMap.width, 40, j * 100.0f / flowMap.height), Vector3.down), out inf, 100f, mask);
					Vector3 normDir = inf.normal;
					normDir.y = 0;
					normDir.Normalize();
					flowMap.SetPixel(i, j, new Color(Mathf.Clamp01(.5f + -normDir.x * cutFactor), Mathf.Clamp01(.5f + -normDir.z * cutFactor), 0));
				}
				else
				{
					flowMap.SetPixel(i, j, new Color(.5f, .5f, 0));
				}
			}
		}
		flowMap.Apply();
	}

	void CreateFlowMapVTwo()
	{
		for (int i = 0; i < flowMap.width; i++)
		{
			for (int j = 0; j < flowMap.height; j++)
			{
				if (maskMap.GetPixel(i, j) == Color.white || true)
				{
					RaycastHit inf;
					Physics.Raycast(new Ray(new Vector3(i * 100.0f / flowMap.width, 40, j * 100.0f / flowMap.height), Vector3.down), out inf, 100f, mask);
					Vector3 normDir = inf.normal;
					normDir.y = 0;
					normDir.Normalize();
					vecArray[i, j] = new Vector2(-normDir.x * cutFactor, -normDir.z * cutFactor);
				}
			}
		}
	}

	public void CalculateFlowMap()
	{
		if (texA2D == null || texB2D == null)
		{
			return;
		}
		float lowestA = 2;
		float lowestB = 2;
		float highestA = 0;
		float highestB = 0;

		for (int i = 0; i < texA2D.width; i++)
		{
			for (int j = 0; j < texA2D.height; j++)
			{
				float tempA = texA2D.GetPixel(i, j).r;
				if (tempA > 0)
				{
					if (lowestA > tempA)
					{
						lowestA = tempA;
					}
					else if (highestA < tempA)
					{
						highestA = tempA;
					}

					float tempB = texB2D.GetPixel(i, j).r;
					if (lowestB > tempB)
					{
						lowestB = tempB;
					}
					else if (highestB < tempB)
					{
						highestB = tempB;
					}
				}
			}
		}

		generatedTexture = new Texture2D(texA2D.width, texA2D.height);
		for (int i = 0; i < generatedTexture.width; i++)
		{
			for (int j = 0; j < generatedTexture.height; j++)
			{
				generatedTexture.SetPixel(i, j, new Color(.5f, .5f, 0f));
			}
		}

		for (int i = 0; i < generatedTexture.width; i++)
		{
			for (int j = 0; j < generatedTexture.height; j++)
			{
				if (texB2D.GetPixel(i, j).r <= lowestB)
				{
					generatedTexture.SetPixel(i, j, Color.blue);
					continue;
				}
				else
				{
					int offset = 1;
					float mag = 100f;
					Color col = generatedTexture.GetPixel(i, j);
					if (i + offset < generatedTexture.width || i - offset >= 0)
					{
						float dif = (texB2D.GetPixel(i - offset, j) - texB2D.GetPixel(i + offset, j)).r * mag;
						col.r += dif;
					}

					if (j + offset < generatedTexture.height || j - offset >= 0)
					{
						float dif = (texB2D.GetPixel(i, j - offset) - texB2D.GetPixel(i, j + offset)).r * mag;
						col.g += dif;
					}
					generatedTexture.SetPixel(i, j, col);
				}
			}
		}

		generatedTexture.Apply();
		generatedTexture.filterMode = FilterMode.Point;
		mat.SetTexture("_FlowTex", generatedTexture);

		byte[] output = generatedTexture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../Fileout_Generated.png", output);
	}
}
