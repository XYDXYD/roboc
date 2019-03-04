using System;

namespace PlayMaker.Tutorial
{
	public class SelectSpecificTypeOfCubeNodeInputParameters : IPlaymakerCommandInputParameters
	{
		private string typeOfCubeToSelect;

		public SelectSpecificTypeOfCubeNodeInputParameters(string typeOfCubeToSelect_)
		{
			typeOfCubeToSelect = typeOfCubeToSelect_;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(string))
			{
				return (T)(object)typeOfCubeToSelect;
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
