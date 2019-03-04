using Simulation;
using Svelto.Context;
using Svelto.IoC;
using System;
using System.Collections;
using UnityEngine;

internal sealed class MultiplayerGameTimerClient : IInitialize, IWaitForFrameworkDestruction
{
	private float _currentTime;

	private DateTime _gameStartTime;

	private float _timeElapsed;

	[Inject]
	internal GameStartDispatcher gameStartDispatcher
	{
		private get;
		set;
	}

	public event Action<float> OnTimeUpdated = delegate
	{
	};

	public void OnDependenciesInjected()
	{
		gameStartDispatcher.Register(Start);
	}

	public void OnFrameworkDestroyed()
	{
		gameStartDispatcher.Unregister(Start);
	}

	public void SetCurrentTime(float time)
	{
		_currentTime = time;
	}

	public float GetCurrentTime()
	{
		return _currentTime;
	}

	public float GetElapsedTime()
	{
		return _timeElapsed;
	}

	public DateTime GetGameStartTime()
	{
		return _gameStartTime;
	}

	private void Start()
	{
		_gameStartTime = DateTime.UtcNow;
		TaskRunnerExtensions.Run(Tick());
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			float deltaTime = Time.get_deltaTime();
			_timeElapsed += deltaTime;
			_currentTime -= deltaTime;
			this.OnTimeUpdated(_currentTime);
			if (_currentTime <= 0f)
			{
				_currentTime = 0f;
			}
			yield return null;
		}
	}
}
