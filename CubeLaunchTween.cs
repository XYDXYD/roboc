using Mothership;
using UnityEngine;

internal sealed class CubeLaunchTween : MonoBehaviour
{
	private const float C_TWEEN_TIME = 0.3f;

	private const float C_TWEEN_SCALE = 0.3f;

	private Quaternion _finalRotation;

	private Vector3 _finalPosition;

	private Vector3 _finalScale;

	private float _time;

	private float _parentScale;

	private MachineEditorBatcher _batcher;

	public Vector3 WorldStartPosition
	{
		private get;
		set;
	}

	public Vector3 WorldEndPosition
	{
		private get;
		set;
	}

	public CubeLaunchTween()
		: this()
	{
	}

	internal void InitialUpdate(MachineEditorBatcher batcher)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		_time = 0f;
		_batcher = batcher;
		_batcher.onPreBatching += Disable;
		Vector3 localScale = this.get_transform().get_parent().get_localScale();
		_parentScale = localScale.x;
		_finalScale = this.get_transform().get_localScale();
		_finalPosition = this.get_transform().get_localPosition();
		_finalRotation = this.get_transform().get_localRotation();
		Vector3 val = WorldStartPosition - WorldEndPosition;
		Vector3 val2 = this.get_transform().get_parent().InverseTransformDirection(val);
		this.get_transform().set_localPosition(_finalPosition + val2);
		this.get_transform().set_localScale(_finalScale * 0.3f);
	}

	private void Update()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		_time += Time.get_deltaTime();
		if (_time > 0.3f)
		{
			Disable();
			return;
		}
		float num = _time / 0.3f;
		float num2 = 1f - num;
		Vector3 val = (WorldStartPosition - WorldEndPosition) * num2;
		Vector3 val2 = this.get_transform().get_parent().InverseTransformDirection(val);
		this.get_transform().set_localPosition(_finalPosition + val2 / _parentScale);
		float num3 = num2 * 270f;
		this.get_transform().set_localEulerAngles(new Vector3(num3, 0f, 0f));
		this.get_transform().set_localScale(_finalScale * (0.3f + num * 0.7f));
	}

	private void Disable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		this.get_transform().set_localPosition(_finalPosition);
		this.get_transform().set_localScale(_finalScale);
		this.get_transform().set_localRotation(_finalRotation);
		Object.Destroy(this);
	}

	private void OnDisable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		this.get_transform().set_localPosition(_finalPosition);
		this.get_transform().set_localScale(_finalScale);
		this.get_transform().set_localRotation(_finalRotation);
		_batcher.onPreBatching -= Disable;
	}
}
