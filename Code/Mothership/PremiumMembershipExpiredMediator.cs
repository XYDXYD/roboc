using Svelto.IoC;
using System;

namespace Mothership
{
	internal class PremiumMembershipExpiredMediator
	{
		[Inject]
		internal PremiumMembership PremiumMembership
		{
			private get;
			set;
		}

		public void Register(Action listener)
		{
			PremiumMembership.onSubscriptionExpired += listener;
		}

		public void Deregister(Action listener)
		{
			PremiumMembership.onSubscriptionExpired -= listener;
		}
	}
}
