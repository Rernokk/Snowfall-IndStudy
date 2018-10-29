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
	private bool applyBlur = false;

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
		if (texA2D == null || texB2D == null || texC2D == null)
		{
			return;
		}
		float lowestA = 2;
		float lowestB = 2;
		float highestA = 0;
		float highestB = 0;

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
					
				}
				generatedTexture.SetPixel(i, j, generatedDirectionColor);
				//generatedTexture.SetPixel(i, j, new Color(i / (float)generatedTexture.width, j / (float)generatedTexture.height, 0));
			}
		}

		//for (int i = 0; i < generatedTexture.width; i++)
		//{
		//	for (int j = 0; j < generatedTexture.height; j++)
		//	{
		//		if (texB2D.GetPixel(i, j).r <= lowestB)
		//		{
		//			continue;
		//		}
		//		else
		//		{ 
		//			//Sample Heights along X axis
		//			float sampleXA, sampleXB;
		//			sampleXA = texB2D.GetPixel(i - 1, j).r * 100;
		//			sampleXB = texB2D.GetPixel(i + 1, j).r * 100;

		//			//Sample Heights along Y axis
		//			float sampleYA, sampleYB;
		//			sampleYA = texB2D.GetPixel(i - 1, j).r * 100;
		//			sampleYB = texB2D.GetPixel(i + 1, j).r * 100;

		//			//Calculate Direction for X
		//			float xDirection = sampleXB - sampleXA;

		//			//Calculate Direction for Y
		//			float yDirection = sampleYB - sampleYA;

		//			//Amplifying Difference for X
		//			xDirection *= 1;

		//			//Amplifying Difference for Y
		//			yDirection *= 1;

		//			//Fix Range from (-1, 1) to (0, 1) for X
		//			xDirection = xDirection * .5f + .5f;

		//			//Fix Range from (-1, 1) to (0, 1) for Y
		//			yDirection = yDirection * .5f + .5f;

		//			//Assign Color Channels
		//			generatedTexture.SetPixel(i, j, new Color(xDirection, yDirection, 0));
		//			if (generatedTexture.GetPixel(i,j) == new Color(.5f,.5f,0f)){
		//				generatedTexture.SetPixel(i, j, Color.magenta);
		//			}
		//		}
		//	}
		//}

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
