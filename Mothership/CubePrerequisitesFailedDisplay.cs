using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Mothership
{
	internal class CubePrerequisitesFailedDisplay : MonoBehaviour, IInitialize
	{
		[SerializeField]
		private UILabel bodyLabel;

		[Inject]
		internal CubePrerequisitesFailedObserver cubePrerequisitesFailedObserver
		{
			private get;
			set;
		}

		public CubePrerequisitesFailedDisplay()
			: this()
		{
		}

		public unsafe void OnDependenciesInjected()
		{
			cubePrerequisitesFailedObserver.AddAction(new ObserverAction<string>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void OnDestroy()
		{
			cubePrerequisitesFailedObserver.RemoveAction(new ObserverAction<string>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void OkClicked()
		{
			this.get_gameObject().SetActive(false);
		}

		private void HandleCubePrerequisitesFailed(ref string reason)
		{
			if (this.get_transform().get_parent().get_gameObject()
				.get_activeInHierarchy())
			{
				bodyLabel.set_text(reason);
				this.get_gameObject().SetActive(true);
			}
		}
	}
}
