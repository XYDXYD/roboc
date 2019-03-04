using UnityEngine;
using Utility;

public class PartyColours : ScriptableObject
{
	[SerializeField]
	private Color[] _colours;

	public PartyColours()
		: this()
	{
	}

	public Color GetColour(int partyIndex)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (partyIndex > _colours.Length - 1)
		{
			Console.LogError("Requested party colour beyond array length");
			partyIndex %= _colours.Length;
		}
		return _colours[partyIndex];
	}
}
