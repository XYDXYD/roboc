using Svelto.DataStructures;
using System.Collections.Generic;

internal sealed class CubeNodeInstance
{
	private FasterList<CubeNodeInstance> _neighbours;

	private FasterList<CubeNodeInstance> _originalNeighbours;

	private FasterList<CubeNodeInstance> _neighboursWithLinkToMe;

	private List<ConnectionPoint> _adjacentCubeLocations;

	private Dictionary<CubeNodeInstance, ConnectionPoint> _neighbourConnectionPoint;

	private CubeNodeInstance _linkToChair;

	public bool isDestroyed;

	public bool processed;

	public bool processedSearch;

	private InstantiatedCube _instantiatedCube;

	public InstantiatedCube instantiatedCube
	{
		get
		{
			return _instantiatedCube;
		}
		set
		{
			_instantiatedCube = value;
			_adjacentCubeLocations = _instantiatedCube.persistentCubeData.connectionPoints;
		}
	}

	public CubeNodeInstance linkToChair
	{
		get
		{
			return _linkToChair;
		}
		set
		{
			if (_linkToChair != null)
			{
				_linkToChair.RemoveNodeLink(this);
			}
			_linkToChair = value;
			if (_linkToChair != null)
			{
				numLinksToChair = _linkToChair.numLinksToChair + 1;
			}
			else
			{
				numLinksToChair = 0;
			}
			if (_linkToChair != null)
			{
				_linkToChair.AddNodeLink(this);
			}
		}
	}

	public int numLinksToChair
	{
		get;
		private set;
	}

	public CubeNodeInstance()
	{
		_neighboursWithLinkToMe = new FasterList<CubeNodeInstance>(CubeFaceExtensions.NumberOfFaces());
		_neighbours = new FasterList<CubeNodeInstance>(CubeFaceExtensions.NumberOfFaces());
		_originalNeighbours = new FasterList<CubeNodeInstance>(CubeFaceExtensions.NumberOfFaces());
		_neighbourConnectionPoint = new Dictionary<CubeNodeInstance, ConnectionPoint>();
	}

	public void AddLink(CubeNodeInstance node, ConnectionPoint connectionPoint)
	{
		if (!_neighbours.Contains(node))
		{
			_neighbours.Add(node);
			_originalNeighbours.Add(node);
			_neighbourConnectionPoint[node] = connectionPoint;
		}
	}

	public void ReAddLink(CubeNodeInstance node)
	{
		_neighbours.Add(node);
	}

	public void BreakLink(CubeNodeInstance node)
	{
		_neighbours.UnorderedRemove(node);
		_neighboursWithLinkToMe.UnorderedRemove(node);
	}

	public void BreakLinks()
	{
		for (int num = _neighbours.get_Count() - 1; num >= 0; num--)
		{
			_neighbours.get_Item(num).BreakLink(this);
			BreakLink(_neighbours.get_Item(num));
		}
	}

	public void BreakOriginalLinks()
	{
		for (int num = _originalNeighbours.get_Count() - 1; num >= 0; num--)
		{
			_originalNeighbours.get_Item(num).BreakLink(this);
			BreakLink(_originalNeighbours.get_Item(num));
		}
	}

	private void BrakOriginalLink(CubeNodeInstance node)
	{
		_originalNeighbours.UnorderedRemove(node);
	}

	public FasterList<CubeNodeInstance> GetNeighbours()
	{
		return _neighbours;
	}

	public FasterList<CubeNodeInstance> GetOriginalNeighbours()
	{
		return _originalNeighbours;
	}

	public FasterList<CubeNodeInstance> GetNeighboursThatLinkToMe()
	{
		return _neighboursWithLinkToMe;
	}

	public void UpdateNumberOfLinksToChair()
	{
		if (_linkToChair != null)
		{
			numLinksToChair = _linkToChair.numLinksToChair + 1;
		}
	}

	public void AddNodeLink(CubeNodeInstance nodeThatLinksToMe)
	{
		_neighboursWithLinkToMe.Add(nodeThatLinksToMe);
	}

	public void RemoveNodeLink(CubeNodeInstance nodeThatLinksToMe)
	{
		_neighboursWithLinkToMe.UnorderedRemove(nodeThatLinksToMe);
	}

	public List<ConnectionPoint> GetAdjacentCubeLocations()
	{
		return _adjacentCubeLocations;
	}

	public ConnectionPoint GetConnectionPointOfNeighbour(CubeNodeInstance neighbour)
	{
		return _neighbourConnectionPoint[neighbour];
	}
}
