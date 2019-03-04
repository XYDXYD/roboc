using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ECS.Legacy;
using Svelto.Ticker.Legacy;
using UnityEngine;

namespace Simulation
{
	internal sealed class DiscShieldEffectsEngine : SingleEntityViewEngine<DiscShieldEffectsNode>, IQueryingEntityViewEngine, ITickable, IEngine, ITickableBase
	{
		private IEntityViewsDB _entityViewsesDb;

		private SoundParameterData _soundParameterData = new SoundParameterData();

		private FasterList<int> _closeEffectNodeList = new FasterList<int>();

		private FasterList<int> _nearToCloseEffectNodeList = new FasterList<int>();

		private FasterList<int> _openEffectNodeList = new FasterList<int>();

		private FasterList<int> _closeEffectNodeToRemoveList = new FasterList<int>();

		private FasterList<int> _nearToCloseEffectNodeToRemoveList = new FasterList<int>();

		private FasterList<int> _openEffectNodeToRemoveList = new FasterList<int>();

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

		protected override void Add(DiscShieldEffectsNode shieldNode)
		{
			shieldNode.effectsComponent.startOpenEffect.subscribers += HandleStartOpenEffect;
			shieldNode.effectsComponent.startNearToCloseEffect.subscribers += HandleStartNearToCloseEffect;
			shieldNode.effectsComponent.startCloseEffect.subscribers += HandleStartCloseEffect;
		}

		protected override void Remove(DiscShieldEffectsNode shieldNode)
		{
			shieldNode.effectsComponent.startOpenEffect.subscribers -= HandleStartOpenEffect;
			shieldNode.effectsComponent.startNearToCloseEffect.subscribers -= HandleStartNearToCloseEffect;
			shieldNode.effectsComponent.startCloseEffect.subscribers -= HandleStartCloseEffect;
		}

		public void Tick(float deltaSec)
		{
			UpdateOpenEffect();
			UpdateNearToCloseEffect();
			UpdateCloseEffect();
		}

		private void HandleStartCloseEffect(IDiscShieldEffectsComponent sender, int nodeId)
		{
			_closeEffectNodeList.Add(nodeId);
		}

		private void HandleStartNearToCloseEffect(IDiscShieldEffectsComponent sender, int nodeId)
		{
			_nearToCloseEffectNodeList.Add(nodeId);
		}

		private void HandleStartOpenEffect(IDiscShieldEffectsComponent sender, int nodeId)
		{
			_openEffectNodeList.Add(nodeId);
		}

		private void UpdateOpenEffect()
		{
			for (int i = 0; i < _openEffectNodeList.get_Count(); i++)
			{
				int value = _openEffectNodeList.get_Item(i);
				DiscShieldEffectsNode discShieldEffectsNode = default(DiscShieldEffectsNode);
				DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
				if (!_entityViewsesDb.TryQueryEntityView<DiscShieldEffectsNode>(value, ref discShieldEffectsNode) || !_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(value, ref discShieldAudioNode))
				{
					continue;
				}
				IDiscShieldEffectsComponent effectsComponent = discShieldEffectsNode.effectsComponent;
				float openingTimer = effectsComponent.openingTimer;
				float openTime = effectsComponent.openTime;
				Renderer discShieldRenderer = effectsComponent.discShieldRenderer;
				float @float = discShieldRenderer.get_material().GetFloat("_activate");
				if (@float < 1f)
				{
					if (openingTimer == 0f)
					{
						discShieldAudioNode.audioComponent.playOnSound.Dispatch(ref value);
					}
					@float = Mathf.Lerp(0f, 1f, openingTimer / openTime);
					discShieldRenderer.get_material().SetFloat("_activate", @float);
					effectsComponent.openingTimer += Time.get_deltaTime();
				}
				else
				{
					UpdateRingEffect(ref value, discShieldAudioNode, effectsComponent);
				}
			}
			RemoveOpenedShields();
		}

		private void UpdateRingEffect(ref int id, DiscShieldAudioNode audioNode, IDiscShieldEffectsComponent effectsData)
		{
			float ringOpeningTimer = effectsData.ringOpeningTimer;
			Renderer ringEffectRenderer = effectsData.ringEffectRenderer;
			float @float = ringEffectRenderer.get_material().GetFloat("_Range");
			float ringOpenTime = effectsData.ringOpenTime;
			if (@float > 0f && ringOpeningTimer <= ringOpenTime)
			{
				if (ringOpeningTimer == 0f)
				{
					audioNode.audioComponent.playLoopSound.Dispatch(ref id);
				}
				@float = Mathf.Lerp(1f, 0f, ringOpeningTimer / ringOpenTime);
				ringEffectRenderer.get_material().SetFloat("_Range", @float);
				effectsData.ringOpeningTimer += Time.get_deltaTime();
				return;
			}
			ringOpeningTimer = effectsData.ringClosingTimer;
			float ringCloseTime = effectsData.ringCloseTime;
			if (@float < 1f && ringOpeningTimer <= ringCloseTime)
			{
				@float = Mathf.Lerp(0f, 1f, ringOpeningTimer / ringCloseTime);
				effectsData.ringEffectRenderer.get_material().SetFloat("_Range", @float);
				effectsData.ringClosingTimer += Time.get_deltaTime();
			}
			else
			{
				effectsData.openingTimer = 0f;
				effectsData.ringOpeningTimer = 0f;
				effectsData.ringClosingTimer = 0f;
				_openEffectNodeToRemoveList.Add(id);
			}
		}

