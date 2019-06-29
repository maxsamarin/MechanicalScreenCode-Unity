using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class MechanicalScreen : MonoBehaviour
{
	public float AspectRatio_X = 16;
	public float AspectRatio_Y = 9;

	public int NumPixels_X = 70;
	public int NumPixels_Y { get { return (int) (NumPixels_X * (AspectRatio_Y / AspectRatio_X)); } }
	public float ScreenWidth = 10;
	public float ScreenHeight { get { return ScreenWidth * (AspectRatio_Y / AspectRatio_X); } }

	public float Angle_Light = -20;
	public float Angle_Dark = 30;

	public RenderTexture RenderTexture;

	public Transform PixelPrefab;

	private Transform[, ] _pixels = new Transform[2000, 2000];

	private Texture2D _texture;

	private void Awake()
	{
		_texture = new Texture2D(RenderTexture.width, RenderTexture.height, TextureFormat.ARGB32, false);

		//Create a RenderTexture file in your assets through the right-click menu

		//To play a video, make a VideoPlayer render onto the RenderTexture file 
		//To display the view of another camera, make that Camera onto the RenderTexture file 

		for (int i = 0; i < NumPixels_X; i++)
		{
			for (int j = 0; j < NumPixels_Y; j++)
			{
				Transform pixel = Instantiate(PixelPrefab, transform);
				_pixels[i, j] = pixel;
				float pos_x = -FloatDiv(i, NumPixels_X) * ScreenWidth + ScreenWidth / 2f;
				float pos_y = FloatDiv(j, NumPixels_Y) * ScreenHeight - ScreenHeight / 2f;
				pixel.localPosition = new Vector3(pos_x, pos_y, 0);
			}
		}
	}

	private void Update()
	{
		Rect rectReadPicture = new Rect(0, 0, RenderTexture.width, RenderTexture.height);
		RenderTexture.active = RenderTexture;

		//Read pixels from rendertexture
		_texture.ReadPixels(rectReadPicture, 0, 0);
		_texture.Apply();

		for (int i = 0; i < NumPixels_X; i++)
		{
			for (int j = 0; j < NumPixels_Y; j++)
			{
				float t = FromRenderTexture(i, j);
				_pixels[i, j].localEulerAngles = new Vector3(Mathf.Lerp(Angle_Dark, Angle_Light, t), 0, 0);
			}
		}
	}

	private float FloatDiv(int a, int b)
	{
		return ((float) a) / (float) b;
	}

	private float Wave(int i, int j)
	{
		return Mathf.InverseLerp(-1, 1, Mathf.Sin(10 * FloatDiv(i, NumPixels_X) + 20 * Time.time));
	}

	private float FromRenderTexture(int i, int j)
	{

		Color color = _texture.GetPixel((int) (FloatDiv(i, NumPixels_X) * _texture.width), (int) (FloatDiv(j, NumPixels_Y) * _texture.height));
		return color.grayscale;
	}

}