using Svelto.DataStructures;
using Svelto.Factories;
using System;
using UnityEngine;

namespace Svelto.Context.Legacy
{
	public class MonoBehaviourFactory : IMonoBehaviourFactory
	{
		private WeakReference<IUnityContextHierarchyChangedListener> _unityContext;

		public MonoBehaviourFactory(IUnityContextHierarchyChangedListener unityContext)
		{
			_unityContext = new WeakReference<IUnityContextHierarchyChangedListener>(unityContext);
		}

		public M Build<M>(Func<M> constructor) where M : MonoBehaviour
		{
			M val = constructor();
			_unityContext.get_Target().OnMonobehaviourAdded((object)val);
			GameObject gameObject = val.get_gameObject();
			if (gameObject.GetComponent<NotifyComponentsRemoved>() == null)
			{
				gameObject.AddComponent<NotifyComponentsRemoved>().unityContext = _unityContext;
			}
			return val;
		}
	}
}
