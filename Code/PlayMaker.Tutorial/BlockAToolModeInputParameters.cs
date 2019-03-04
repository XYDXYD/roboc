using HutongGames.PlayMaker;
using Mothership;
using System;

namespace PlayMaker.Tutorial
{
	public class BlockAToolModeInputParameters : IPlaymakerCommandInputParameters
	{
		private FsmEnum toolToBlock;

		private bool blockOrUnblock;

		public BlockAToolModeInputParameters(FsmEnum toolToBlock_, bool blockOrUnBlock_)
		{
			toolToBlock = toolToBlock_;
			blockOrUnblock = blockOrUnBlock_;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(bool))
			{
				return (T)(object)blockOrUnblock;
			}
			if (typeof(T) == typeof(CurrentToolMode.ToolMode))
			{
				return (T)(object)toolToBlock.get_Value();
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
