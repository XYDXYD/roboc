using System;

namespace PlayMaker.Tutorial
{
	public class FinishedReachedTutorialNodeInputParameters : IPlaymakerCommandInputParameters
	{
		public T GetInputParameters<T>()
		{
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
