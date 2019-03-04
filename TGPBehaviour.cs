using rail;
using UnityEngine;

public class TGPBehaviour : MonoBehaviour
{
	public TGPBehaviour()
		: this()
	{
	}

	private void Start()
	{
		Object.DontDestroyOnLoad(this);
	}

	private void OnApplicationQuit()
	{
		rail_api.CSharpRailUnRegisterAllEvent();
		rail_api.RailFinalize();
	}

	private void Update()
	{
		rail_api.RailFireEvents();
	}
}
