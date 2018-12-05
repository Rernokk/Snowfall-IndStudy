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
		//mat.SetTexture("_MainTex", perlinNoise);

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
		generatedTexture.filterMode = FilterMode.Trilinear;
		
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
				//Create Color Group
				Vector3[,] colGroup = new Vector3[3, 3];

				//Supersample nearby cells
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
				
				//Give Base Color
				Color generatedDirectionColor = new Color(.5f, .5f, 0f);

				//Check if color is not the black background
				if (colGroup[1, 1].x != 0 && (i != 0 || j != 0))
				{
					//Vector results to the origin
					Vector3 tempA = colGroup[0, 1] - colGroup[2, 1];
					Vector3 tempB = colGroup[1, 0] - colGroup[1, 2];
					generatedDirectionColor += new Color(tempA.x, tempB.y, 0) * 100f;
				}
				generatedTexture.SetPixel(i, j, generatedDirectionColor);
				//generatedTexture.SetPixel(i, j, new Color(i / (float)generatedTexture.width, j / (float)generatedTexture.height, 0));
			}
		}

		Texture2D tempTex = new Texture2D(generatedTexture.width, generatedTexture.height);
		int[,] gFilter = new int[,] { { 41, 26, 7 }, { 26, 16, 4 }, { 7, 4, 1 } };
		for (int i = 1; i < generatedTexture.width - 1; i ++){
			for (int j = 1; j < generatedTexture.height - 1; j++){
				Vector3 colArray = Vector3.zero;
				for (int a = -2; a <= 2; a ++){
					for (int b = -2; b <= 2; b++){
						Color col = generatedTexture.GetPixel(a + i, b + j);
						colArray += new Vector3(col.r, col.g, col.b) * gFilter[Mathf.Abs(a), Mathf.Abs(b)];
					}
				}
				colArray /= 273f;
				tempTex.SetPixel(i, j, new Color(colArray.x, colArray.y, colArray.z));
			}
		}
		tempTex.Apply();
		generatedTexture = tempTex;

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
	
	////Flood-Fill Approach to generation:
	//public void FloodFillTexture(){
	//	//Pseudo
	//	//Find highest point on height map
	//	//
	//}
}
