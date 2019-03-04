using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Compares the field value to the value specified. Returns success if the values are the same.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=151")]
	[TaskCategory("Reflection")]
	[TaskIcon("{SkinColor}ReflectionIcon.png")]
	public class CompareFieldValue : Conditional
	{
		[Tooltip("The GameObject to compare the field on")]
		public SharedGameObject targetGameObject;

		[Tooltip("The component to compare the field on")]
		public SharedString componentName;

		[Tooltip("The name of the field")]
		public SharedString fieldName;

		[Tooltip("The value to compare to")]
		public SharedVariable compareValue;

		public CompareFieldValue()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (compareValue == null)
			{
				Debug.LogWarning((object)"Unable to compare field - compare value is null");
				return 1;
			}
			Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(componentName.get_Value());
			if (typeWithinAssembly == null)
			{
				Debug.LogWarning((object)"Unable to compare field - type is null");
				return 1;
			}
			Component component = this.GetDefaultGameObject(targetGameObject.get_Value()).GetComponent(typeWithinAssembly);
			if (component == null)
			{
				Debug.LogWarning((object)("Unable to compare the field with component " + componentName.get_Value()));
				return 1;
			}
			FieldInfo field = ((object)component).GetType().GetField(fieldName.get_Value());
			object value = field.GetValue(component);
			if (value == null && compareValue.GetValue() == null)
			{
				return 2;
			}
			return (!value.Equals(compareValue.GetValue())) ? 1 : 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			componentName = null;
			fieldName = null;
			compareValue = null;
		}
	}
}
