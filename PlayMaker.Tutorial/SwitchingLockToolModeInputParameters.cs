using System;

namespace PlayMaker.Tutorial
{
	public class SwitchingLockToolModeInputParameters : IPlaymakerCommandInputParameters
	{
		private bool setting;

		public SwitchingLockToolModeInputParameters(bool setting_)
		{
			setting = setting_;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(bool))
			{
				return (T)(object)setting;
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
