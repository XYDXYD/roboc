using System;
using System.Collections.Generic;

internal sealed class GraphTraversalUtility
{
	public static void Traverse<NodeType>(NodeType root, Action<NodeType> OnNodeProcessed, Action<NodeType, NodeType> OnNeighbourProcessed) where NodeType : IGraphNode<NodeType>
	{
		Queue<NodeType> worklist = (Queue<NodeType>)new Queue<NodeType>();
		Dictionary<NodeType, bool> visited = (Dictionary<NodeType, bool>)new Dictionary<NodeType, bool>();
		((Queue<NodeType>)worklist).Enqueue(root);
		while (((Queue<NodeType>)worklist).Count > 0)
		{
			NodeType currentNode = (NodeType)((Queue<NodeType>)worklist).Dequeue();
			OnNodeProcessed((NodeType)currentNode);
			root.VisitNeighbours(delegate(NodeType neighbour)
			{
				if (!((Dictionary<NodeType, bool>)visited).ContainsKey(neighbour))
				{
					((Dictionary<NodeType, bool>)visited).Add(neighbour, value: false);
					OnNeighbourProcessed((NodeType)currentNode, neighbour);
					((Queue<NodeType>)worklist).Enqueue(neighbour);
				}
			});
		}
	}
}
