using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Sets the property to the value specified. Returns success if the property was set.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=150")]
	[TaskCategory("Reflection")]
	[TaskIcon("{SkinColor}ReflectionIcon.png")]
	public class SetPropertyValue : Action
	{
		[Tooltip("The GameObject to set the property on")]
		public SharedGameObject targetGameObject;

		[Tooltip("The component to set the property on")]
		public SharedString componentName;

		[Tooltip("The name of the property")]
		public SharedString propertyName;

		[Tooltip("The value to set")]
		public SharedVariable propertyValue;

		public SetPropertyValue()
			: this()
		{
		}

		public override TaskStatus OnUpdate()
		{
			if (propertyValue == null)
			{
				Debug.LogWarning((object)"Unable to get field - field value is null");
				return 1;
			}
			Type typeWithinAssembly = TaskUtility.GetTypeWithinAssembly(componentName.get_Value());
			if (typeWithinAssembly == null)
			{
				Debug.LogWarning((object)"Unable to set property - type is null");
				return 1;
			}
			Component component = this.GetDefaultGameObject(targetGameObject.get_Value()).GetComponent(typeWithinAssembly);
			if (component == null)
			{
				Debug.LogWarning((object)("Unable to set the property with component " + componentName.get_Value()));
				return 1;
			}
			PropertyInfo property = ((object)component).GetType().GetProperty(propertyName.get_Value());
			property.SetValue(component, propertyValue.GetValue(), null);
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
