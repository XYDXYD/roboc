using System.Collections.Generic;

namespace BehaviorDesigner.Runtime.Tasks
{
	[TaskDescription("Similar to the selector task, the priority selector task will return success as soon as a child task returns success. Instead of running the tasks sequentially from left to right within the tree, the priority selector will ask the task what its priority is to determine the order. The higher priority tasks have a higher chance at being run first.")]
	[HelpURL("http://www.opsive.com/assets/BehaviorDesigner/documentation.php?id=29")]
	[TaskIcon("{SkinColor}PrioritySelectorIcon.png")]
	public class PrioritySelector : Composite
	{
		private int currentChildIndex;

		private TaskStatus executionStatus;

		private List<int> childrenExecutionOrder = new List<int>();

		public PrioritySelector()
			: this()
		{
		}

		public override void OnStart()
		{
			childrenExecutionOrder.Clear();
			for (int i = 0; i < base.children.Count; i++)
			{
				float priority = base.children[i].GetPriority();
				int index = childrenExecutionOrder.Count;
				for (int j = 0; j < childrenExecutionOrder.Count; j++)
				{
					if (base.children[childrenExecutionOrder[j]].GetPriority() < priority)
					{
						index = j;
						break;
					}
				}
				childrenExecutionOrder.Insert(index, i);
			}
		}

		public override int CurrentChildIndex()
		{
			return childrenExecutionOrder[currentChildIndex];
		}

		public override bool CanExecute()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Invalid comparison between Unknown and I4
			return currentChildIndex < base.children.Count && (int)executionStatus != 2;
		}

		public override void OnChildExecuted(TaskStatus childStatus)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			currentChildIndex++;
			executionStatus = childStatus;
		}

		public override void OnConditionalAbort(int childIndex)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			currentChildIndex = childIndex;
			executionStatus = 0;
		}

		public override void OnEnd()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			executionStatus = 0;
			currentChildIndex = 0;
		}
	}
}
