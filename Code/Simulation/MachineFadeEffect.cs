using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xft;

namespace Simulation
{
	internal sealed class MachineFadeEffect : IInitialize, IWaitForFrameworkDestruction
	{
		private enum FadeState
		{
			VisibleToWhite,
			WhiteToInvisible,
			InvisibleToWhite,
			WhiteToVisible,
			None
		}

		private Dictionary<int, Dictionary<Renderer, Material[]>> _machinesOriginalMaterials = new Dictionary<int, Dictionary<Renderer, Material[]>>();

		private Dictionary<int, Dictionary<Renderer, Material[]>> _machinesClonedMaterials = new Dictionary<int, Dictionary<Renderer, Material[]>>();

		private Dictionary<int, ParticleSystemRenderer[]> _machinesParticleSystems = new Dictionary<int, ParticleSystemRenderer[]>();

		private Dictionary<int, List<TrailRenderer>> _machinesTrail = new Dictionary<int, List<TrailRenderer>>();

		private Dictionary<int, XWeaponTrail[]> _machinesFlags = new Dictionary<int, XWeaponTrail[]>();

		private Dictionary<int, FadeState> _machineFadeState = new Dictionary<int, FadeState>();

		private Dictionary<int, float> _machineTimer = new Dictionary<int, float>();

		private Dictionary<int, UILabel[]> _machinesLabels = new Dictionary<int, UILabel[]>();

		private Dictionary<int, HeadLightSwitch[]> _machinesLights = new Dictionary<int, HeadLightSwitch[]>();

		private MaterialPropertyBlock _materialProperty = new MaterialPropertyBlock();

		[Inject]
		internal MachinePreloader machinePreloader
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal MachineRootContainer rootContainer
		{
			private get;
			set;
		}

		[Inject]
		internal LivePlayersContainer livePlayersContainer
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnMachineDestroyed += HandleOnMachineDestroyed;
			spawnDispatcher.OnPlayerUnregistered += HandleOnPlayerUnregistered;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineDestroyed -= HandleOnMachineDestroyed;
			spawnDispatcher.OnPlayerUnregistered -= HandleOnPlayerUnregistered;
		}

