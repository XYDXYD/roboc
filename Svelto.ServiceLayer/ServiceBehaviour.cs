using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Text;
using Utility;

namespace Svelto.ServiceLayer
{
	public class ServiceBehaviour
	{
		public Action Alternative;

		public Action MainBehaviour;

		public Exception exceptionThrown;

		private readonly int _autoAttempts;

		private readonly string _strErrorTitleKey;

		private readonly string _strErrorBodyKey;

		private int _autoRetryAttempt;

		private string _alternativeLabelKey;

		private string _mainLabelKey;

		private readonly StringBuilder _stringBuilder = new StringBuilder();

		public short errorCode
		{
			get;
			set;
		}

		public string errorTitle => StringTableBase<StringTable>.Instance.GetString(_strErrorTitleKey);

		public string errorBody
		{
			get
			{
				if (exceptionThrown is UserSuspendedException)
				{
					return exceptionThrown.Message;
				}
				_stringBuilder.Length = 0;
				_stringBuilder.AppendFormat("{0} \r\n", StringTableBase<StringTable>.Instance.GetString(_strErrorBodyKey));
				if (reasonID != null)
				{
					_stringBuilder.Append(StringTableBase<StringTable>.Instance.GetReplaceString("strReason", "[REASON]", StringTableBase<StringTable>.Instance.GetString(reasonID.ToString())));
					_stringBuilder.AppendFormat(" {0}", guidParameterCode);
				}
				else if (!string.IsNullOrEmpty(serverReason))
				{
					_stringBuilder.Append(StringTableBase<StringTable>.Instance.GetReplaceString("strReason", "[REASON]", StringTableBase<StringTable>.Instance.GetString(serverReason)));
				}
				return _stringBuilder.ToString();
			}
		}

		public string alternativeText
		{
			get
			{
				if (Alternative == null)
				{
					return null;
				}
				return StringTableBase<StringTable>.Instance.GetString(_alternativeLabelKey);
			}
		}

		public string mainText
		{
			get
			{
				return StringTableBase<StringTable>.Instance.GetString(_mainLabelKey);
			}
			set
			{
				_mainLabelKey = value;
			}
		}

		public Enum reasonID
		{
			private get;
			set;
		}

		public string guidParameterCode
		{
			private get;
			set;
		}

		public string serverReason
		{
			private get;
			set;
		}

		public ServiceBehaviour(string strErrorTitleKey, string strErrorBodyKey)
		{
			_autoAttempts = 3;
			reasonID = null;
			_strErrorTitleKey = strErrorTitleKey;
			_strErrorBodyKey = strErrorBodyKey;
			_mainLabelKey = "strRetry";
			Reset();
		}

		public ServiceBehaviour(string strErrorTitleKey, string strErrorBodyKey, int autoAttempts)
		{
			_autoAttempts = autoAttempts;
			reasonID = null;
			_strErrorTitleKey = strErrorTitleKey;
			_strErrorBodyKey = strErrorBodyKey;
			_mainLabelKey = "strRetry";
			Reset();
		}

		public ServiceBehaviour(string strErrorTitleKey, string strErrorBodyKey, string mainButtonText, int autoAttempts)
		{
			_autoAttempts = autoAttempts;
			reasonID = null;
			_strErrorTitleKey = strErrorTitleKey;
			_strErrorBodyKey = strErrorBodyKey;
			_mainLabelKey = mainButtonText;
			Reset();
		}

		public void SetAlternativeBehaviour(Action alternativeAction, string alternativeLabelKey)
		{
			Alternative = alternativeAction;
			_alternativeLabelKey = alternativeLabelKey;
		}

		public void RemoveAlternativeBehaviour()
		{
			_alternativeLabelKey = null;
			Alternative = null;
		}

		public void Failed(Exception e, IServiceFailedAnswer answer, bool retry)
		{
			exceptionThrown = e;
			if (retry && _autoRetryAttempt > 0)
			{
				AutoRetryOrFail(answer);
			}
			else if (answer != null && answer.failed != null)
			{
				answer.failed(this);
			}
		}

		private void Reset()
		{
			_autoRetryAttempt = _autoAttempts;
		}

		private void AutoRetryOrFail(IServiceFailedAnswer answer)
		{
			Console.LogWarning(FastConcatUtility.FastConcat<string>("Auto retry attempt left - ", _autoRetryAttempt.ToString()));
			if (exceptionThrown != null)
			{
				Console.LogError("Error was: " + exceptionThrown.ToString());
			}
			if (--_autoRetryAttempt <= 0)
			{
				if (answer != null && answer.failed != null)
				{
					answer.failed(this);
				}
				Reset();
			}
			else
			{
				TaskRunner.get_Instance().Run(TimeOut());
			}
		}

		private IEnumerator TimeOut()
		{
			yield return (object)new WaitForSecondsEnumerator(0.5f);
			MainBehaviour();
		}
	}
}