		private void RemoveOpenedShields()
		{
			for (int i = 0; i < _openEffectNodeToRemoveList.get_Count(); i++)
			{
				_openEffectNodeList.Remove(_openEffectNodeToRemoveList.get_Item(i));
			}
			_openEffectNodeToRemoveList.FastClear();
		}

		private void UpdateNearToCloseEffect()
		{
			for (int i = 0; i < _nearToCloseEffectNodeList.get_Count(); i++)
			{
				int value = _nearToCloseEffectNodeList.get_Item(i);
				DiscShieldSettingsNode discShieldSettingsNode = default(DiscShieldSettingsNode);
				DiscShieldEffectsNode discShieldEffectsNode = default(DiscShieldEffectsNode);
				DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
				if (!_entityViewsesDb.TryQueryEntityView<DiscShieldEffectsNode>(value, ref discShieldEffectsNode) || !_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(value, ref discShieldAudioNode) || !_entityViewsesDb.TryQueryEntityView<DiscShieldSettingsNode>(value, ref discShieldSettingsNode))
				{
					continue;
				}
				IDiscShieldEffectsComponent effectsComponent = discShieldEffectsNode.effectsComponent;
				discShieldAudioNode.audioComponent.playGlowLoopSound.Dispatch(ref value);
				float discShieldLifeTime = discShieldSettingsNode.settingsComponent.discShieldLifeTime;
				float nearToCloseEffectStartTime = effectsComponent.nearToCloseEffectStartTime;
				Dispatcher<IDiscShieldAudioComponent, SoundParameterData> setGlowLoopSoundParameter = discShieldAudioNode.audioComponent.setGlowLoopSoundParameter;
				Material material = effectsComponent.discShieldRenderer.get_material();
				float nearToCloseEffectsTimer = effectsComponent.nearToCloseEffectsTimer;
				float @float = material.GetFloat("_unstable");
				if (@float < 1f)
				{
					if (nearToCloseEffectsTimer == 0f)
					{
						discShieldAudioNode.audioComponent.playGlowLoopSound.Dispatch(ref value);
					}
					@float = Mathf.Lerp(0f, 1f, nearToCloseEffectsTimer / (discShieldLifeTime - nearToCloseEffectStartTime));
					_soundParameterData.SetValues(value, @float);
					setGlowLoopSoundParameter.Dispatch(ref _soundParameterData);
					material.SetFloat("_unstable", @float);
					effectsComponent.nearToCloseEffectsTimer += Time.get_deltaTime();
				}
				else
				{
					discShieldAudioNode.audioComponent.stopGlowLoopSound.Dispatch(ref value);
					material.SetFloat("_unstable", 0f);
					effectsComponent.nearToCloseEffectsTimer = 0f;
					_nearToCloseEffectNodeToRemoveList.Add(value);
				}
			}
			RemoveShieldsFromNearCloseEffect();
		}

		private void RemoveShieldsFromNearCloseEffect()
		{
			for (int i = 0; i < _nearToCloseEffectNodeToRemoveList.get_Count(); i++)
			{
				_nearToCloseEffectNodeList.Remove(_nearToCloseEffectNodeToRemoveList.get_Item(i));
			}
			_nearToCloseEffectNodeToRemoveList.FastClear();
		}

		private void UpdateCloseEffect()
		{
			for (int i = 0; i < _closeEffectNodeList.get_Count(); i++)
			{
				int value = _closeEffectNodeList.get_Item(i);
				DiscShieldAudioNode discShieldAudioNode = default(DiscShieldAudioNode);
				DiscShieldEffectsNode discShieldEffectsNode = default(DiscShieldEffectsNode);
				if (!_entityViewsesDb.TryQueryEntityView<DiscShieldAudioNode>(value, ref discShieldAudioNode) || !_entityViewsesDb.TryQueryEntityView<DiscShieldEffectsNode>(value, ref discShieldEffectsNode))
				{
					continue;
				}
				IDiscShieldEffectsComponent effectsComponent = discShieldEffectsNode.effectsComponent;
				discShieldAudioNode.audioComponent.stopLoopSound.Dispatch(ref value);
				discShieldAudioNode.audioComponent.playOffSound.Dispatch(ref value);
				float closingTimer = effectsComponent.closingTimer;
				float closeTime = effectsComponent.closeTime;
				float @float = effectsComponent.discShieldRenderer.get_material().GetFloat("_activate");
				if (@float > 0f)
				{
					if (closingTimer == 0f)
					{
						discShieldAudioNode.audioComponent.stopLoopSound.Dispatch(ref value);
						discShieldAudioNode.audioComponent.playOffSound.Dispatch(ref value);
					}
					@float = Mathf.Lerp(1f, 0f, closingTimer / closeTime);
					effectsComponent.discShieldRenderer.get_material().SetFloat("_activate", @float);
					effectsComponent.closingTimer += Time.get_deltaTime();
				}
				else
				{
					effectsComponent.closingTimer = 0f;
					_closeEffectNodeToRemoveList.Add(value);
				}
			}
			RemoveClosedShields();
		}

		private void RemoveClosedShields()
		{
			for (int i = 0; i < _closeEffectNodeToRemoveList.get_Count(); i++)
			{
				_closeEffectNodeList.Remove(_closeEffectNodeToRemoveList.get_Item(i));
			}
			_closeEffectNodeToRemoveList.FastClear();
		}
	}
}
