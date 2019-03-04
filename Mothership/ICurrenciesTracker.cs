using System;
using System.Collections;

namespace Mothership
{
	internal interface ICurrenciesTracker
	{
		void RegisterWalletChangedListener(Action<Wallet> onWalletChangedCallback);

		void UnRegisterWalletChangedListener(Action<Wallet> onWalletChangedCallback);

		void RetrieveCurrentWallet(Action<Wallet> callbackOnLoaded);

		void RefreshWallet(Action<Wallet> callback = null);

		IEnumerator RefreshUserWalletEnumerator();
	}
}
