using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Sets the field to the value specified. Returns success if the field was set.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=149")]
	[TaskCategory("Reflection")]
	[TaskIcon("{SkinColor}ReflectionIcon.png")]
	public class SetFieldValue : Action
	{
		[Tooltip("The GameObject to set the field on")]
		public SharedGameObject targetGameObject;

		[Tooltip("The component to set the field on")]
		public SharedString componentName;

		[Tooltip("The name of the field")]
		public SharedString fieldName;

		[Tooltip("The value to set")]
		public SharedVariable fieldValue;

		public SetFieldValue()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (fieldValue == null)
			{
				Debug.LogWarning((object)"Unable to get field - field value is null");
				return 1;
			}
			Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(componentName.get_Value());
			if (typeWithinAssembly == null)
			{
				Debug.LogWarning((object)"Unable to set field - type is null");
				return 1;
			}
			Component component = this.GetDefaultGameObject(targetGameObject.get_Value()).GetComponent(typeWithinAssembly);
			if (component == null)
			{
				Debug.LogWarning((object)("Unable to set the field with component " + componentName.get_Value()));
				return 1;
			}
			FieldInfo field = ((object)component).GetType().GetField(fieldName.get_Value());
			field.SetValue(component, fieldValue.GetValue());
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			componentName = null;
			fieldName = null;
			fieldValue = null;
		}
	}
}
