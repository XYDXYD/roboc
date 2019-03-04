using System;
using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Start a new behavior tree and return success after it has been started.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=20")]
	[TaskIcon("{SkinColor}StartBehaviorTreeIcon.png")]
	public class StartBehaviorTree : Action
	{
		[Tooltip("The GameObject of the behavior tree that should be started. If null use the current behavior")]
		public SharedGameObject behaviorGameObject;

		[Tooltip("The group of the behavior tree that should be started")]
		public SharedInt group;

		[Tooltip("Should this task wait for the behavior tree to complete?")]
		public SharedBool waitForCompletion = false;

		[Tooltip("Should the variables be synchronized?")]
		public SharedBool synchronizeVariables;

		private bool behaviorComplete;

		private Behavior behavior;

		public StartBehaviorTree()
			: this()
		{
		}

		public unsafe override void OnStart()
		{
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Expected O, but got Unknown
			Behavior[] components = this.GetDefaultGameObject(behaviorGameObject.get_Value()).GetComponents<Behavior>();
			if (components.Length == 1)
			{
				behavior = components[0];
			}
			else if (components.Length > 1)
			{
				for (int i = 0; i < components.Length; i++)
				{
					if (components[i].get_Group() == group.get_Value())
					{
						behavior = components[i];
						break;
					}
				}
				if (behavior == null)
				{
					behavior = components[0];
				}
			}
			if (!(behavior != null))
			{
				return;
			}
			List<SharedVariable> allVariables = this.get_Owner().GetAllVariables();
			if (allVariables != null && synchronizeVariables.get_Value())
			{
				for (int j = 0; j < allVariables.Count; j++)
				{
					behavior.SetVariableValue(allVariables[j].get_Name(), (object)allVariables[j]);
				}
			}
			behavior.EnableBehavior();
			if (waitForCompletion.get_Value())
			{
				behaviorComplete = false;
				behavior.add_OnBehaviorEnd(new BehaviorHandler((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		public override TaskStatus OnUpdate()
		{
			if (!(behavior == null))
			{
				if (waitForCompletion.get_Value() && !behaviorComplete)
				{
					return 3;
				}
				return 2;
			}
			return 1;
		}

		private void BehaviorEnded(Behavior behavior)
		{
			behaviorComplete = true;
		}

		public unsafe override void OnEnd()
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			if (behavior != null && waitForCompletion.get_Value())
			{
				behavior.remove_OnBehaviorEnd(new BehaviorHandler((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			}
		}

		public override void OnReset()
		{
			behaviorGameObject = null;
			group = 0;
			waitForCompletion = false;
			synchronizeVariables = false;
		}
	}
}
