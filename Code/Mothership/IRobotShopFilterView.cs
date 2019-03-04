using Svelto.DataStructures;
using Svelto.Factories;
using System;

namespace Mothership
{
	internal interface IRobotShopFilterView
	{
		void InitList(RobotShopFilter robotShopFilter, FasterList<string> filterStringKeys, IGameObjectFactory gameObjectFactory, Action<uint> filterUpdatedCallback, Action<RobotShopFilter, bool> filterClickedCallback, uint defaultSelectedIndex);

		void UpdateFilterStrings(FasterList<string> filterStringKeys = null);

		void HideView();

		uint GetValue();

		void Reset();
	}
}
