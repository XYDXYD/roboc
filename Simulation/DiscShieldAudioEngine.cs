using Fabric;
using Svelto.ECS;

namespace Simulation
{
	internal sealed class DiscShieldAudioEngine : SingleEntityViewEngine<DiscShieldAudioNode>, IQueryingEntityViewEngine, IEngine
	{
		private IEntityViewsDB _entityViewsesDb;

		public IEntityViewsDB entityViewsDB
		{
			set
			{
				_entityViewsesDb = value;
			}
		}

		public void Ready()
		{
		}

		private void HandleSetGlowLoopSoundParameter(IDiscShieldAudioComponent sender, SoundParameterData parameterData)
		{
			DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
			if (_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(parameterData.nodeId, ref discShieldAudioNode))
			{
				EventManager.get_Instance().SetParameter(sender.shieldGlowLoopSoundName, sender.shieldGlowLoopSoundParameterName, parameterData.soundParameter, discShieldAudioNode.objectComponent.discShieldObject.get_gameObject());
			}
		}

		private void HandleStopGlowLoopSound(IDiscShieldAudioComponent sender, int id)
		{
			DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
			if (_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(id, ref discShieldAudioNode))
			{
				EventManager.get_Instance().PostEvent(sender.shieldGlowLoopSoundName, 1, discShieldAudioNode.objectComponent.discShieldObject.get_gameObject());
			}
		}

		private void HandleStopLoopSound(IDiscShieldAudioComponent sender, int id)
		{
			DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
			if (_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(id, ref discShieldAudioNode))
			{
				EventManager.get_Instance().PostEvent(sender.shieldLoopSoundName, 1, discShieldAudioNode.objectComponent.discShieldObject.get_gameObject());
			}
		}

		private void HandlePlayGlowLoopSound(IDiscShieldAudioComponent sender, int id)
		{
			DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
			if (_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(id, ref discShieldAudioNode))
			{
				EventManager.get_Instance().PostEvent(sender.shieldGlowLoopSoundName, 0, discShieldAudioNode.objectComponent.discShieldObject.get_gameObject());
			}
		}

		private void HandlePlayLoopSound(IDiscShieldAudioComponent sender, int id)
		{
			DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
			if (_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(id, ref discShieldAudioNode))
			{
				EventManager.get_Instance().PostEvent(sender.shieldLoopSoundName, 0, discShieldAudioNode.objectComponent.discShieldObject.get_gameObject());
			}
		}

		private void HandlePlayOffSound(IDiscShieldAudioComponent sender, int id)
		{
			DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
			if (_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(id, ref discShieldAudioNode))
			{
				EventManager.get_Instance().PostEvent(sender.shieldOffSoundName, 0, discShieldAudioNode.objectComponent.discShieldObject.get_gameObject());
			}
		}

		private void HandlePlayOnSound(IDiscShieldAudioComponent sender, int id)
		{
			DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
			if (_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(id, ref discShieldAudioNode))
			{
				EventManager.get_Instance().PostEvent(sender.shieldOnSoundName, 0, discShieldAudioNode.objectComponent.discShieldObject.get_gameObject());
			}
		}

		protected override void Add(DiscShieldAudioNode audioNode)
		{
			audioNode.audioComponent.playOnSound.subscribers += HandlePlayOnSound;
			audioNode.audioComponent.playOffSound.subscribers += HandlePlayOffSound;
			audioNode.audioComponent.playLoopSound.subscribers += HandlePlayLoopSound;
			audioNode.audioComponent.playGlowLoopSound.subscribers += HandlePlayGlowLoopSound;
			audioNode.audioComponent.stopLoopSound.subscribers += HandleStopLoopSound;
			audioNode.audioComponent.stopGlowLoopSound.subscribers += HandleStopGlowLoopSound;
			audioNode.audioComponent.setGlowLoopSoundParameter.subscribers += HandleSetGlowLoopSoundParameter;
		}

		protected override void Remove(DiscShieldAudioNode audioNode)
		{
			audioNode.audioComponent.playOnSound.subscribers -= HandlePlayOnSound;
			audioNode.audioComponent.playOffSound.subscribers -= HandlePlayOffSound;
			audioNode.audioComponent.playLoopSound.subscribers -= HandlePlayLoopSound;
			audioNode.audioComponent.playGlowLoopSound.subscribers -= HandlePlayGlowLoopSound;
			audioNode.audioComponent.stopLoopSound.subscribers -= HandleStopLoopSound;
			audioNode.audioComponent.stopGlowLoopSound.subscribers -= HandleStopGlowLoopSound;
			audioNode.audioComponent.setGlowLoopSoundParameter.subscribers -= HandleSetGlowLoopSoundParameter;
		}
	}
}
