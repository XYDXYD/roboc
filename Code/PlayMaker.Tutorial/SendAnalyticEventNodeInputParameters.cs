using System;

namespace PlayMaker.Tutorial
{
	public class SendAnalyticEventNodeInputParameters : IPlaymakerCommandInputParameters
	{
		private string _analyticsEvent;

		public SendAnalyticEventNodeInputParameters(string analyticsEvent)
		{
			_analyticsEvent = analyticsEvent;
		}

		public T GetInputParameters<T>()
		{
			if (typeof(T) == typeof(string))
			{
				return (T)(object)_analyticsEvent;
			}
			throw new Exception("Error: conversion to type not implemented");
		}
	}
}
