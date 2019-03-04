using Svelto.IoC;
using System;
using UnityEngine;

internal sealed class CubeAntiGravity : MonoBehaviour, IDisableInMultiplayer
{
	public float ForceUpwards = 1f;

	[NonSerialized]
	public int numberHeliumCubes = 1;

	private Rigidbody RB;

	private float invMaxUpVelocity;

	private int _lastNumHeliumCubes = -1;

	private float _numHeliumModifer = 1f;

	private const float MAX_UP_VELOCITY = 5f;

	private const float MIN_CEILING_MULTIPLIER = 0.5f;

	private const int NUM_HELIUM_FULL_CEILING = 20;

	private const float hoverDelayTime = 0.1f;

	private bool _functionalEnabled = true;

	private float startTime;

	[Inject]
	internal CeilingHeightManager ceilingHeightManager
	{
		private get;
		set;
	}

	public CubeAntiGravity()
		: this()
	{
	}

	private void Awake()
	{
		startTime = Time.get_time();
	}

	private void Start()
	{
		RB = GetParentRB();
		invMaxUpVelocity = 0.2f;
	}

	private Rigidbody GetParentRB()
	{
		Rigidbody component = this.GetComponent<Rigidbody>();
		if (component != null)
		{
			return component;
		}
		component = this.get_transform().get_parent().get_gameObject()
			.GetComponent<Rigidbody>();
		if (component != null)
		{
			return component;
		}
		return null;
	}

	private void ProcessNumHeliumModifer()
	{
		if (numberHeliumCubes != _lastNumHeliumCubes)
		{
			_numHeliumModifer = 1f / (0.5f + 0.5f * (Mathf.Clamp((float)numberHeliumCubes, 1f, 20f) / 20f));
			_lastNumHeliumCubes = numberHeliumCubes;
		}
	}

	private void FixedUpdate()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (RB != null && !(Time.get_time() < startTime + 0.1f) && _functionalEnabled)
		{
			ProcessNumHeliumModifer();
			float num = invMaxUpVelocity * (5f - Mathf.Clamp(Vector3.Dot(RB.get_velocity(), Vector3.get_up()), 0f, 5f));
			CeilingHeightManager ceilingHeightManager = this.ceilingHeightManager;
			Vector3 inputForce = -Physics.get_gravity() * ForceUpwards * num * Time.get_fixedDeltaTime();
			Vector3 position = this.get_transform().get_position();
			Vector3 val = ceilingHeightManager.ApplyMaxCeilingToForce(inputForce, position.y * _numHeliumModifer);
			RB.AddForceAtPosition(val, this.get_transform().get_position());
		}
	}

	public void Break()
	{
		Object.Destroy(this);
	}

	public void SetFunctionalsEnabled(bool functionalsEnabled)
	{
		_functionalEnabled = functionalsEnabled;
	}
}
