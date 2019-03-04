using UnityEngine;

public class DNABehaviour : MonoBehaviour
{
	public DNABehaviour()
		: this()
	{
	}

	private void OnApplicationQuit()
	{
		Debug.Log((object)"DNA QUIT");
		DeltaDNAHelper.DeltaDNAEndSession();
	}
}
