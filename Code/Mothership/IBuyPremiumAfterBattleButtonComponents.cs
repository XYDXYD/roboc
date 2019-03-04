using Svelto.ECS;

namespace Mothership
{
	internal interface IBuyPremiumAfterBattleButtonComponents
	{
		DispatchOnChange<bool> goBackButtonPressed
		{
			get;
		}
	}
}
