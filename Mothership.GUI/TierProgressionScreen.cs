using Svelto.ECS;
using System;
using UnityEngine;

namespace Mothership.GUI
{
	internal class TierProgressionScreen : MonoBehaviour
	{
		public GameObject tierWidgetTemplate;

		public UITable layoutTable;

		public DispatchOnChange<bool> isShown;

		public UIButton backButton;

		public TierProgressionScreen()
			: this()
		{
		}

		public void Initialize(int id)
		{
			isShown = new DispatchOnChange<bool>(id);
			isShown.NotifyOnValueSet((Action<int, bool>)OnShow);
		}

		private void OnShow(int id, bool show)
		{
			this.get_gameObject().SetActive(show);
		}
	}
}
