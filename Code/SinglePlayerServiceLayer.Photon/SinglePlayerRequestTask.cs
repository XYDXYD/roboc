using Svelto.Tasks;
using System;

namespace SinglePlayerServiceLayer.Photon
{
	internal abstract class SinglePlayerRequestTask<TData> : SinglePlayerRequest<TData>, ITask, IAbstractTask
	{
		public bool isDone
		{
			get;
			private set;
		}

		public float progress
		{
			get;
			private set;
		}

		private event Action<bool> _onComplete;

		public SinglePlayerRequestTask(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
			: base(strErrorTitleKey, strErrorBodyKey, autoAttempts)
		{
		}

		public IAbstractTask OnComplete(Action<bool> action)
		{
			_onComplete += action;
			return this;
		}

		protected override void OnSuccess()
		{
			base.OnSuccess();
			TaskComplete();
		}

		protected override void OnFailed(Exception exception)
		{
			base.OnFailed(exception);
			TaskComplete(result: false);
		}

		protected void TaskComplete(bool result = true)
		{
			isDone = true;
			progress = 1f;
			if (this._onComplete != null)
			{
				this._onComplete(result);
			}
		}
	}
}
