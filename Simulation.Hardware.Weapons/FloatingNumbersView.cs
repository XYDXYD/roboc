using Svelto.IoC;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class FloatingNumbersView : MonoBehaviour, IInitialize
	{
		[SerializeField]
		private GameObject _prefab;

		[SerializeField]
		private Color _damageColor = Color.get_white();

		[SerializeField]
		private Color _healingColor = Color.get_green();

		[SerializeField]
		private string _critAudioEvent;

		public const string NORMAL_ANIMATION = "FloatingNumber_Normal";

		public const string CRITICAL_ANIMATION = "FloatingNumber_Critical";

		[Inject]
		public FloatingNumbersController floatingNumbersController
		{
			private get;
			set;
		}

		public GameObject prefab => _prefab;

		public Color damageColor => _damageColor;

		public Color healingColor => _healingColor;

		public Transform viewTransform
		{
			get;
			private set;
		}

		public string critAudioEvent => _critAudioEvent;

		public FloatingNumbersView()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)


		public void OnDependenciesInjected()
		{
			viewTransform = this.get_transform();
			floatingNumbersController.SetView(this);
		}
	}
}
