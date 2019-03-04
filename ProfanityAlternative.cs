internal class ProfanityAlternative
{
	public char key;

	public char alt;

	public ProfanityAlternative(char initKey, char initAlt)
	{
		key = initKey;
		alt = initAlt;
	}

	public override int GetHashCode()
	{
		return key;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		ProfanityAlternative profanityAlternative = obj as ProfanityAlternative;
		if (profanityAlternative == null)
		{
			return false;
		}
		if (profanityAlternative.key != key)
		{
			return false;
		}
		return true;
	}

	public bool Equals(ProfanityAlternative pa)
	{
		if (pa == null)
		{
			return false;
		}
		return key.Equals(pa.key);
	}
}
