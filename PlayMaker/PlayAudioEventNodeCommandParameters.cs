using Fabric;
using HutongGames.PlayMaker;
using System;

namespace PlayMaker
{
	public class PlayAudioEventNodeCommandParameters : IPlaymakerCommandInputParameters
	{
		private FsmString _input;

		private FsmEnum _enum;

		public PlayAudioEventNodeCommandParameters(FsmString input_, FsmEnum enum_)
		{
			_input = input_;
			_enum = enum_;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(string))
			{
				return (T)(object)_input.get_Value();
			}
			if (typeof(T) == typeof(FsmString))
			{
				return (T)(object)_input;
			}
			if (typeof(T) == typeof(EventAction))
			{
				return (T)(object)_enum.get_Value();
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
