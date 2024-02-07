using UnityEngine;

public class CamBlurController : MonoBehaviour
{
	public bool updateTexture;

	public Texture2D outputTexture;

	public GameObject quadObj;

	public Material quadMat;

	private float avgR;

	private float avgG;

	private float avgB;

	private float avgA;

	private float blurPixelCount;

	private void Awake()
	{
		outputTexture = new Texture2D(Screen.width, Screen.height);
		quadMat.mainTexture = outputTexture;
	}

	public void blurScreen()
	{
		updateTexture = true;
	}

	public void disableBlur()
	{
		quadObj.SetActive(false);
	}

	private void OnPostRender()
	{
		if (updateTexture)
		{
			outputTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
			outputTexture.Apply();
			updateTexture = false;
			quadObj.SetActive(true);
		}
	}

	private Texture2D FastBlur(Texture2D image, int radius, int iterations)
	{
		Texture2D texture2D = image;
		for (int i = 0; i < iterations; i++)
		{
			texture2D = BlurImage(texture2D, radius, true);
			texture2D = BlurImage(texture2D, radius, false);
		}
		return texture2D;
	}

	private Texture2D BlurImage(Texture2D image, int blurSize, bool horizontal)
	{
		Texture2D texture2D = new Texture2D(image.width, image.height);
		int width = image.width;
		int height = image.height;
		if (horizontal)
		{
			for (int i = 0; i < height; i++)
			{
				for (int j = 0; j < width; j++)
				{
					ResetPixel();
					int k;
					for (k = j; k < j + blurSize && k < width; k++)
					{
						AddPixel(image.GetPixel(k, i));
					}
					k = j;
					while (k > j - blurSize && k > 0)
					{
						AddPixel(image.GetPixel(k, i));
						k--;
					}
					CalcPixel();
					for (k = j; k < j + blurSize && k < width; k++)
					{
						texture2D.SetPixel(k, i, new Color(avgR, avgG, avgB, 1f));
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < width; j++)
			{
				for (int i = 0; i < height; i++)
				{
					ResetPixel();
					int l;
					for (l = i; l < i + blurSize && l < height; l++)
					{
						AddPixel(image.GetPixel(j, l));
					}
					l = i;
					while (l > i - blurSize && l > 0)
					{
						AddPixel(image.GetPixel(j, l));
						l--;
					}
					CalcPixel();
					for (l = i; l < i + blurSize && l < height; l++)
					{
						texture2D.SetPixel(j, l, new Color(avgR, avgG, avgB, 1f));
					}
				}
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	private void AddPixel(Color pixel)
	{
		avgR += pixel.r;
		avgG += pixel.g;
		avgB += pixel.b;
		blurPixelCount += 1f;
	}

	private void ResetPixel()
	{
		avgR = 0f;
		avgG = 0f;
		avgB = 0f;
		blurPixelCount = 0f;
	}

	private void CalcPixel()
	{
		avgR /= blurPixelCount;
		avgG /= blurPixelCount;
		avgB /= blurPixelCount;
	}
}
