using Svelto.Context;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Mothership.GarageSkins
{
	public class DisableOnGarageSkinActivated : MonoBehaviour, IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		private GarageBaySkinNotificationObserver observer
		{
			get;
			set;
		}

		public DisableOnGarageSkinActivated()
			: this()
		{
		}

		public unsafe void OnDependenciesInjected()
		{
			observer.AddAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void OnFrameworkDestroyed()
		{
			observer.RemoveAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void ToggleGO(ref bool garageSkinActivated)
		{
			this.get_gameObject().SetActive(!garageSkinActivated);
		}
	}
}
