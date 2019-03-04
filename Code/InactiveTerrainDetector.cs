using System;
using UnityEngine;
using Utility;

internal class InactiveTerrainDetector : MonoBehaviour
{
	private const float Y_POS_BOUNDARY = -100f;

	private bool _reportedFault;

	private Transform _t;

	private TerrainCollider _tc;

	public InactiveTerrainDetector()
		: this()
	{
	}

	private void Start()
	{
		_t = this.GetComponent<Transform>();
		_tc = Object.FindObjectOfType<TerrainCollider>();
	}

	private void Update()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!(_t != null) || _reportedFault)
		{
			return;
		}
		Vector3 position = _t.get_position();
		if (position.y < -100f)
		{
			if (_tc != null)
			{
				bool activeSelf = _tc.get_gameObject().get_activeSelf();
				bool enabled = _tc.get_enabled();
				bool isTrigger = _tc.get_isTrigger();
				ReportError(string.Format("User fell through ground. [Found Terrain collider {3}: Active:=={0}, Enabled=={1}, isTrigger=={2}] ", activeSelf, enabled, isTrigger, _tc.get_name()));
			}
			else
			{
				ReportError("User fell through ground. [Could not find active collider] ");
			}
			_reportedFault = true;
		}
	}

	private void ReportError(string e)
	{
		Console.Log(e);
		Console.LogException(new Exception(e));
	}
}
