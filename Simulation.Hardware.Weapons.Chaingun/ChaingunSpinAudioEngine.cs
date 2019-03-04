using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Ticker.Legacy;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal class ChaingunSpinAudioEngine : SingleEntityViewEngine<ChaingunSpinAudioNode>, IQueryingEntityViewEngine, ITickable, IEngine, ITickableBase
	{
		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public void Ready()
		{
		}

		public void Tick(float deltaSec)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			ReadOnlyDictionary<int, SharedSpinDataNode> val = entityViewsDB.QueryIndexableEntityViews<SharedSpinDataNode>();
			FasterReadOnlyList<ChaingunSpinAudioNode> val2 = entityViewsDB.QueryEntityViews<ChaingunSpinAudioNode>();
			for (int i = 0; i < val2.get_Count(); i++)
			{
				ChaingunSpinAudioNode chaingunSpinAudioNode = val2.get_Item(i);
				SharedSpinDataNode sharedSpinDataNode = val.get_Item(chaingunSpinAudioNode.get_ID());
				float spinPower = sharedSpinDataNode.sharedChaingunSpinComponent.sharedSpinData.spinPower;
				if (spinPower > 0f)
				{
					EventManager.get_Instance().SetParameter(chaingunSpinAudioNode.spinAudioComponent.spinLoopAudioEvent, chaingunSpinAudioNode.spinAudioComponent.spinLoopPowerParameter, spinPower, chaingunSpinAudioNode.transformComponent.T.get_gameObject());
					EventManager.get_Instance().SetParameter(chaingunSpinAudioNode.firingAudioComponent.firingAudio, chaingunSpinAudioNode.spinAudioComponent.spinLoopPowerParameter, spinPower, chaingunSpinAudioNode.transformComponent.T.get_gameObject());
				}
			}
		}

		protected override void Add(ChaingunSpinAudioNode node)
		{
			node.spinEventComponent.spinStarted.subscribers += HandleSpinStarted;
			node.spinEventComponent.spinStopped.subscribers += HandleSpinStopped;
		}

		protected override void Remove(ChaingunSpinAudioNode node)
		{
			node.spinEventComponent.spinStarted.subscribers -= HandleSpinStarted;
			node.spinEventComponent.spinStopped.subscribers -= HandleSpinStopped;
		}

		private void HandleSpinStarted(int weaponId)
		{
			ChaingunSpinAudioNode node = entityViewsDB.QueryEntityView<ChaingunSpinAudioNode>(weaponId);
			StartSpinLoop(node);
		}

		private void HandleSpinStopped(int weaponId)
		{
			ChaingunSpinAudioNode node = entityViewsDB.QueryEntityView<ChaingunSpinAudioNode>(weaponId);
			StopSpinLoop(node);
		}

		private void StartSpinLoop(ChaingunSpinAudioNode node)
		{
			EventManager.get_Instance().PostEvent(node.spinAudioComponent.spinLoopAudioEvent, 0, (object)null, node.transformComponent.T.get_gameObject());
		}

		private void StopSpinLoop(ChaingunSpinAudioNode node)
		{
			if (EventManager.get_Instance() != null)
			{
				EventManager.get_Instance().PostEvent(node.spinAudioComponent.spinLoopAudioEvent, 1, (object)null, node.transformComponent.T.get_gameObject());
			}
		}
	}
}
