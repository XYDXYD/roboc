using Mothership;
using Svelto.DataStructures;
using Svelto.IoC;
using System;
using System.Collections.Generic;

internal sealed class GarageSlotOrderPresenter
{
	private class GarageSlotComparer : IComparer<GarageSlotDependency>
	{
		public int Compare(GarageSlotDependency x, GarageSlotDependency y)
		{
			if (x.garageSlot < y.garageSlot)
			{
				return -1;
			}
			if (x.garageSlot > y.garageSlot)
			{
				return 1;
			}
			return 0;
		}
	}

	private readonly FasterList<int> _garageSlotOrder = new FasterList<int>();

	private readonly FasterList<GarageSlotDependency> _sortedGarageSlots = new FasterList<GarageSlotDependency>();

	private readonly GarageSlotComparer _garageSlotComparer = new GarageSlotComparer();

	[Inject]
	public GarageSlotsPresenter garageSlotsPresenter
	{
		private get;
		set;
	}

	public FasterList<int> currentSlotOrder => _garageSlotOrder;

	public event Action OnSlotsReordered = delegate
	{
	};

	public void OnGarageDataLoaded(FasterList<int> garageSlotOrder)
	{
		_garageSlotOrder.FastClear();
		_garageSlotOrder.AddRange(garageSlotOrder);
	}

	public void ShowOrderedGarageSlots(Dictionary<int, GarageSlotDependency> garageSlots, uint currentGarageSlot, uint newGarageSlotLimit)
	{
		_sortedGarageSlots.FastClear();
		_sortedGarageSlots.AddRange((ICollection<GarageSlotDependency>)garageSlots.Values);
		SortGarageSlots(_sortedGarageSlots);
		garageSlotsPresenter.PopulateSlots(_sortedGarageSlots, currentGarageSlot, newGarageSlotLimit);
	}

	public void GarageSlotDeletedSuccess(uint deletedSlot)
	{
		FasterList<int> val = new FasterList<int>(_garageSlotOrder);
		_garageSlotOrder.FastClear();
		for (int i = 0; i < val.get_Count(); i++)
		{
			if (val.get_Item(i) != deletedSlot)
			{
				_garageSlotOrder.Add(val.get_Item(i));
			}
		}
	}

	public void MoveSlotLeft(uint garageSlotId)
	{
		int num = FindCurrentIndex(garageSlotId);
		int num2 = num - 1;
		if (num2 >= 0)
		{
			RearrangeSlots(garageSlotId, num, num2);
		}
	}

	public void MoveSlotRight(uint garageSlotId)
	{
		int num = FindCurrentIndex(garageSlotId);
		int num2 = num + 1;
		if (num2 < _sortedGarageSlots.get_Count())
		{
			RearrangeSlots(garageSlotId, num, num2);
		}
	}

	public void HandleUIMessage(GarageSlot slot, object message)
	{
		if (message is ButtonType)
		{
			switch ((ButtonType)message)
			{
			case ButtonType.MoveGarageSlotLeft:
				MoveSlotLeft(slot.slotId);
				break;
			case ButtonType.MoveGarageSlotRight:
				MoveSlotRight(slot.slotId);
				break;
			}
		}
	}

	private void SortGarageSlots(FasterList<GarageSlotDependency> sortedGarageSlots)
	{
		if (_garageSlotOrder.get_Count() == 0)
		{
			sortedGarageSlots.Sort((IComparer<GarageSlotDependency>)_garageSlotComparer);
			for (int i = 0; i < sortedGarageSlots.get_Count(); i++)
			{
				_garageSlotOrder.Add((int)sortedGarageSlots.get_Item(i).garageSlot);
			}
		}
		else
		{
			SortSlotsByOrder(sortedGarageSlots);
		}
	}

	private void SortSlotsByOrder(FasterList<GarageSlotDependency> sortedGarageSlots)
	{
		FasterList<GarageSlotDependency> val = new FasterList<GarageSlotDependency>(sortedGarageSlots);
		sortedGarageSlots.FastClear();
		FasterList<int> val2 = new FasterList<int>();
		for (int i = 0; i < _garageSlotOrder.get_Count(); i++)
		{
			bool flag = false;
			for (int j = 0; j < val.get_Count(); j++)
			{
				if (val.get_Item(j).garageSlot == _garageSlotOrder.get_Item(i))
				{
					sortedGarageSlots.Add(val.get_Item(j));
					val.UnorderedRemoveAt(j);
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				val2.Add(i);
			}
		}
		if (val2.get_Count() > 0)
		{
			for (int num = val2.get_Count() - 1; num >= 0; num--)
			{
				_garageSlotOrder.RemoveAt(val2.get_Item(num));
			}
		}
		if (val.get_Count() > 0)
		{
			for (int k = 0; k < val.get_Count(); k++)
			{
				_sortedGarageSlots.Add(val.get_Item(k));
				_garageSlotOrder.Add((int)val.get_Item(k).garageSlot);
			}
			this.OnSlotsReordered();
		}
	}

	private void RearrangeSlots(uint garageSlotId, int currentIndex, int newIndex)
	{
		GarageSlotDependency garageSlotDependency = _sortedGarageSlots.get_Item(currentIndex);
		_garageSlotOrder.RemoveAt(currentIndex);
		_sortedGarageSlots.RemoveAt(currentIndex);
		if (newIndex == _garageSlotOrder.get_Count())
		{
			_garageSlotOrder.Add((int)garageSlotId);
			_sortedGarageSlots.Add(garageSlotDependency);
		}
		else
		{
			_garageSlotOrder.Insert(newIndex, (int)garageSlotId);
			_sortedGarageSlots.Insert(newIndex, garageSlotDependency);
		}
		ReorderSlots(garageSlotId);
	}

	private int FindCurrentIndex(uint garageSlot)
	{
		for (int i = 0; i < _garageSlotOrder.get_Count(); i++)
		{
			if (_garageSlotOrder.get_Item(i) == (int)garageSlot)
			{
				return i;
			}
		}
		return _garageSlotOrder.get_Count();
	}

	private void ReorderSlots(uint garageSlotId)
	{
		garageSlotsPresenter.ReorderGarageSlots(_sortedGarageSlots, garageSlotId);
		this.OnSlotsReordered();
	}
}
