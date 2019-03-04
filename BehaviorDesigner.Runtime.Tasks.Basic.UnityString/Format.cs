using System;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityString
{
	[TaskCategory("Basic/String")]
	[TaskDescription("Stores a string with the specified format.")]
	public class Format : Action
	{
		[Tooltip("The format of the string")]
		public SharedString format;

		[Tooltip("Any variables to appear in the string")]
		public SharedGenericVariable[] variables;

		[Tooltip("The result of the format")]
		[RequiredField]
		public SharedString storeResult;

		private object[] variableValues;

		public Format()
			: this()
		{
		}

		public override void OnAwake()
		{
			variableValues = new object[variables.Length];
		}

		public override TaskStatus OnUpdate()
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < variableValues.Length; i++)
			{
				variableValues[i] = variables[i].get_Value().value.GetValue();
			}
			try
			{
				storeResult.set_Value(string.Format(format.get_Value(), variableValues));
			}
			catch (Exception ex)
			{
				Debug.LogError((object)ex.Message);
				return 1;
			}
			return 2;
		}

		public override void OnReset()
		{
			format = string.Empty;
			variables = null;
			storeResult = null;
		}
	}
}
