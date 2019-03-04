using UnityEngine;

namespace Mothership.Garage.Thumbnail
{
	internal class TextureScaler
	{
		public static void ResizeTexture(Texture2D dest, Texture2D source)
		{
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0141: Unknown result type (might be due to invalid IL or missing references)
			//IL_0160: Unknown result type (might be due to invalid IL or missing references)
			//IL_0178: Unknown result type (might be due to invalid IL or missing references)
			//IL_017d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
			Color[] pixels = source.GetPixels(0);
			Vector2 val = default(Vector2);
			val._002Ector((float)source.get_width(), (float)source.get_height());
			float num = dest.get_width();
			float num2 = dest.get_height();
			int num3 = (int)num * (int)num2;
			Color[] array = (Color[])new Color[num3];
			Vector2 val2 = default(Vector2);
			val2._002Ector(val.x / num, val.y / num2);
			Vector2 val3 = default(Vector2);
			for (int i = 0; i < num3; i++)
			{
				float num4 = (float)i % num;
				float num5 = Mathf.Floor((float)i / num);
				val3.x = num4 / num * val.x;
				val3.y = num5 / num2 * val.y;
				int num6 = (int)Mathf.Max(Mathf.Floor(val3.x - val2.x * 0.5f), 0f);
				int num7 = (int)Mathf.Min(Mathf.Ceil(val3.x + val2.x * 0.5f), val.x);
				int num8 = (int)Mathf.Max(Mathf.Floor(val3.y - val2.y * 0.5f), 0f);
				int num9 = (int)Mathf.Min(Mathf.Ceil(val3.y + val2.y * 0.5f), val.y);
				Color val4 = default(Color);
				float num10 = 0f;
				for (int j = num8; j < num9; j++)
				{
					for (int k = num6; k < num7; k++)
					{
						val4 += pixels[(int)((float)j * val.x + (float)k)];
						num10 += 1f;
					}
				}
				array[i] = val4 / num10;
			}
			dest.SetPixels(array);
			dest.Apply();
		}
	}
}
