using Svelto.DataStructures;
using System;
using UnityEngine;

namespace Svelto.Context.Legacy
{
	public class NotifyComponentsRemoved : MonoBehaviour
	{
		public WeakReference<IUnityContextHierarchyChangedListener> unityContext
		{
			private get;
			set;
		}

		public NotifyComponentsRemoved()
			: this()
		{
		}

		private void Start()
		{
			if (unityContext == null)
			{
				Object.Destroy(this);
			}
		}

		private void OnDestroy()
		{
			if (unityContext == null || !((WeakReference)unityContext).IsAlive)
			{
				return;
			}
			MonoBehaviour[] components = this.get_gameObject().GetComponents<MonoBehaviour>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i] != null)
				{
					unityContext.get_Target().OnMonobehaviourRemoved(components[i]);
				}
			}
		}
	}
}
