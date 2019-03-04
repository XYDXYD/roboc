using System;

namespace PlayMaker
{
	public class PlayMakerDataRequestBase<INPUTTYPE> : IPlayMakerDataRequest where INPUTTYPE : IPlaymakerRequestInputParameters
	{
		private INPUTTYPE _inputParams;

		private Action _resultCallback;

		public PlayMakerDataRequestBase(INPUTTYPE inputParams, Action resultCallback)
		{
			_inputParams = inputParams;
			_resultCallback = resultCallback;
		}

		void IPlayMakerDataRequest.AssignResults<T>(T resultsObj)
		{
		}

		void IPlayMakerDataRequest.Execute()
		{
			if (_resultCallback != null)
			{
				_resultCallback();
			}
		}

		IPlaymakerRequestInputParameters IPlayMakerDataRequest.GetInputParameters()
		{
			IPlaymakerRequestInputParameters playmakerRequestInputParameters = _inputParams;
			return _inputParams;
		}

		T IPlayMakerDataRequest.GetInputParameters<T>()
		{
			return _inputParams as T;
		}
	}
	public class PlayMakerDataRequestBase<INPUTTYPE, RETURNTYPE> : IPlayMakerDataRequest where INPUTTYPE : IPlaymakerRequestInputParameters where RETURNTYPE : class, IPlaymakerRequestReturnResults, new()
	{
		private INPUTTYPE _inputParams;

		private RETURNTYPE _returnData;

		private Action<RETURNTYPE> _resultCallback;

		public PlayMakerDataRequestBase(INPUTTYPE inputParams, Action<RETURNTYPE> resultCallback)
		{
			_inputParams = inputParams;
			_returnData = new RETURNTYPE();
			_returnData.SetDefaultReturnResults();
			_resultCallback = resultCallback;
		}

		void IPlayMakerDataRequest.AssignResults<T>(T resultsObj)
		{
			RETURNTYPE val = _returnData = (resultsObj as RETURNTYPE);
		}

		void IPlayMakerDataRequest.Execute()
		{
			if (_resultCallback != null)
			{
				_resultCallback(_returnData);
			}
		}

		IPlaymakerRequestInputParameters IPlayMakerDataRequest.GetInputParameters()
		{
			IPlaymakerRequestInputParameters playmakerRequestInputParameters = _inputParams;
			return _inputParams;
		}

		T IPlayMakerDataRequest.GetInputParameters<T>()
		{
			return _inputParams as T;
		}
	}
}
