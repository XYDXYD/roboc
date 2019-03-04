using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("create a gameobject FSM from within a FSM and also handle injection correctly.")]
	public class CreateGameObjectWithInjectCommandParameters : IPlaymakerCommandInputParameters
	{
		private FsmString _inputString;

		private FsmGameObject _outputGameObject;

		private Vector3 _spawnPoint;

		private Quaternion _spawnRotation;

		public CreateGameObjectWithInjectCommandParameters(FsmString nameOfPrefab, FsmGameObject inputGameObject, Vector3 spawnPoint, Quaternion spawnRotation)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			_inputString = nameOfPrefab;
			_outputGameObject = inputGameObject;
			_spawnPoint = spawnPoint;
			_spawnRotation = spawnRotation;
		}

		public T GetInputParameters<T>()
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			if (typeof(T) == typeof(string))
			{
				return (T)(object)_inputString.get_Value();
			}
			if (typeof(T) == typeof(Vector3))
			{
				return (T)(object)_spawnPoint;
			}
			if (typeof(T) == typeof(Quaternion))
			{
				return (T)(object)_spawnRotation;
			}
			if (typeof(T) == typeof(FsmGameObject))
			{
				return (T)(object)_outputGameObject;
			}
			if (typeof(T) == typeof(FsmString))
			{
				return (T)(object)_inputString;
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
