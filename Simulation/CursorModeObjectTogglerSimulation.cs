using Svelto.ES.Legacy;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class CursorModeObjectTogglerSimulation : MonoBehaviour, IInitialize, IAlignmentRectifierPausable, IComponent
	{
		private bool _pausedFunctionalCubes;

		[Inject]
		internal ICursorMode cursorMode
		{
			private get;
			set;
		}

		public CursorModeObjectTogglerSimulation()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			cursorMode.OnSwitch += UpdateState;
		}

		public void AlignmentRectifierStarted()
		{
			_pausedFunctionalCubes = true;
			UpdateState(cursorMode.currentMode);
		}

		public void AlignmentRectifierEnded()
		{
			_pausedFunctionalCubes = false;
			UpdateState(cursorMode.currentMode);
		}

		private void Start()
		{
			if (cursorMode.currentMode == Mode.Free)
			{
				this.get_gameObject().SetActive(false);
			}
		}

		private void OnDestroy()
		{
			cursorMode.OnSwitch -= UpdateState;
		}

		private void UpdateState(Mode cursorMode)
		{
			if (!_pausedFunctionalCubes)
			{
				if (cursorMode == Mode.Lock)
				{
					this.get_gameObject().SetActive(true);
				}
				else
				{
					this.get_gameObject().SetActive(false);
				}
			}
			else
			{
				this.get_gameObject().SetActive(!_pausedFunctionalCubes);
			}
		}
	}
}
