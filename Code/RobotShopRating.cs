using System.Collections.Generic;
using UnityEngine;

internal class RobotShopRating : MonoBehaviour
{
	public List<UISprite> Stars;

	public Color ratedColor = new Color(84f / 85f, 131f / 255f, 4f / 51f);

	public Color unRatedColor = Color.get_black();

	public RobotShopRating()
		: this()
	{
	}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
	//IL_0015: Unknown result type (might be due to invalid IL or missing references)
	//IL_001b: Unknown result type (might be due to invalid IL or missing references)
	//IL_0020: Unknown result type (might be due to invalid IL or missing references)


	public void Set(double score, bool setColours = true)
	{
		int rating = Mathf.RoundToInt((float)score);
		SetScore(Stars, rating, setColours);
	}

	private void SetScore(List<UISprite> stars, int rating, bool setColours)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		int i;
		for (i = 0; i < rating; i++)
		{
			if (setColours)
			{
				stars[i].set_color(ratedColor);
			}
			else
			{
				stars[i].get_gameObject().SetActive(true);
			}
		}
		for (; i < stars.Count; i++)
		{
			if (setColours)
			{
				stars[i].set_color(unRatedColor);
			}
			else
			{
				stars[i].get_gameObject().SetActive(false);
			}
		}
	}
}
