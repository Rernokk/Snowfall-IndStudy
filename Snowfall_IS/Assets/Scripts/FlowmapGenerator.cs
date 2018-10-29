using System.IO;
using UnityEngine;

public class FlowmapGenerator : MonoBehaviour
{
	[SerializeField]
	public Texture2D maskMap, flowMap;

	[SerializeField]
	private LayerMask mask;

	[SerializeField]
	private float cutFactor = .05f;

	[SerializeField]
	private bool GenerateTexture = true;

	[SerializeField]
	private int amnt;
	private Material mat;
	private Vector2[,] vecArray;
	private Vector2[,] backupArray;
	private FastNoise myNoise;
	private Texture2D perlinNoise;
	private int perlinOffX = 0, perlinOffY = 0;

	public Texture2D texA2D;
	public Texture2D texB2D;
	public Texture2D texC2D;

	public Texture2D generatedTexture;

	// Use this for initialization
	private void Start()
	{
		mat = GetComponent<MeshRenderer>().material;
		perlinNoise = new Texture2D(1024, 1024);
		myNoise = new FastNoise();
		myNoise.SetNoiseType(FastNoise.NoiseType.Perlin);
		UpdatePerlinNoise();
		mat.SetTexture("_MainTex", perlinNoise);

		CalculateFlowMap();
	}

	private void Update()
	{
		//UpdatePerlinNoise();
		perlinOffX++;
		perlinOffY++;
	}

	private void UpdatePerlinNoise()
	{
		//return;
		for (int i = 0; i < perlinNoise.width; i++)
		{
			for (int j = 0; j < perlinNoise.height; j++)
			{
				float col = (1 + myNoise.GetPerlin(i * 3 + perlinOffX, j * 3 + perlinOffY)) * .5f;
				perlinNoise.SetPixel(i, j, new Color(col, col, col));
			}
		}
		perlinNoise.Apply();
	}

	public void CalculateFlowMap()
	{
		if (!GenerateTexture)
			return;

		if (texA2D == null || texB2D == null || texC2D == null)
		{
			return;
		}
		float lowestA = 2;
		float highestA = 0;

		texA2D.anisoLevel = 9;

		for (int i = 0; i < texA2D.width; i++)
		{
			for (int j = 0; j < texA2D.height; j++)
			{
				float val = texA2D.GetPixel(i, j).r;
				if (val != 0)
				{
					if (val < lowestA)
					{
						lowestA = texA2D.GetPixel(i, j).r;
					}
					else if (val > highestA)
					{
						highestA = texA2D.GetPixel(i, j).r;
					}
				}
			}
		}

		generatedTexture = new Texture2D(texA2D.width, texA2D.height);
		generatedTexture.anisoLevel = 0;
		//generatedTexture.filterMode = FilterMode.Trilinear;
		
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
				Vector3[,] colGroup = new Vector3[3, 3];
				for (int k = -1; k <= 1; k++)
				{
					for (int l = -1; l <= 1; l++)
					{
						if (i + k >= 0 && i + k < texA2D.width && j + l >= 0 && j + l < texA2D.height)
						{
							Color grabVal = texA2D.GetPixelBilinear((i + k)/ (float)texA2D.width, (j + l)/ (float)texA2D.height);
							colGroup[k + 1, l + 1] = new Vector3(grabVal.r, grabVal.g, 0f);
						}
						else
						{
							Color grabVal = texA2D.GetPixelBilinear((i) / (float) texA2D.width, (j) / (float) texA2D.height);
							colGroup[k + 1, l + 1] = new Vector3(grabVal.r, grabVal.g, 0f);
						}
					}
				}
				Color generatedDirectionColor = new Color(.5f, .5f, 0f);
				if (colGroup[1, 1].x != 0)
				{
					Vector3 tempA = colGroup[1, 1] - colGroup[2, 1];
					Vector3 tempB = colGroup[1, 1] - colGroup[0, 1];
					if (tempA.x <= 0 || tempA.y <= 0)
					{
						generatedDirectionColor.r = 0;
					} else {
						generatedDirectionColor.r = 1;
					}

					if (tempB.x <= 0 || tempB.y <= 0)
					{
						generatedDirectionColor.g = 0;
					} else {
						generatedDirectionColor.g = 1;
					}
				}
				generatedTexture.SetPixel(i, j, generatedDirectionColor);
				//generatedTexture.SetPixel(i, j, new Color(i / (float)generatedTexture.width, j / (float)generatedTexture.height, 0));
			}
		}


		generatedTexture.Apply();
		generatedTexture.filterMode = FilterMode.Point;
		mat.SetTexture("_FlowTex", generatedTexture);

		byte[] output = generatedTexture.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../Generated.png", output);
		output = texA2D.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../TexA.png", output);
		output = texB2D.EncodeToPNG();
		File.WriteAllBytes(Application.dataPath + "/../TexB.png", output);
	}
}
