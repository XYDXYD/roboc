using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace PlayMaker.Tutorial
{
	public class AdjustCameraPositionNodeInputParameters : IPlaymakerCommandInputParameters
	{
		private Vector3 _cameraPos;

		private Vector3 _cameraOrientation;

		public AdjustCameraPositionNodeInputParameters(FsmVector3 newCameraPos, FsmVector3 newCameraOrientation)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			Vector3 value = newCameraPos.get_Value();
			float x = value.x;
			Vector3 value2 = newCameraPos.get_Value();
			float y = value2.y;
			Vector3 value3 = newCameraPos.get_Value();
			_cameraPos = new Vector3(x, y, value3.z);
			Vector3 value4 = newCameraOrientation.get_Value();
			float x2 = value4.x;
			Vector3 value5 = newCameraOrientation.get_Value();
			float y2 = value5.y;
			Vector3 value6 = newCameraOrientation.get_Value();
			_cameraOrientation = new Vector3(x2, y2, value6.z);
		}

		public T GetInputParameters<T>()
		{
			return GetInputParameters<T>(0);
		}

		public T GetInputParameters<T>(int which)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			if (typeof(T) == typeof(Vector3) && which == 0)
			{
				return (T)(object)_cameraPos;
			}
			if (typeof(T) == typeof(Vector3) && which == 1)
			{
				return (T)(object)_cameraOrientation;
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
