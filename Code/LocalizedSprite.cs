using System;

[Serializable]
internal sealed class LocalizedSprite
{
	public string locale;

	public string spriteNameToUse;

	public LocalizedSprite(string locale_, string spriteName_)
	{
		locale = locale_;
		spriteNameToUse = spriteName_;
	}
}
