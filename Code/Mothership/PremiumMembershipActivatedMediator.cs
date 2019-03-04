using Svelto.IoC;
using System;

namespace Mothership
{
	internal class PremiumMembershipActivatedMediator
	{
		[Inject]
		internal PremiumMembership PremiumMembership
		{
			private get;
			set;
		}

		public void Register(Action<TimeSpan> listener)
		{
			PremiumMembership.onSubscriptionActivated += listener;
		}

		public void Deregister(Action<TimeSpan> listener)
		{
			PremiumMembership.onSubscriptionActivated -= listener;
		}
	}
}
