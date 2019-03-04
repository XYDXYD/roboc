using Svelto.Ticker.Legacy;
using System;
using UnityEngine;

internal sealed class CeilingHeightManager : ITickable, ITickableBase
{
	private float _mapMaxCeiling = 1000f;

	private float _bobValue;

	private float _bobAngle;

	private const float _bobFrequency = 50f;

	private const float _bobMagnitude = 10f;

	public void SetMaxCeilingHeight(float height)
	{
		_mapMaxCeiling = height;
	}

	public float GetMaxCeilingHeight()
	{
		return _mapMaxCeiling;
	}

	public Vector3 ApplyMaxCeilingToForce(Vector3 inputForce, float currentHeight)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		float curve = InterpollationCurves.GetCurve(Mathf.Clamp(currentHeight + _bobValue, 0f, _mapMaxCeiling), _mapMaxCeiling, InterpollationCurve.SharpInverseSquare);
		float num = inputForce.y * curve;
		return inputForce - new Vector3(0f, inputForce.y, 0f) + Vector3.get_up() * num;
	}

	public Vector3 ApplyMaxCeilingToForce(Vector3 inputForce, float currentHeight, float maxCeilingMultiplier)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		float num = _mapMaxCeiling * maxCeilingMultiplier;
		float curve = InterpollationCurves.GetCurve(Mathf.Clamp(currentHeight + _bobValue, 0f, num), num, InterpollationCurve.SharpInverseSquare);
		float num2 = inputForce.y * curve;
		return inputForce - new Vector3(0f, inputForce.y, 0f) + Vector3.get_up() * num2;
	}

	public float ApplyMaxCeilingToForce(float inputForce, float currentHeight)
	{
		float curve = InterpollationCurves.GetCurve(Mathf.Clamp(currentHeight + _bobValue, 0f, _mapMaxCeiling), _mapMaxCeiling, InterpollationCurve.SharpInverseSquare);
		return inputForce * curve;
	}

	public void Tick(float deltaTime)
	{
		_bobAngle += deltaTime * 50f;
		_bobValue = 10f * Mathf.Sin((float)Math.PI / 180f * _bobAngle);
	}

	public float GetBobModification(float currentHeight)
	{
		float num = 1f - InterpollationCurves.GetCurve(Mathf.Clamp(currentHeight, 0f, _mapMaxCeiling), _mapMaxCeiling, InterpollationCurve.SharpInverseSquare);
		return _bobValue * num;
	}
}
