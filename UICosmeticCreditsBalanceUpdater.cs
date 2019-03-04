using Mothership;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Globalization;
using UnityEngine;

internal class UICosmeticCreditsBalanceUpdater : MonoBehaviour, IInitialize
{
	public UILabel cosmeticCreditsLabel;

	[Inject]
	internal CosmeticCreditsObserver cosmeticCreditsObserver
	{
		private get;
		set;
	}

	public UICosmeticCreditsBalanceUpdater()
		: this()
	{
	}

	unsafe void IInitialize.OnDependenciesInjected()
	{
		cosmeticCreditsObserver.AddAction(new ObserverAction<long>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}

	private void UpdateValueLabel(ref long cosmeticCreditsAmount)
	{
		if (cosmeticCreditsLabel != null)
		{
			cosmeticCreditsLabel.set_text(cosmeticCreditsAmount.ToString("N0", CultureInfo.InvariantCulture));
		}
	}

	private unsafe void OnDestroy()
	{
		cosmeticCreditsObserver.RemoveAction(new ObserverAction<long>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
	}
}
