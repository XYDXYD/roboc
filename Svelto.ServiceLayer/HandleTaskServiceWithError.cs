using Svelto.Tasks;
using System;
using System.Collections;

namespace Svelto.ServiceLayer
{
	internal class HandleTaskServiceWithError : IEnumerable
	{
		private enum InteractionState
		{
			Waiting,
			RetryAction,
			AlternativeAction
		}

		private TaskService _serviceTask;

		private Action _onExitErrorState;

		private Action _onEntryErrorState;

		public HandleTaskServiceWithError(TaskService serviceTask, Action onExitErrorState, Action onEntryErrorState)
		{
			_serviceTask = serviceTask;
			_onExitErrorState = onExitErrorState;
			_onEntryErrorState = onEntryErrorState;
		}

		public IEnumerator GetEnumerator()
		{
			TaskWrapper taskWrapper = new TaskWrapper(_serviceTask);
			yield return taskWrapper;
			while (!_serviceTask.succeeded)
			{
				ServiceBehaviour behaviour = _serviceTask.behaviour;
				InteractionState interactionState = InteractionState.Waiting;
				if (_onEntryErrorState != null)
				{
					_onEntryErrorState();
				}
				ErrorWindow.ShowErrorWindow(new GenericErrorData(behaviour.errorTitle, behaviour.errorBody, Localization.Get("strRetry", true), behaviour.alternativeText, delegate
				{
					interactionState = InteractionState.RetryAction;
				}, delegate
				{
					if (behaviour.Alternative != null)
					{
						behaviour.Alternative();
					}
					interactionState = InteractionState.AlternativeAction;
				}), (behaviour.exceptionThrown == null) ? null : behaviour.exceptionThrown.ToString());
				while (interactionState == InteractionState.Waiting)
				{
					yield return null;
				}
				if (_onExitErrorState != null)
				{
					_onExitErrorState();
				}
				if (interactionState == InteractionState.RetryAction)
				{
					yield return taskWrapper;
					continue;
				}
				break;
			}
		}
	}
}
