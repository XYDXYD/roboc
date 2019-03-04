using System.Collections.Generic;

internal class ProfanityNode
{
	public char ch;

	public bool terminator;

	public List<ProfanityNode> children;

	public ProfanityNode(char initChar)
	{
		ch = initChar;
		children = new List<ProfanityNode>();
	}

	public override int GetHashCode()
	{
		return ch;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		ProfanityNode profanityNode = obj as ProfanityNode;
		if (profanityNode == null)
		{
			return false;
		}
		if (profanityNode.ch != ch)
		{
			return false;
		}
		return true;
	}

	public bool Equals(ProfanityNode pn)
	{
		if (pn == null)
		{
			return false;
		}
		return ch.Equals(pn.ch);
	}
}
