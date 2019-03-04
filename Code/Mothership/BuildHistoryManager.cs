using Fabric;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

namespace Mothership
{
	internal class BuildHistoryManager : IInitialize, IWaitForFrameworkDestruction, IHandleEditingInput, IInputComponent, IComponent
	{
		private const int MAX_HISTORY_SIZE = 2000;

		private const string AUDIO_UNDO = "BUILD_Undo";

		private FasterList<Action> _fasterList;

		private int _undoActionIndex;

		[Inject]
		public IGUIInputControllerMothership guiInputControllerMothership
		{
			private get;
			set;
		}

		internal bool lockInput
		{
			get;
			set;
		}

		public BuildHistoryManager()
		{
			_undoActionIndex = -1;
			_fasterList = new FasterList<Action>(2000);
		}

		public void OnDependenciesInjected()
		{
			guiInputControllerMothership.OnScreenStateChange += ClearBuildHistory;
		}

		public void OnFrameworkDestroyed()
		{
			guiInputControllerMothership.OnScreenStateChange -= ClearBuildHistory;
			ClearBuildHistory();
		}

		public void HandleEditingInput(InputEditingData inputEditingData)
		{
			if (guiInputControllerMothership.GetActiveScreen() == GuiScreens.BuildMode && inputEditingData[EditingInputAxis.UNDO_LAST_ACTION] == 1f)
			{
				Undo();
			}
		}

		internal void StoreUndoBuildAction(Action action)
		{
			if (_fasterList.get_Count() > _undoActionIndex + 1)
			{
				int num = _fasterList.get_Count() - 1;
				for (int num2 = num; num2 >= _undoActionIndex + 1; num2--)
				{
					_fasterList.RemoveAt(num2);
				}
				if (_fasterList.get_Count() <= 0)
				{
					_fasterList.Clear();
				}
			}
			_fasterList.Add(action);
			_undoActionIndex++;
			if (_fasterList.get_Count() > 2000)
			{
				_fasterList.RemoveAt(0);
				_undoActionIndex--;
			}
		}

		internal void ActuallyClearBuildHistory()
		{
			_undoActionIndex = -1;
			_fasterList.Clear();
		}

		private void Undo()
		{
			if (_undoActionIndex <= -1)
			{
				lockInput = false;
				return;
			}
			EventManager.get_Instance().PostEvent("BUILD_Undo", 0);
			Action action = _fasterList.get_Item(_undoActionIndex);
			action();
			_undoActionIndex--;
			lockInput = false;
		}

		private void ClearBuildHistory()
		{
			if (!WorldSwitching.IsInBuildMode())
			{
				ActuallyClearBuildHistory();
			}
		}
	}
}
