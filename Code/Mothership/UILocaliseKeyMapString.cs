using Svelto.IoC;
using System;
using UnityEngine;

namespace Mothership
{
	internal sealed class UILocaliseKeyMapString : MonoBehaviour, IInitialize
	{
		public string stringKey;

		public UILabel label;

		[Inject]
		internal ControlsChangedObserver controlsChangedObserver
		{
			private get;
			set;
		}

		[Inject]
		internal LocalisationWrapper localiseWrapper
		{
			private get;
			set;
		}

		public UILocaliseKeyMapString()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			controlsChangedObserver.onControlsChanged += UpdateLabel;
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Combine(localiseWrapper.OnLocalisationChanged, new Action(UpdateLabel));
			UpdateLabel();
		}

		private void OnDestroy()
		{
			controlsChangedObserver.onControlsChanged -= UpdateLabel;
			LocalisationWrapper localiseWrapper = this.localiseWrapper;
			localiseWrapper.OnLocalisationChanged = (Action)Delegate.Remove(localiseWrapper.OnLocalisationChanged, new Action(UpdateLabel));
		}

		private void UpdateLabel()
		{
			label.set_text(StringTableBase<StringTable>.Instance.GetReplaceStringWithInputActionKeyMap(stringKey));
		}
	}
}
