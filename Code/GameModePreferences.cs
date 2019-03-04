using System.Collections.Generic;

internal struct GameModePreferences
{
	private int _prefs;

	public GameModePreferences(int mask = 65535)
	{
		_prefs = mask;
	}

	public bool ContainsAny(IEnumerable<GameModeType> e)
	{
		IEnumerator<GameModeType> enumerator = e.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (Contains(enumerator.Current))
			{
				return true;
			}
		}
		return false;
	}

	public bool Contains(GameModeType mode)
	{
		return (_prefs & (1 << (int)mode)) != 0;
	}

	public void Add(GameModeType mode)
	{
		_prefs |= 1 << (int)mode;
	}

	public void Remove(GameModeType mode)
	{
		_prefs &= ~(1 << (int)mode);
	}

	public override bool Equals(object obj)
	{
		GameModePreferences gameModePreferences = (GameModePreferences)obj;
		return gameModePreferences._prefs == _prefs;
	}

	public static bool operator ==(GameModePreferences a, GameModePreferences b)
	{
		return a._prefs == b._prefs;
	}

	public static bool operator !=(GameModePreferences a, GameModePreferences b)
	{
		return a._prefs != b._prefs;
	}

	public override int GetHashCode()
	{
		return _prefs;
	}

	public int Serialize()
	{
		return _prefs;
	}
}
