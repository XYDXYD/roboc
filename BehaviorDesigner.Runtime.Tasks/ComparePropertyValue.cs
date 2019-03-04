using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Compares the property value to the value specified. Returns success if the values are the same.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=152")]
	[TaskCategory("Reflection")]
	[TaskIcon("{SkinColor}ReflectionIcon.png")]
	public class ComparePropertyValue : Conditional
	{
		[Tooltip("The GameObject to compare the property of")]
		public SharedGameObject targetGameObject;

		[Tooltip("The component to compare the property of")]
		public SharedString componentName;

		[Tooltip("The name of the property")]
		public SharedString propertyName;

		[Tooltip("The value to compare to")]
		public SharedVariable compareValue;

		public ComparePropertyValue()
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
				Debug.LogWarning((object)"Unable to compare property - type is null");
				return 1;
			}
			Component component = this.GetDefaultGameObject(targetGameObject.get_Value()).GetComponent(typeWithinAssembly);
			if (component == null)
			{
				Debug.LogWarning((object)("Unable to compare the property with component " + componentName.get_Value()));
				return 1;
			}
			PropertyInfo property = ((object)component).GetType().GetProperty(propertyName.get_Value());
			object value = property.GetValue(component, null);
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
			propertyName = null;
			compareValue = null;
		}
	}
}
