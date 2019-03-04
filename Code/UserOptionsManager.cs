using Svelto.IoC;
using Svelto.ServiceLayer;

internal sealed class UserOptionsManager : IInitialize
{
	private static UserOptionsData _userOptionsData;

	private bool _uoInitialised;

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	public bool initialised => _uoInitialised;

	void IInitialize.OnDependenciesInjected()
	{
		if (_userOptionsData == null)
		{
			LoadOptions();
		}
		else
		{
			_uoInitialised = true;
		}
	}

	private void OnLoad(UserOptionsData options)
	{
		_userOptionsData = options;
		_uoInitialised = true;
	}

	private void LoadOptions()
	{
		IServiceRequest serviceRequest = serviceFactory.Create<ILoadUserOptionsRequest>().SetAnswer(new ServiceAnswer<UserOptionsData>(OnLoad));
		serviceRequest.Execute();
	}

	private void SaveOptions()
	{
		IServiceRequest serviceRequest = serviceFactory.Create<ISaveUserOptionsRequest, UserOptionsData>(_userOptionsData);
		serviceRequest.Execute();
	}
}
