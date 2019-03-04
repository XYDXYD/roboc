using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Gets the value from the field specified. Returns success if the field was retrieved.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=147")]
	[TaskCategory("Reflection")]
	[TaskIcon("{SkinColor}ReflectionIcon.png")]
	public class GetFieldValue : Action
	{
		[Tooltip("The GameObject to get the field on")]
		public SharedGameObject targetGameObject;

		[Tooltip("The component to get the field on")]
		public SharedString componentName;

		[Tooltip("The name of the field")]
		public SharedString fieldName;

		[Tooltip("The value of the field")]
		[RequiredField]
		public SharedVariable fieldValue;

		public GetFieldValue()
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
				Debug.LogWarning((object)"Unable to get field - type is null");
				return 1;
			}
			Component component = this.GetDefaultGameObject(targetGameObject.get_Value()).GetComponent(typeWithinAssembly);
			if (component == null)
			{
				Debug.LogWarning((object)("Unable to get the field with component " + componentName.get_Value()));
				return 1;
			}
			FieldInfo field = ((object)component).GetType().GetField(fieldName.get_Value());
			fieldValue.SetValue(field.GetValue(component));
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