		private void HandleOnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			ResetState(machineId);
		}

		internal void Register(int machineId)
		{
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Expected O, but got Unknown
			if (_machinesOriginalMaterials.ContainsKey(machineId))
			{
				return;
			}
			PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(machineId);
			FasterList<Renderer> allRenderers = preloadedMachine.allRenderers;
			Dictionary<Renderer, Material[]> dictionary = new Dictionary<Renderer, Material[]>();
			Dictionary<Renderer, Material[]> dictionary2 = new Dictionary<Renderer, Material[]>();
			_machinesOriginalMaterials.Add(machineId, dictionary);
			_machinesClonedMaterials.Add(machineId, dictionary2);
			for (int i = 0; i < allRenderers.get_Count(); i++)
			{
				Renderer val = allRenderers.get_Item(i);
				if (val is MeshRenderer || val is SkinnedMeshRenderer)
				{
					Material[] sharedMaterials = allRenderers.get_Item(i).get_sharedMaterials();
					Material[] array = (Material[])new Material[sharedMaterials.Length];
					for (int j = 0; j < sharedMaterials.Length; j++)
					{
						Material val2 = array[j] = new Material(sharedMaterials[j]);
					}
					dictionary[val] = sharedMaterials;
					dictionary2[val] = array;
				}
			}
			GameObject machineBoard = preloadedMachine.machineBoard;
			_machinesFlags[machineId] = machineBoard.GetComponentsInChildren<XWeaponTrail>(true);
			_machinesLabels[machineId] = machineBoard.GetComponentsInChildren<UILabel>(true);
			_machinesLights[machineId] = machineBoard.GetComponentsInChildren<HeadLightSwitch>(true);
			_machineFadeState[machineId] = FadeState.None;
			_machineTimer[machineId] = 0f;
		}

		private void HandleOnPlayerUnregistered(UnregisterParametersPlayer unregisterParameters)
		{
			if (_machinesOriginalMaterials.ContainsKey(unregisterParameters.machineId))
			{
				_machinesFlags.Remove(unregisterParameters.machineId);
				_machinesLabels.Remove(unregisterParameters.machineId);
				_machinesLights.Remove(unregisterParameters.machineId);
				_machineFadeState.Remove(unregisterParameters.machineId);
				_machineTimer.Remove(unregisterParameters.machineId);
				_machinesOriginalMaterials.Remove(unregisterParameters.machineId);
				_machinesClonedMaterials.Remove(unregisterParameters.machineId);
				_machinesTrail.Remove(unregisterParameters.machineId);
				_machinesParticleSystems.Remove(unregisterParameters.machineId);
			}
		}

		internal void Unregister(int machineId)
		{
			if (_machinesOriginalMaterials.ContainsKey(machineId))
			{
				ResetState(machineId);
				_machinesOriginalMaterials.Remove(machineId);
				_machinesClonedMaterials.Remove(machineId);
				_machinesParticleSystems.Remove(machineId);
				_machinesTrail.Remove(machineId);
				_machinesFlags.Remove(machineId);
				_machinesLabels.Remove(machineId);
				_machinesLights.Remove(machineId);
			}
		}

		private void ResetState(int machineId)
		{
			if (!_machinesOriginalMaterials.ContainsKey(machineId))
			{
				return;
			}
			FadeState fadeState = _machineFadeState[machineId];
			if (fadeState != FadeState.None)
			{
				ParticleSystemRenderer[] array = _machinesParticleSystems[machineId];
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i].set_enabled(true);
					}
				}
				XWeaponTrail[] array2 = _machinesFlags[machineId];
				List<TrailRenderer> list = _machinesTrail[machineId];
				UILabel[] array3 = _machinesLabels[machineId];
				HeadLightSwitch[] array4 = _machinesLights[machineId];
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						list[j].set_enabled(true);
					}
				}
				if (array3 != null)
				{
					for (int k = 0; k < array3.Length; k++)
					{
						array3[k].set_enabled(true);
					}
				}
				if (array4 != null)
				{
					for (int l = 0; l < array4.Length; l++)
					{
						array4[l].set_enabled(true);
					}
				}
				if (array2 != null)
				{
					for (int m = 0; m < array2.Length; m++)
					{
						array2[m].set_enabled(true);
					}
				}
				if (fadeState == FadeState.WhiteToInvisible || fadeState == FadeState.InvisibleToWhite)
				{
					Dictionary<Renderer, Material[]> dictionary = _machinesOriginalMaterials[machineId];
					Dictionary<Renderer, Material[]>.Enumerator enumerator = dictionary.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Renderer key = enumerator.Current.Key;
						Material[] value = enumerator.Current.Value;
						key.set_sharedMaterials(value);
						key.GetPropertyBlock(_materialProperty);
						_materialProperty.SetFloat("_WhiteFade", 0f);
						key.SetPropertyBlock(_materialProperty);
					}
				}
			}
			_machineFadeState[machineId] = FadeState.None;
			_machineTimer[machineId] = 0f;
		}

		private IEnumerator FadeToInvisible(float duration1, float duration2, float initialDuration, int machineId, Action<Material, int> onSwap)
		{
			Dictionary<Renderer, Material[]> originalMaterials = _machinesOriginalMaterials[machineId];
			Dictionary<Renderer, Material[]> clonedMaterials = _machinesClonedMaterials[machineId];
			ParticleSystemRenderer[] particleSystems = _machinesParticleSystems[machineId];
			XWeaponTrail[] flags = _machinesFlags[machineId];
			List<TrailRenderer> trails = _machinesTrail[machineId];
			UILabel[] labels = _machinesLabels[machineId];
			HeadLightSwitch[] lights = _machinesLights[machineId];
			int playerId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
			if (trails != null)
			{
				for (int i = 0; i < trails.Count; i++)
				{
					trails[i].set_enabled(false);
				}
			}
			if (particleSystems != null)
			{
				for (int j = 0; j < particleSystems.Length; j++)
				{
					particleSystems[j].set_enabled(false);
				}
			}
			if (flags != null)
			{
				for (int k = 0; k < flags.Length; k++)
				{
					flags[k].set_enabled(false);
				}
			}
			if (labels != null)
			{
				for (int l = 0; l < labels.Length; l++)
				{
					labels[l].set_enabled(false);
				}
			}
			if (lights != null)
			{
				for (int m = 0; m < lights.Length; m++)
				{
					lights[m].set_enabled(false);
				}
			}
			float timer2 = duration1;
			while (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId))
			{
				float num;
				timer2 = (num = timer2 - Time.get_deltaTime());
				if (!(num > 0f) || _machineFadeState[machineId] != 0)
				{
					break;
				}
				_machineTimer[machineId] = timer2;
				float lerpValue2 = 1f - timer2 / initialDuration;
				Dictionary<Renderer, Material[]>.Enumerator enumerator = originalMaterials.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Renderer key = enumerator.Current.Key;
					key.GetPropertyBlock(_materialProperty);
					_materialProperty.SetFloat("_WhiteFade", lerpValue2);
					key.SetPropertyBlock(_materialProperty);
				}
				yield return null;
			}
			if (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId) && _machineFadeState[machineId] == FadeState.VisibleToWhite)
			{
				Dictionary<Renderer, Material[]>.Enumerator enumerator2 = clonedMaterials.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Renderer key2 = enumerator2.Current.Key;
					Material[] value = enumerator2.Current.Value;
					for (int n = 0; n < value.Length; n++)
					{
						onSwap(value[n], machineId);
					}
					key2.set_sharedMaterials(value);
				}
				_machineFadeState[machineId] = FadeState.WhiteToInvisible;
			}
			if (!livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId) || _machineFadeState[machineId] != FadeState.WhiteToInvisible)
			{
				yield break;
			}
			timer2 = duration2;
			do
			{
				timer2 -= Time.get_deltaTime();
				_machineTimer[machineId] = timer2;
				float lerpValue = Mathf.Clamp01(1f - timer2 / initialDuration);
				Dictionary<Renderer, Material[]>.Enumerator enumerator3 = clonedMaterials.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					Renderer key3 = enumerator3.Current.Key;
					key3.GetPropertyBlock(_materialProperty);
					_materialProperty.SetFloat("_WhiteFade", lerpValue);
					key3.SetPropertyBlock(_materialProperty);
				}
				yield return null;
			}
			while (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId) && timer2 > 0f && _machineFadeState[machineId] == FadeState.WhiteToInvisible);
		}

		private IEnumerator FadeToVisible(float duration1, float duration2, float initialDuration, int machineId)
		{
			Dictionary<Renderer, Material[]> originalMaterials = _machinesOriginalMaterials[machineId];
			Dictionary<Renderer, Material[]> clonedMaterials = _machinesClonedMaterials[machineId];
			ParticleSystemRenderer[] particleSystems = _machinesParticleSystems[machineId];
			int playerId = playerMachinesContainer.GetPlayerFromMachineId(TargetType.Player, machineId);
			float timer2 = duration1;
			while (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId))
			{
				float num;
				timer2 = (num = timer2 - Time.get_deltaTime());
				if (!(num > 0f) || _machineFadeState[machineId] != FadeState.InvisibleToWhite)
				{
					break;
				}
				_machineTimer[machineId] = timer2;
				Dictionary<Renderer, Material[]>.Enumerator enumerator = clonedMaterials.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Renderer key = enumerator.Current.Key;
					key.GetPropertyBlock(_materialProperty);
					_materialProperty.SetFloat("_WhiteFade", timer2 / initialDuration);
					key.SetPropertyBlock(_materialProperty);
				}
				yield return null;
			}
			if (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId) && _machineFadeState[machineId] == FadeState.InvisibleToWhite)
			{
				Dictionary<Renderer, Material[]>.Enumerator enumerator2 = originalMaterials.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Renderer key2 = enumerator2.Current.Key;
					Material[] value = enumerator2.Current.Value;
					key2.set_sharedMaterials(value);
				}
				XWeaponTrail[] array = _machinesFlags[machineId];
				if (array != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						array[i].set_enabled(true);
					}
				}
				UILabel[] array2 = _machinesLabels[machineId];
				if (array2 != null)
				{
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j].set_enabled(true);
					}
				}
				HeadLightSwitch[] array3 = _machinesLights[machineId];
				if (array3 != null)
				{
					for (int k = 0; k < array3.Length; k++)
					{
						array3[k].set_enabled(true);
					}
				}
				if (particleSystems != null)
				{
					for (int l = 0; l < particleSystems.Length; l++)
					{
						particleSystems[l].set_enabled(true);
					}
				}
				List<TrailRenderer> list = _machinesTrail[machineId];
				if (list != null)
				{
					for (int m = 0; m < list.Count; m++)
					{
						list[m].Clear();
						list[m].set_enabled(true);
					}
				}
				_machineFadeState[machineId] = FadeState.WhiteToVisible;
			}
			if (!livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId) || _machineFadeState[machineId] != FadeState.WhiteToVisible)
			{
				yield break;
			}
			timer2 = duration2;
			do
			{
				timer2 -= Time.get_deltaTime();
				_machineTimer[machineId] = timer2;
				float lerpValue = Mathf.Clamp01(timer2 / initialDuration);
				Dictionary<Renderer, Material[]>.Enumerator enumerator3 = originalMaterials.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					Renderer key3 = enumerator3.Current.Key;
					key3.GetPropertyBlock(_materialProperty);
					_materialProperty.SetFloat("_WhiteFade", lerpValue);
					key3.SetPropertyBlock(_materialProperty);
				}
				yield return null;
			}
			while (livePlayersContainer.IsPlayerAlive(TargetType.Player, playerId) && timer2 > 0f && _machineFadeState[machineId] == FadeState.WhiteToVisible);
			if (_machineFadeState[machineId] == FadeState.WhiteToVisible)
			{
				_machineFadeState[machineId] = FadeState.None;
			}
		}

		internal void ToInvisible(float duration, int machineId, Action<Material, int> OnSwap)
		{
			FadeState fadeState = _machineFadeState[machineId];
			if (fadeState == FadeState.VisibleToWhite || fadeState == FadeState.WhiteToInvisible)
			{
				return;
			}
			float num = duration * 0.5f;
			float num2 = duration * 0.5f;
			if (fadeState == FadeState.WhiteToVisible || fadeState == FadeState.None)
			{
				_machineFadeState[machineId] = FadeState.VisibleToWhite;
				num -= _machineTimer[machineId];
			}
			else
			{
				num = 0f;
				num2 -= _machineTimer[machineId];
				_machineFadeState[machineId] = FadeState.WhiteToInvisible;
			}
			GameObject machineRoot = rootContainer.GetMachineRoot(TargetType.Player, machineId);
			ParticleSystemRenderer[] componentsInChildren = machineRoot.GetComponentsInChildren<ParticleSystemRenderer>(true);
			List<TrailRenderer> list = new List<TrailRenderer>();
			TrailRenderer[] componentsInChildren2 = machineRoot.GetComponentsInChildren<TrailRenderer>();
			if (componentsInChildren2 != null)
			{
				for (int i = 0; i < componentsInChildren2.Length; i++)
				{
					if (componentsInChildren2[i].get_enabled())
					{
						list.Add(componentsInChildren2[i]);
					}
				}
			}
			_machinesTrail[machineId] = list;
			_machinesParticleSystems[machineId] = componentsInChildren;
			TaskRunner.get_Instance().Run(FadeToInvisible(num, num2, duration * 0.5f, machineId, OnSwap));
		}

		internal void ToVisible(float duration, int machineId)
		{
			float num = duration * 0.5f;
			float num2 = duration * 0.5f;
			FadeState fadeState = _machineFadeState[machineId];
			if (fadeState == FadeState.VisibleToWhite || fadeState == FadeState.WhiteToInvisible)
			{
				if (fadeState == FadeState.WhiteToInvisible)
				{
					_machineFadeState[machineId] = FadeState.InvisibleToWhite;
					num -= _machineTimer[machineId];
				}
				else
				{
					num = 0f;
					num2 -= _machineTimer[machineId];
					_machineFadeState[machineId] = FadeState.WhiteToVisible;
				}
				TaskRunner.get_Instance().Run(FadeToVisible(num, num2, duration * 0.5f, machineId));
			}
		}
	}
}
