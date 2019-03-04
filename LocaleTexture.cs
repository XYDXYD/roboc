using System;
using UnityEngine;

[Serializable]
internal sealed class LocaleTexture
{
	public string locale;

	public Texture texture;

	public LocaleTexture(string pLocale, Texture pTexture)
	{
		locale = pLocale;
		texture = pTexture;
	}
}
