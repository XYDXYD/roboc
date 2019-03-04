using Fabric;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;

namespace Simulation
{
	internal class PlaySoundWhenParticleDestroyedEngine : SingleEntityViewEngine<PlaySoundWhenParticleDestroyedNode>, IQueryingEntityViewEngine, IEngine
	{
		private FasterList<PlaySoundWhenParticleDestroyedNode> _activeNodesList = new FasterList<PlaySoundWhenParticleDestroyedNode>();

		private ITaskRoutine _taskRunner;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public PlaySoundWhenParticleDestroyedEngine()
		{
			_taskRunner = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Update);
		}

		public void Ready()
		{
		}

		protected override void Add(PlaySoundWhenParticleDestroyedNode node)
		{
			node.hardwareDisabledComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)HandleNewNode);
			HandleNewNode(node.get_ID(), node.hardwareDisabledComponent.enabled);
		}

		protected override void Remove(PlaySoundWhenParticleDestroyedNode node)
		{
			node.hardwareDisabledComponent.isPartEnabled.StopNotify((Action<int, bool>)HandleNewNode);
			HandleNewNode(node.get_ID(), isEnabled: false);
		}

		private void HandleNewNode(int entityID, bool isEnabled)
		{
			PlaySoundWhenParticleDestroyedNode playSoundWhenParticleDestroyedNode = entityViewsDB.QueryEntityView<PlaySoundWhenParticleDestroyedNode>(entityID);
			if (isEnabled)
			{
				_activeNodesList.Add(playSoundWhenParticleDestroyedNode);
				if (_activeNodesList.get_Count() == 1)
				{
					_taskRunner.Start((Action<PausableTaskException>)null, (Action)null);
				}
			}
			else
			{
				_activeNodesList.Remove(playSoundWhenParticleDestroyedNode);
				if (_activeNodesList.get_Count() == 0)
				{
					_taskRunner.Stop();
				}
			}
		}

		private IEnumerator Update()
		{
			while (true)
			{
				int nodesLength = _activeNodesList.get_Count();
				for (int i = 0; i < nodesLength; i++)
				{
					IPlaySoundWhenParticleDestroyedComponent playSoundWhenParticleDestroyedComponent = _activeNodesList.get_Item(i).playSoundWhenParticleDestroyedComponent;
					int particleCount = playSoundWhenParticleDestroyedComponent.particleSys.get_particleCount();
					if (particleCount < playSoundWhenParticleDestroyedComponent.previousParticleNumber)
					{
						EventManager.get_Instance().PostEvent(playSoundWhenParticleDestroyedComponent.soundToPlay, 0, (object)null, playSoundWhenParticleDestroyedComponent.particleSys.get_gameObject());
					}
					playSoundWhenParticleDestroyedComponent.previousParticleNumber = particleCount;
				}
				yield return null;
			}
		}
	}
}
