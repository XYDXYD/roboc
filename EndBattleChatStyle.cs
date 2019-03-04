using System;
using UnityEngine;

[Serializable]
internal class EndBattleChatStyle
{
	public GameObject[] enabledElements;

	public GameObject[] enabledElementsOnFocus;

	public int unfocussedDepth = 30;

	public int focussedDepth = 201;
}
