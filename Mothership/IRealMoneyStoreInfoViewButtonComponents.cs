using Svelto.ECS;

namespace Mothership
{
	internal interface IRealMoneyStoreInfoViewButtonComponents
	{
		DispatchOnChange<bool> buyButtonPressed
		{
			get;
		}

		DispatchOnChange<bool> goBackButtonPressed
		{
			get;
		}
	}
}
