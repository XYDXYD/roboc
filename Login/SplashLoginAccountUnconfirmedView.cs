using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Login
{
	internal class SplashLoginAccountUnconfirmedView : MonoBehaviour, ISplashLoginDialogView
	{
		private SplashLoginAccountUnconfirmedController _splashLoginAccountUnconfirmedController;

		public SplashLoginAccountUnconfirmedView()
			: this()
		{
		}

		public void InjectController(ISplashLoginDialogController splashLoginDialogController)
		{
			_splashLoginAccountUnconfirmedController = (SplashLoginAccountUnconfirmedController)splashLoginDialogController;
		}

		public void DestroySelf()
		{
			Object.Destroy(this.get_gameObject());
		}

		internal void RebuildSignalChain()
		{
			GameObject gameObject = this.get_gameObject();
			while (gameObject.get_transform().get_parent() != null)
			{
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
				if (gameObject.GetComponent<IChainRoot>() != null)
				{
					break;
				}
			}
		}
	}
}
