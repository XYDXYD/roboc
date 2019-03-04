using CustomGames;
using Services.Web.Photon;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Simulation
{
	internal sealed class DamageMultiplierEngine : SingleEntityViewEngine<DamageMultiplierNode>, IInitialize, IWaitForFrameworkDestruction
	{
		private const float DMG_MULT_DEF_VAL = 1f;

		private const string MSG_ERROR_DMG_MULT = "Error processing damage multiplier";

		private FasterList<DamageMultiplierNode> _pendingNodes = new FasterList<DamageMultiplierNode>();

		private bool _damageMultiplierReceived;

		private float _damageMultipler = 1f;

		[Inject]
		public GameStartDispatcher gameStartDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceRequestFactory
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			gameStartDispatcher.Register(OnGameStart);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			gameStartDispatcher.Unregister(OnGameStart);
		}

		private void OnGameStart()
		{
			if (WorldSwitching.IsCustomGame())
			{
				TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadAndApplyCustomGameDamageMultiplier);
				return;
			}
			_damageMultipler = 1f;
			DamageMultiplierReceived();
		}

		protected override void Add(DamageMultiplierNode node)
		{
			if (_damageMultiplierReceived)
			{
				node.damageStats.damageMultiplier = _damageMultipler;
			}
			else
			{
				_pendingNodes.Add(node);
			}
		}

		protected override void Remove(DamageMultiplierNode node)
		{
			_pendingNodes.Remove(node);
		}

		private IEnumerator LoadAndApplyCustomGameDamageMultiplier()
		{
			_damageMultipler = 1f;
			IRetrieveCustomGameSessionRequest request = serviceRequestFactory.Create<IRetrieveCustomGameSessionRequest>();
			TaskService<RetrieveCustomGameSessionRequestData> task = new TaskService<RetrieveCustomGameSessionRequestData>(request);
			yield return task;
			if (!task.succeeded)
			{
				RemoteLogger.Error("Error processing damage multiplier", " failed to retrieve damage multiplier setting from custom game cached session", null);
				yield break;
			}
			RetrieveCustomGameSessionRequestData result = task.result;
			if (result == null)
			{
				RemoteLogger.Error("Error processing damage multiplier", " the request's result is null!", null);
				yield break;
			}
			CustomGameSessionData customGameSessionData = result.Data;
			if (customGameSessionData == null)
			{
				RemoteLogger.Error("Error processing damage multiplier", " custom game session data is null", null);
				yield break;
			}
			string value = customGameSessionData.Config["DamageMultiplier"];
			float damageAsPercentage = Convert.ToSingle(value);
			_damageMultipler = damageAsPercentage / 100f;
			DamageMultiplierReceived();
		}

		private void DamageMultiplierReceived()
		{
			_damageMultiplierReceived = true;
			for (int i = 0; i < _pendingNodes.get_Count(); i++)
			{
				DamageMultiplierNode damageMultiplierNode = _pendingNodes.get_Item(i);
				damageMultiplierNode.damageStats.damageMultiplier = _damageMultipler;
			}
			_pendingNodes.FastClear();
		}
	}
}
