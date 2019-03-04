using Mothership;
using System;

namespace PlayMaker.Tutorial
{
	public class ChangeToolModeNodeInputParameters : IPlaymakerCommandInputParameters
	{
		private CurrentToolMode.ToolMode _mode;

		public ChangeToolModeNodeInputParameters(Enum toolMode)
		{
			_mode = (CurrentToolMode.ToolMode)(object)toolMode;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(CurrentToolMode.ToolMode))
			{
				return (T)(object)_mode;
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
