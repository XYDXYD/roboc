using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Simulation
{
	internal class PowerConsumptionSequenceEngine : IQueryingEntityViewEngine, IWaitForFrameworkInitialization, IWaitForFrameworkDestruction, IEngine
	{
		private readonly PowerConsumptionSequencer _sequencer;

		private readonly ITaskRoutine _tickTask;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public PowerConsumptionSequenceEngine(PowerConsumptionSequencer sequencer)
		{
			_sequencer = sequencer;
			_tickTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		public void Ready()
		{
		}

		public void OnFrameworkInitialized()
		{
			_tickTask.Start((Action<PausableTaskException>)null, (Action)null);
		}

		public void OnFrameworkDestroyed()
		{
			_tickTask.Stop();
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				float deltaTime = Time.get_deltaTime();
				FasterReadOnlyList<PowerBarConsumptionEntityView> powerBarConsumptionViews = entityViewsDB.QueryEntityViews<PowerBarConsumptionEntityView>();
				for (int i = 0; i < powerBarConsumptionViews.get_Count(); i++)
				{
					powerBarConsumptionViews.get_Item(i).deltaTimeComponent.deltaTime = deltaTime;
				}
				object obj = new object();
				_sequencer.Next<object>(this, ref obj, (Enum)(object)0);
				yield return null;
			}
		}
	}
}
