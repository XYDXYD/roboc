using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Gets the value from the property specified. Returns success if the property was retrieved.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=148")]
	[TaskCategory("Reflection")]
	[TaskIcon("{SkinColor}ReflectionIcon.png")]
	public class GetPropertyValue : Action
	{
		[Tooltip("The GameObject to get the property of")]
		public SharedGameObject targetGameObject;

		[Tooltip("The component to get the property of")]
		public SharedString componentName;

		[Tooltip("The name of the property")]
		public SharedString propertyName;

		[Tooltip("The value of the property")]
		[RequiredField]
		public SharedVariable propertyValue;

		public GetPropertyValue()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (propertyValue == null)
			{
				Debug.LogWarning((object)"Unable to get property - property value is null");
				return 1;
			}
			Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(componentName.get_Value());
			if (typeWithinAssembly == null)
			{
				Debug.LogWarning((object)"Unable to get property - type is null");
				return 1;
			}
			Component component = this.GetDefaultGameObject(targetGameObject.get_Value()).GetComponent(typeWithinAssembly);
			if (component == null)
			{
				Debug.LogWarning((object)("Unable to get the property with component " + componentName.get_Value()));
				return 1;
			}
			PropertyInfo property = ((object)component).GetType().GetProperty(propertyName.get_Value());
			propertyValue.SetValue(property.GetValue(component, null));
			return 2;
		}

		public override void OnReset()
		{
			targetGameObject = null;
			componentName = null;
			propertyName = null;
			propertyValue = null;
		}
	}
}
