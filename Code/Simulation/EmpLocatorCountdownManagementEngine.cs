using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation
{
	internal sealed class EmpLocatorCountdownManagementEngine : SingleEntityViewEngine<EmpLocatorCountdownManagementNode>, IQueryingEntityViewEngine, ITickable, IEngine, ITickableBase
	{
		private FasterList<EmpLocatorCountdownManagementNode> _empLocatorsList = new FasterList<EmpLocatorCountdownManagementNode>();

		[Inject]
		internal EmpTargetingLocatorPool empLocatorPool
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void Ready()
		{
		}

		protected override void Add(EmpLocatorCountdownManagementNode node)
		{
			_empLocatorsList.Add(node);
		}

		protected override void Remove(EmpLocatorCountdownManagementNode node)
		{
			_empLocatorsList.Remove(node);
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterListEnumerator<EmpLocatorCountdownManagementNode> enumerator = _empLocatorsList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				EmpLocatorCountdownManagementNode current = enumerator.get_Current();
				if (current.objectComponent.empLocatorObject.get_gameObject().get_activeSelf())
				{
					current.countdownComponent.countdownTimer -= Time.get_deltaTime();
					float countdownTimer = current.countdownComponent.countdownTimer;
					float countdown = current.countdownComponent.countdown;
					EventManager.get_Instance().SetParameter("EMP_Glow_Loop", "EMP_Time", 1f - countdownTimer / countdown, null);
					EventManager.get_Instance().SetParameter("EMP_Glow2_Loop", "EMP_Time", 1f - countdownTimer / countdown, null);
					if (countdownTimer <= 0f)
					{
						int value = current.get_ID();
						current.activationComponent.activateEmpStun.Dispatch(ref value);
						GameObject empLocatorObject = current.objectComponent.empLocatorObject;
						empLocatorObject.SetActive(false);
					}
				}
			}
		}
	}
}
