using Mothership;
using UnityEngine;

internal sealed class MachineBoard
{
	private GameObject _layout;

	private static MachineBoard _instance;

	public static MachineBoard Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new MachineBoard();
			}
			return _instance;
		}
	}

	public Transform board
	{
		get
		{
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Expected O, but got Unknown
			if (_layout == null)
			{
				_layout = GameObject.Find("CubesLayout");
				if (_layout != null)
				{
					Object.Destroy(_layout);
				}
				_layout = new GameObject("CubesLayout");
				CubeLayoutAdjuster cubeLayoutAdjuster = _layout.AddComponent<CubeLayoutAdjuster>();
				UnityContext val = Object.FindObjectOfType(typeof(UnityContext)) as UnityContext;
				if (val != null)
				{
					_layout.get_transform().SetParent(val.get_transform(), false);
				}
				cubeLayoutAdjuster.Recalculate();
			}
			return _layout.get_transform();
		}
	}
}
