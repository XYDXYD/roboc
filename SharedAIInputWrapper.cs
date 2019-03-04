using BehaviorDesigner.Runtime;
using System;

[Serializable]
public class SharedAIInputWrapper : SharedVariable<AIInputWrapper>
{
	public static implicit operator SharedAIInputWrapper(AIInputWrapper value)
	{
		SharedAIInputWrapper sharedAIInputWrapper = new SharedAIInputWrapper();
		sharedAIInputWrapper.set_Value(value);
		return sharedAIInputWrapper;
	}
}
