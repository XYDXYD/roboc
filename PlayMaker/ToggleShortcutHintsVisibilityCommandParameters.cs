using HutongGames.PlayMaker;
using System;

namespace PlayMaker
{
	[ActionCategory(/*Could not decode attribute arguments.*/)]
	[Tooltip("toggle visibility of the shortcut hints in the top left of the screen")]
	public class ToggleShortcutHintsVisibilityCommandParameters : IPlaymakerCommandInputParameters
	{
		private FsmBool _input;

		public ToggleShortcutHintsVisibilityCommandParameters(FsmBool input)
		{
			_input = input;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(bool))
			{
				return (T)(object)_input.get_Value();
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
