using Services.Requests.Interfaces;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Ticker.Legacy;
using System;
using System.Collections;
using UnityEngine;
using Utility;

internal class SpeedometerManagerBase : ITickable, ITickableBase
{
	private float _timeSinceLastUpdate;

	private bool _currentlyEnabled;

	private DateTime _lastRecordedTimestamp;

	private Vector3 _lastRecordedPosition;

	private Rigidbody _rigidBody;

	private GameObject _rbObj;

	private float[] _prevSpeeds;

	private int _lastWrittenSpeed;

	private float _measurementSystemMultiplier;

	private IServiceRequestFactory _serviceFactory;

	protected virtual int _numToAverage => 1;

	protected virtual float _updateInterval => 0.12f;

	private event Action<float> _onSpeedChanged = delegate
	{
	};

	private event Action<bool> _onEnabledChanged = delegate
	{
	};

	public virtual void Initialise(Rigidbody rigidBody, ITicker ticker, IServiceRequestFactory serviceFactory)
	{
		CubeSpeedometer[] componentsInChildren = rigidBody.GetComponentsInChildren<CubeSpeedometer>();
		if (componentsInChildren.Length != 0)
		{
			_serviceFactory = serviceFactory;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetSpeedometerManager(this);
			}
			_prevSpeeds = new float[_numToAverage];
			_rigidBody = rigidBody;
			_rbObj = _rigidBody.get_gameObject();
			Reset();
			ticker.Add(this);
			TaskRunner.get_Instance().Run(LoadPlatformConfigurationValues());
		}
	}

	public void Register(Action<float> onSpeedChanged, Action<bool> onEnabledChanged)
	{
		_onSpeedChanged += onSpeedChanged;
		_onEnabledChanged += onEnabledChanged;
		onEnabledChanged(_currentlyEnabled);
	}

	public void Unregister(Action<float> onSpeedChanged, Action<bool> onEnabledChanged)
	{
		_onSpeedChanged -= onSpeedChanged;
		_onEnabledChanged -= onEnabledChanged;
	}

	public void Tick(float deltaTime)
	{
		if (!(_rigidBody != null))
		{
			return;
		}
		if (_rbObj.get_activeInHierarchy() != _currentlyEnabled)
		{
			_currentlyEnabled = _rbObj.get_activeInHierarchy();
			this._onEnabledChanged(_currentlyEnabled);
			if (_currentlyEnabled)
			{
				Reset();
			}
		}
		if (_currentlyEnabled)
		{
			_timeSinceLastUpdate += deltaTime;
			if (_timeSinceLastUpdate > _updateInterval)
			{
				UpdateSpeed();
				_timeSinceLastUpdate = 0f;
			}
		}
	}

	private void Reset()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		_timeSinceLastUpdate = 0f;
		_lastRecordedPosition = _rigidBody.get_worldCenterOfMass();
		_lastRecordedTimestamp = DateTime.UtcNow;
		for (int i = 0; i < _numToAverage; i++)
		{
			_prevSpeeds[i] = 0f;
		}
	}

	private void UpdateSpeed()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		DateTime utcNow = DateTime.UtcNow;
		Vector3 worldCenterOfMass = _rigidBody.get_worldCenterOfMass();
		Vector3 val = worldCenterOfMass - _lastRecordedPosition;
		double num = (double)val.get_magnitude() / (utcNow - _lastRecordedTimestamp).TotalSeconds;
		_prevSpeeds[_lastWrittenSpeed] = (float)num;
		_lastWrittenSpeed = (_lastWrittenSpeed + 1) % _numToAverage;
		float num2 = 0f;
		for (int i = 0; i < _numToAverage; i++)
		{
			num2 += _prevSpeeds[i];
		}
		this._onSpeedChanged(num2 / (float)_numToAverage * _measurementSystemMultiplier);
		_lastRecordedTimestamp = utcNow;
		_lastRecordedPosition = worldCenterOfMass;
	}

	private IEnumerator LoadPlatformConfigurationValues()
	{
		ILoadPlatformConfigurationRequest request = _serviceFactory.Create<ILoadPlatformConfigurationRequest>();
		TaskService<PlatformConfigurationSettings> task = request.AsTask();
		yield return new HandleTaskServiceWithError(task, delegate
		{
			Console.Log("error occured in Speedometer Manager Base");
		}, delegate
		{
			Console.Log("error occured in Speedometer Manager Base");
		}).GetEnumerator();
		if (task.succeeded)
		{
			_measurementSystemMultiplier = ((!task.result.UseDecimalSystem) ? 1f : 1.61f);
		}
		else
		{
			Console.LogError("Unable to load platform config");
		}
	}
}
