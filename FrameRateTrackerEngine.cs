using Services.Analytics;
using Svelto.Context;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

internal sealed class FrameRateTrackerEngine : IEngine, IWaitForFrameworkDestruction
{
	private const int MAX_VALUE = 4800;

	private IAnalyticsRequestFactory _analyticsRequestFactory;

	private float _dt;

	private float _fps;

	private float[] _fpsDeviation;

	private int _frameCount;

	private float _grossFPSInSession;

	private int _totalAvgFrameInSession;

	private float _updateRate = 0.5f;

	public FrameRateTrackerEngine(IAnalyticsRequestFactory analyticsRequestFactory)
	{
		_analyticsRequestFactory = analyticsRequestFactory;
		_fpsDeviation = new float[4800];
		TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
	}

	void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
	{
		if (_totalAvgFrameInSession > 0 && _grossFPSInSession > 0f)
		{
			CalculateDeviationAndVariance(_grossFPSInSession / (float)_totalAvgFrameInSession);
		}
	}

	private void CalculateDeviationAndVariance(float meanFPS)
	{
		float num = 0f;
		for (int i = 0; i < _totalAvgFrameInSession; i++)
		{
			float num2 = _fpsDeviation[i] - meanFPS;
			num += num2 * num2;
		}
		float variance = num / (float)_totalAvgFrameInSession;
		TaskRunner.get_Instance().Run(FrameRateStats(Mathf.CeilToInt(meanFPS), variance));
	}

	private IEnumerator FrameRateStats(int meanFPS, float variance)
	{
		Scene activeScene = SceneManager.GetActiveScene();
		LogFrameRateDependency frameRateDependency = new LogFrameRateDependency(activeScene.get_name(), meanFPS, (int)Mathf.Sqrt(variance));
		TaskService logFrameRateRequest = _analyticsRequestFactory.Create<ILogFrameRateRequest, LogFrameRateDependency>(frameRateDependency).AsTask();
		yield return logFrameRateRequest;
		if (!logFrameRateRequest.succeeded)
		{
			throw new Exception("Log Frame Rate Request failed", logFrameRateRequest.behaviour.exceptionThrown);
		}
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			if (_totalAvgFrameInSession < 4800 && Mathf.Approximately(Time.get_timeScale(), 1f))
			{
				_frameCount++;
				_dt += Time.get_deltaTime();
				if (_dt > _updateRate)
				{
					_fps = (float)_frameCount / _dt;
					_fpsDeviation[_totalAvgFrameInSession++] = _fps;
					_grossFPSInSession += _fps;
					_frameCount = 0;
					_dt = 0f;
				}
			}
			yield return null;
		}
	}
}
