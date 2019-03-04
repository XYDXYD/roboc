using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Log is a simple task which will output the specified text and return success. It can be used for debugging.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=16")]
	[TaskIcon("{SkinColor}LogIcon.png")]
	public class Log : Action
	{
		[Tooltip("Text to output to the log")]
		public SharedString text;

		[Tooltip("Is this text an error?")]
		public SharedBool logError;

		public Log()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (logError.get_Value())
			{
				Debug.LogError((object)text);
			}
			else
			{
				Debug.Log((object)text);
			}
			return 2;
		}

		public override void OnReset()
		{
			text = string.Empty;
			logError = false;
		}
	}
}
