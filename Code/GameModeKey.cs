public struct GameModeKey
{
	public readonly GameModeType type;

	public readonly bool IsRanked;

	public readonly bool IsBrawl;

	public readonly bool IsCustomGame;

	public GameModeKey(GameModeType type, bool isRanked = false, bool isBrawl = false, bool isCustomGame = false)
	{
		this.type = type;
		IsRanked = isRanked;
		IsBrawl = isBrawl;
		IsCustomGame = isCustomGame;
	}

	public override bool Equals(object obj)
	{
		if (obj is GameModeKey)
		{
			return this == (GameModeKey)obj;
		}
		return false;
	}

	public override int GetHashCode()
	{
		int num = (int)type;
		num = ((num << 1) | (IsRanked ? 1 : 0));
		num = ((num << 1) | (IsBrawl ? 1 : 0));
		return (num << 1) | (IsCustomGame ? 1 : 0);
	}

	public static bool operator ==(GameModeKey a, GameModeKey b)
	{
		return a.type == b.type && a.IsRanked == b.IsRanked && a.IsBrawl == b.IsBrawl && a.IsCustomGame == b.IsCustomGame;
	}

	public static bool operator !=(GameModeKey a, GameModeKey b)
	{
		return !(a == b);
	}
}
