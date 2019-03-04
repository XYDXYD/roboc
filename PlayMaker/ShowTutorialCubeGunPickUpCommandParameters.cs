using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("create a gameobject FSM from within a FSM and also handle injection correctly.")]
	public class ShowTutorialCubeGunPickUpCommandParameters : IPlaymakerCommandInputParameters
	{
		public GameObject _gameObject;

		public FsmVector3 _position;

		public FsmQuaternion _rotation;

		public FsmGameObject _storeObject;

		public ShowTutorialCubeGunPickUpCommandParameters(FsmGameObject gameObject, FsmVector3 position, FsmVector3 rotation, FsmGameObject storeObject)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			_gameObject = gameObject.get_Value();
			_position = position;
			_rotation = FsmQuaternion.op_Implicit(Quaternion.Euler(rotation.get_Value()));
			_storeObject = storeObject;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(GameObject))
			{
				return (T)(object)_gameObject;
			}
			if (typeof(T) == typeof(FsmVector3))
			{
				return (T)(object)_position;
			}
			if (typeof(T) == typeof(FsmQuaternion))
			{
				return (T)(object)_rotation;
			}
			if (typeof(T) == typeof(FsmGameObject))
			{
				return (T)(object)_storeObject;
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
