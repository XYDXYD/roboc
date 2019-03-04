using SocialServiceLayer;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership.GUI.Party
{
	internal class PartyRobotTierChangedCommand : ICommand
	{
		private uint _newTier;

		[Inject]
		internal PartyGUIController partyGUIController
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal ISocialRequestFactory socialFactory
		{
			private get;
			set;
		}

		public ICommand Inject(uint newTier)
		{
			_newTier = newTier;
			return this;
		}

		public void Execute()
		{
			TaskRunner.get_Instance().Run((Func<IEnumerator>)ExecuteTask);
		}

		private IEnumerator ExecuteTask()
		{
			IGetPlatoonDataRequest getPlatoonDataRequest = socialFactory.Create<IGetPlatoonDataRequest>();
			TaskService<Platoon> getPlatoonTask = new TaskService<Platoon>(getPlatoonDataRequest);
			yield return new HandleTaskServiceWithError(getPlatoonTask, delegate
			{
			}, delegate
			{
			}).GetEnumerator();
			if (getPlatoonTask.succeeded && getPlatoonTask.result.isInPlatoon)
			{
				IPlatoonRobotTierChangeRequest robotTierChangedServiceRequest = socialFactory.Create<IPlatoonRobotTierChangeRequest>();
				robotTierChangedServiceRequest.Inject((int)_newTier);
				TaskService robotChangedTask = new TaskService(robotTierChangedServiceRequest);
				yield return robotChangedTask;
				if (!robotChangedTask.succeeded)
				{
					ErrorWindow.ShowServiceErrorWindow(robotChangedTask.behaviour);
				}
			}
		}
	}
}
