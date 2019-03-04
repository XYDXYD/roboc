using Services;
using Services.TechTree;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Utility;

namespace Mothership.OpsRoom
{
	internal class OpsRoomShowTechTreeCTAEngine : SingleEntityViewEngine<OpsRoomShowTechTreeCTAEntityView>, IQueryingEntityViewEngine, IEngine
	{
		private readonly TechPointsTracker _techPointsTracker;

		private readonly IDataSource<Dictionary<string, TechTreeItemData>> _techTreeDataSource;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public OpsRoomShowTechTreeCTAEngine(TechPointsTracker techPointsTracker, IDataSource<Dictionary<string, TechTreeItemData>> techTreeDataSource)
		{
			_techPointsTracker = techPointsTracker;
			_techTreeDataSource = techTreeDataSource;
			_techPointsTracker.OnUserTechPointsAmountChanged += OnTechPointsChanged;
		}

		public void Ready()
		{
		}

		protected override void Add(OpsRoomShowTechTreeCTAEntityView entityView)
		{
			_techPointsTracker.RefreshUserTechPointsAmount();
		}

		protected override void Remove(OpsRoomShowTechTreeCTAEntityView entityView)
		{
		}

		private void OnTechPointsChanged(int tpBalance)
		{
			TaskRunner.get_Instance().Run(UpdateLabel(tpBalance));
		}

		private IEnumerator UpdateLabel(int tpBalance)
		{
			int usablePoints = 0;
			if (tpBalance > 0)
			{
				Dictionary<string, TechTreeItemData> tree = new Dictionary<string, TechTreeItemData>();
				yield return _techTreeDataSource.GetDataAsync(tree);
				usablePoints = GetUsablePoints(tpBalance, tree);
			}
			FasterListEnumerator<OpsRoomShowTechTreeCTAEntityView> enumerator = entityViewsDB.QueryEntityViews<OpsRoomShowTechTreeCTAEntityView>().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					OpsRoomShowTechTreeCTAEntityView current = enumerator.get_Current();
					current.techTreeCTAComponent.gameObject.SetActive(usablePoints > 0);
					current.techTreeCTAComponent.label.set_text(usablePoints.ToString());
					current.OpsRoomCTAValuesComponent.unspentTP.set_value(usablePoints);
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private static int GetUsablePoints(int tpBalance, Dictionary<string, TechTreeItemData> tree)
		{
			int num = 0;
			if (tree.Count > 0)
			{
				Dictionary<uint, int> dictionary = new Dictionary<uint, int>();
				FasterList<uint> val = new FasterList<uint>();
				Dictionary<string, TechTreeItemData>.Enumerator enumerator = tree.GetEnumerator();
				using (Dictionary<string, TechTreeItemData>.Enumerator enumerator2 = enumerator)
				{
					while (enumerator2.MoveNext())
					{
						TechTreeItemData value = enumerator2.Current.Value;
						if (value.isUnlockable && !value.isUnlocked)
						{
							uint techPointsCost = value.techPointsCost;
							if (!dictionary.ContainsKey(techPointsCost))
							{
								dictionary.Add(techPointsCost, 0);
								val.Add(techPointsCost);
							}
							Dictionary<uint, int> dictionary2;
							uint key;
							(dictionary2 = dictionary)[key = techPointsCost] = dictionary2[key] + 1;
						}
					}
				}
				val.Sort();
				for (int i = 0; i < val.get_Count(); i++)
				{
					uint num2 = val.get_Item(i);
					int num3 = dictionary[num2];
					int num4 = num3;
					if (num2 != 0)
					{
						num4 = Math.Min((int)Math.Floor((double)tpBalance / (double)num2), num3);
					}
					tpBalance -= num4 * (int)num2;
					num += num4;
					if (tpBalance <= 0)
					{
						break;
					}
				}
			}
			else
			{
				Console.LogError("Unable to get data for the tech tree: the tree is empty.");
			}
			return num;
		}
	}
}
