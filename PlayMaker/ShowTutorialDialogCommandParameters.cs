using HutongGames.PlayMaker;
using System;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("create a gameobject FSM from within a FSM and also handle injection correctly.")]
	public class ShowTutorialDialogCommandParameters : IPlaymakerCommandInputParameters
	{
		private FsmString _inputString;

		private FsmInt _inputTimeSeconds;

		public ShowTutorialDialogCommandParameters(FsmString bodyTextString, FsmInt timeToDisplaySeconds)
		{
			_inputString = bodyTextString;
			_inputTimeSeconds = timeToDisplaySeconds;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(string))
			{
				return (T)(object)_inputString.get_Value();
			}
			if (typeof(T) == typeof(int))
			{
				return (T)(object)_inputTimeSeconds.get_Value();
			}
			if (typeof(T) == typeof(FsmString))
			{
				return (T)(object)_inputString;
			}
			if (typeof(T) == typeof(FsmInt))
			{
				return (T)(object)_inputTimeSeconds;
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
