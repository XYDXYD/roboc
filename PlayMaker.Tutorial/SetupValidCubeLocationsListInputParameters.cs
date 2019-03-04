using HutongGames.PlayMaker;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayMaker.Tutorial
{
	public class SetupValidCubeLocationsListInputParameters : IPlaymakerCommandInputParameters
	{
		private List<ValidCubeLocationOrientations> _orientations;

		private List<Vector3> _positions;

		private string _cubeTypeId;

		private bool _clearExisting;

		public SetupValidCubeLocationsListInputParameters(FsmArray orientations, FsmArray positions, FsmString cubeTypeId, FsmBool clearExisting)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			_orientations = new List<ValidCubeLocationOrientations>();
			_positions = new List<Vector3>();
			for (int i = 0; i < orientations.get_Length(); i++)
			{
				ValidCubeLocationOrientations item = (ValidCubeLocationOrientations)orientations.get_Values()[i];
				_orientations.Add(item);
			}
			for (int j = 0; j < positions.get_Length(); j++)
			{
				Vector3 item2 = (Vector3)positions.get_Values()[j];
				_positions.Add(item2);
			}
			_cubeTypeId = cubeTypeId.get_Value();
			_clearExisting = clearExisting.get_Value();
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(List<ValidCubeLocationOrientations>))
			{
				return (T)(object)_orientations;
			}
			if (typeof(T) == typeof(List<Vector3>))
			{
				return (T)(object)_positions;
			}
			if (typeof(T) == typeof(string))
			{
				return (T)(object)_cubeTypeId;
			}
			if (typeof(T) == typeof(bool))
			{
				return (T)(object)_clearExisting;
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
