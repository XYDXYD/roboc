using Services.Requests.Interfaces;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using Svelto.Ticker.Legacy;
using System;
using System.Collections;
using UnityEngine;
using Utility;

internal class AltimeterManagerBase : ITickable, ITickableBase
{
	private Rigidbody _rigidBody;

	private GameObject _rbObj;

	private int _framesSinceLastUpdate;

	private bool _currentlyEnabled;

	private float _groundHeight;

	private float _measurementSystemMultiplier;

	private IServiceRequestFactory _serviceFactory;

	protected virtual int _updateInterval => 1;

	private event Action<float> _onHeightChanged = delegate
	{
	};

	private event Action<bool> _onEnabledChanged = delegate
	{
	};

	public virtual void Initialise(Rigidbody rigidBody, float groundHeight, ITicker ticker, IServiceRequestFactory serviceFactory)
	{
		CubeAltimeter[] componentsInChildren = rigidBody.GetComponentsInChildren<CubeAltimeter>();
		if (componentsInChildren.Length != 0)
		{
			_serviceFactory = serviceFactory;
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].SetAltimeterManager(this);
			}
			_rigidBody = rigidBody;
			_rbObj = _rigidBody.get_gameObject();
			_groundHeight = groundHeight;
			UpdateHeight();
			ticker.Add(this);
			TaskRunner.get_Instance().Run(LoadPlatformConfigurationValues());
		}
	}

	public void Register(Action<float> onHeightChanged, Action<bool> onEnabledChanged)
	{
		_onHeightChanged += onHeightChanged;
		_onEnabledChanged += onEnabledChanged;
		onEnabledChanged(_currentlyEnabled);
	}

	public void Unregister(Action<float> onHeightChanged, Action<bool> onEnabledChanged)
	{
		_onHeightChanged -= onHeightChanged;
		_onEnabledChanged -= onEnabledChanged;
	}

	public void Tick(float deltaTime)
	{
		if (_rigidBody != null)
		{
			if (_rbObj.get_activeInHierarchy() != _currentlyEnabled)
			{
				_currentlyEnabled = _rbObj.get_activeInHierarchy();
				this._onEnabledChanged(_currentlyEnabled);
			}
			if (_currentlyEnabled && ++_framesSinceLastUpdate > _updateInterval)
			{
				UpdateHeight();
				_framesSinceLastUpdate = 0;
			}
		}
	}

	private void UpdateHeight()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 worldCenterOfMass = _rigidBody.get_worldCenterOfMass();
		float obj = (worldCenterOfMass.y - _groundHeight) * _measurementSystemMultiplier;
		this._onHeightChanged(obj);
	}

	private IEnumerator LoadPlatformConfigurationValues()
	{
		ILoadPlatformConfigurationRequest request = _serviceFactory.Create<ILoadPlatformConfigurationRequest>();
		TaskService<PlatformConfigurationSettings> task = request.AsTask();
		yield return new HandleTaskServiceWithError(task, delegate
		{
			Console.Log("error occured in Altimeter Manager Base");
		}, delegate
		{
			Console.Log("error occured in Altimeter Manager Base");
		}).GetEnumerator();
		if (task.succeeded)
		{
			_measurementSystemMultiplier = ((!task.result.UseDecimalSystem) ? 1f : 0.3f);
		}
		else
		{
			Console.LogError("Unable to load platform config");
		}
	}
}
