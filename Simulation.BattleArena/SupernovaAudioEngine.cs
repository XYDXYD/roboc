using Fabric;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation.BattleArena
{
	internal sealed class SupernovaAudioEngine : IEngine, IInitialize, IWaitForFrameworkDestruction
	{
		private FasterList<ISupernovaAudioGameComponentStop> supernovaAudioGCStopArray = new FasterList<ISupernovaAudioGameComponentStop>();

		[Inject]
		internal SupernovaAudioObserver supernovaAudioObserver
		{
			private get;
			set;
		}

		[Inject]
		internal VOManager voManager
		{
			private get;
			set;
		}

		public Type[] AcceptedComponents()
		{
			return new Type[1]
			{
				typeof(ISupernovaAudioGameComponentStop)
			};
		}

		public void Add(IComponent component)
		{
			if (component is ISupernovaAudioGameComponentStop)
			{
				supernovaAudioGCStopArray.Add(component as ISupernovaAudioGameComponentStop);
			}
		}

		public void Remove(IComponent component)
		{
			if (component is ISupernovaAudioGameComponentStop)
			{
				supernovaAudioGCStopArray.Remove(component as ISupernovaAudioGameComponentStop);
			}
		}

		void IInitialize.OnDependenciesInjected()
		{
			supernovaAudioObserver.OnSupernovaPlay += OnSupernovaPlay;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			supernovaAudioObserver.OnSupernovaPlay -= OnSupernovaPlay;
		}

		private void OnSupernovaPlay(GameObject supernovaGO)
		{
			for (int i = 0; i < supernovaAudioGCStopArray.get_Count(); i++)
			{
				supernovaAudioGCStopArray.get_Item(i).StopAudio();
			}
			voManager.StopAll();
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Supernova), 0, (object)null, supernovaGO);
		}
	}
}
