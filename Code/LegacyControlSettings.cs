using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using UnityEngine;

internal sealed class LegacyControlSettings : IInitialize
{
	private const string LEGACY_CONTROLS = "LegacyControls";

	private bool _legacyControls;

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	void IInitialize.OnDependenciesInjected()
	{
		if (PlayerPrefs.HasKey("LegacyControls"))
		{
			_legacyControls = (PlayerPrefs.GetInt("LegacyControls") == 1);
			return;
		}
		ILoadSignupDate loadSignupDate = serviceFactory.Create<ILoadSignupDate>();
		loadSignupDate.SetAnswer(new ServiceAnswer<DateTime>(GetSignupDateSuccess, GetSignupDateFailed));
		loadSignupDate.Execute();
	}

	public bool IsLegacyControls()
	{
		return _legacyControls;
	}

	private void GetSignupDateSuccess(DateTime signupDate)
	{
		DateTime t = new DateTime(2016, 5, 23, 11, 0, 0).ToUniversalTime();
		_legacyControls = (signupDate < t);
	}

	private void GetSignupDateFailed(ServiceBehaviour serviceBehaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(serviceBehaviour);
	}
}
