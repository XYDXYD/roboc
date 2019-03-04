using System;

public interface IGraphNode<T>
{
	void VisitNeighbours(Action<T> onVisiting);
}
