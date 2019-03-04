using System;
using System.Collections.Generic;
using System.Diagnostics;

public class LeftLeaningKeyedRedBlackTree<TKey> where TKey : IComparable<TKey>
{
	[DebuggerDisplay("Key={Key}")]
	private class Node
	{
		public TKey Key;

		public Node Left;

		public Node Right;

		public bool IsBlack;
	}

	private Node _rootNode;

	public int Count
	{
		get;
		private set;
	}

	public TKey MinimumKey => GetExtreme(_rootNode, (Node n) => n.Left, (Node n) => n.Key);

	public TKey MaximumKey => GetExtreme(_rootNode, (Node n) => n.Right, (Node n) => n.Key);

	public void Add(TKey key)
	{
		_rootNode = Add(_rootNode, key);
		_rootNode.IsBlack = true;
	}

	public bool Remove(TKey key)
	{
		int count = Count;
		if (_rootNode != null)
		{
			_rootNode = Remove(_rootNode, key);
			if (_rootNode != null)
			{
				_rootNode.IsBlack = true;
			}
		}
		return count != Count;
	}

	public void Clear()
	{
		_rootNode = null;
		Count = 0;
	}

	public IEnumerable<TKey> GetKeys()
	{
		TKey lastKey = default(TKey);
		bool lastKeyValid = false;
		return Traverse(_rootNode, (Node n) => !lastKeyValid || !object.Equals(lastKey, n.Key), delegate(Node n)
		{
			lastKey = n.Key;
			lastKeyValid = true;
			return lastKey;
		});
	}

	private static bool IsRed(Node node)
	{
		if (node == null)
		{
			return false;
		}
		return !node.IsBlack;
	}

	private Node Add(Node node, TKey key)
	{
		if (node == null)
		{
			Count++;
			Node node2 = new Node();
			node2.Key = key;
			return node2;
		}
		if (IsRed(node.Left) && IsRed(node.Right))
		{
			FlipColor(node);
		}
		int num = KeyComparison(key, node.Key);
		if (num < 0)
		{
			node.Left = Add(node.Left, key);
		}
		else if (0 < num)
		{
			node.Right = Add(node.Right, key);
		}
		if (IsRed(node.Right))
		{
			node = RotateLeft(node);
		}
		if (IsRed(node.Left) && IsRed(node.Left.Left))
		{
			node = RotateRight(node);
		}
		return node;
	}

	private Node Remove(Node node, TKey key)
	{
		int num = KeyComparison(key, node.Key);
		if (num < 0)
		{
			if (node.Left != null)
			{
				if (!IsRed(node.Left) && !IsRed(node.Left.Left))
				{
					node = MoveRedLeft(node);
				}
				node.Left = Remove(node.Left, key);
			}
		}
		else
		{
			if (IsRed(node.Left))
			{
				node = RotateRight(node);
			}
			if (KeyComparison(key, node.Key) == 0 && node.Right == null)
			{
				Count--;
				return null;
			}
			if (node.Right != null)
			{
				if (!IsRed(node.Right) && !IsRed(node.Right.Left))
				{
					node = MoveRedRight(node);
				}
				if (KeyComparison(key, node.Key) == 0)
				{
					Count--;
					Node extreme = GetExtreme(node.Right, (Node n) => n.Left, (Node n) => n);
					node.Key = extreme.Key;
					node.Right = DeleteMinimum(node.Right);
				}
				else
				{
					node.Right = Remove(node.Right, key);
				}
			}
		}
		return FixUp(node);
	}

	private static void FlipColor(Node node)
	{
		node.IsBlack = !node.IsBlack;
		node.Left.IsBlack = !node.Left.IsBlack;
		node.Right.IsBlack = !node.Right.IsBlack;
	}

	private static Node RotateLeft(Node node)
	{
		Node right = node.Right;
		node.Right = right.Left;
		right.Left = node;
		right.IsBlack = node.IsBlack;
		node.IsBlack = false;
		return right;
	}

	private static Node RotateRight(Node node)
	{
		Node left = node.Left;
		node.Left = left.Right;
		left.Right = node;
		left.IsBlack = node.IsBlack;
		node.IsBlack = false;
		return left;
	}

	private static Node MoveRedLeft(Node node)
	{
		FlipColor(node);
		if (IsRed(node.Right.Left))
		{
			node.Right = RotateRight(node.Right);
			node = RotateLeft(node);
			FlipColor(node);
			if (IsRed(node.Right.Right))
			{
				node.Right = RotateLeft(node.Right);
			}
		}
		return node;
	}

	private static Node MoveRedRight(Node node)
	{
		FlipColor(node);
		if (IsRed(node.Left.Left))
		{
			node = RotateRight(node);
			FlipColor(node);
		}
		return node;
	}

	private Node DeleteMinimum(Node node)
	{
		if (node.Left == null)
		{
			return null;
		}
		if (!IsRed(node.Left) && !IsRed(node.Left.Left))
		{
			node = MoveRedLeft(node);
		}
		node.Left = DeleteMinimum(node.Left);
		return FixUp(node);
	}

	private static Node FixUp(Node node)
	{
		if (IsRed(node.Right))
		{
			node = RotateLeft(node);
		}
		if (IsRed(node.Left) && IsRed(node.Left.Left))
		{
			node = RotateRight(node);
		}
		if (IsRed(node.Left) && IsRed(node.Right))
		{
			FlipColor(node);
		}
		if (node.Left != null && IsRed(node.Left.Right) && !IsRed(node.Left.Left))
		{
			node.Left = RotateLeft(node.Left);
			if (IsRed(node.Left))
			{
				node = RotateRight(node);
			}
		}
		return node;
	}

	private Node GetNodeForKey(TKey key)
	{
		Node node = _rootNode;
		while (node != null)
		{
			int num = key.CompareTo(node.Key);
			if (num < 0)
			{
				node = node.Left;
				continue;
			}
			if (0 < num)
			{
				node = node.Right;
				continue;
			}
			return node;
		}
		return null;
	}

	private static T GetExtreme<T>(Node node, Func<Node, Node> successor, Func<Node, T> selector)
	{
		T result = default(T);
		for (Node node2 = node; node2 != null; node2 = successor(node2))
		{
			result = selector(node2);
		}
		return result;
	}

	private IEnumerable<T> Traverse<T>(Node node, Func<Node, bool> condition, Func<Node, T> selector)
	{
		Stack<Node> stack = new Stack<Node>();
		Node current = node;
		while (current != null)
		{
			if (current.Left != null)
			{
				stack.Push(current);
				current = current.Left;
				continue;
			}
			Node node2;
			do
			{
				if (condition(current))
				{
					yield return selector(current);
				}
				current = current.Right;
				if (current != null || 0 >= stack.Count)
				{
					break;
				}
				current = (node2 = stack.Pop());
			}
			while (node2 != null);
		}
	}

	private int KeyComparison(TKey leftKey, TKey rightKey)
	{
		return leftKey.CompareTo(rightKey);
	}
}
