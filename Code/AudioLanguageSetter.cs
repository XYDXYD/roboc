using Fabric;
using Svelto.Context;
using Svelto.IoC;
using System;

internal class AudioLanguageSetter : IInitialize, IWaitForFrameworkDestruction
{
	[Inject]
	internal LocalisationWrapper localiseWrapper
	{
		private get;
		set;
	}

	public void OnDependenciesInjected()
	{
		SetAudioLanguage();
		LocalisationWrapper localiseWrapper = this.localiseWrapper;
		localiseWrapper.OnLocalisationChanged = (Action)Delegate.Combine(localiseWrapper.OnLocalisationChanged, new Action(SetAudioLanguage));
	}

	public void OnFrameworkDestroyed()
	{
		LocalisationWrapper localiseWrapper = this.localiseWrapper;
		localiseWrapper.OnLocalisationChanged = (Action)Delegate.Remove(localiseWrapper.OnLocalisationChanged, new Action(SetAudioLanguage));
	}

	private void SetAudioLanguage()
	{
		FabricManager.get_Instance().SetLanguageByName(StringTableBase<StringTable>.Instance.language);
		int languageIndex = FabricManager.get_Instance().GetLanguageIndex();
		FabricManager.get_Instance()._defaultLanguage = languageIndex;
	}
}
