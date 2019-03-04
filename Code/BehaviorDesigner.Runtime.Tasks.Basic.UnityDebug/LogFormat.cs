using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityDebug
{
	[TaskDescription("LogFormat is analgous to Debug.LogFormat().\nIt takes format string, substitutes arguments supplied a '{0-4}' and returns success.\nAny fields or arguments not supplied are ignored.It can be used for debugging.")]
	[TaskIcon("{SkinColor}LogIcon.png")]
	public class LogFormat : Action
	{
		[Tooltip("Text format with {0}, {1}, etc")]
		public SharedString textFormat;

		[Tooltip("Is this text an error?")]
		public SharedBool logError;

		public SharedVariable arg0;

		public SharedVariable arg1;

		public SharedVariable arg2;

		public SharedVariable arg3;

		public LogFormat()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			object[] array = buildParamsArray();
			if (logError.get_Value())
			{
				Debug.LogErrorFormat(textFormat.get_Value(), array);
			}
			else
			{
				Debug.LogFormat(textFormat.get_Value(), array);
			}
			return 2;
		}

		private object[] buildParamsArray()
		{
			object[] array;
			if (isValid(arg3))
			{
				array = new object[4];
				array[3] = arg3.GetValue();
				array[2] = arg2.GetValue();
				array[1] = arg1.GetValue();
				array[0] = arg0.GetValue();
			}
			else if (isValid(arg2))
			{
				array = new object[3];
				array[2] = arg2.GetValue();
				array[1] = arg1.GetValue();
				array[0] = arg0.GetValue();
			}
			else if (isValid(arg1))
			{
				array = new object[2];
				array[1] = arg1.GetValue();
				array[0] = arg0.GetValue();
			}
			else
			{
				if (!isValid(arg0))
				{
					return null;
				}
				array = new object[1]
				{
					arg0.GetValue()
				};
			}
			return array;
		}

		private bool isValid(SharedVariable sv)
		{
			return sv != null && !sv.get_IsNone();
		}

		public override void OnReset()
		{
			textFormat = string.Empty;
			logError = false;
			arg0 = null;
			arg1 = null;
			arg2 = null;
			arg3 = null;
		}
	}
}