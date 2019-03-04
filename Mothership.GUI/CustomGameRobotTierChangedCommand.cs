using Services.Web;
using Svelto.Command;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Mothership.GUI
{
	internal class CustomGameRobotTierChangedCommand : ICommand
	{
		private uint _newTier;

		[Inject]
		internal IServiceRequestFactory serviceFactory
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
			ICustomGameRobotTierChangedRequest request = serviceFactory.Create<ICustomGameRobotTierChangedRequest>();
			request.Inject((int)_newTier);
			request.ClearCache();
			TaskService robotTierTask = new TaskService(request);
			yield return new HandleTaskServiceWithError(robotTierTask, delegate
			{
			}, delegate
			{
			}).GetEnumerator();
		}
	}
}
