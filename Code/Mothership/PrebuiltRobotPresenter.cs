using Svelto.Command;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections;
using System.Collections.Generic;

namespace Mothership
{
	internal class PrebuiltRobotPresenter
	{
		private FasterList<string> _selectedRobotIds = new FasterList<string>();

		private Dictionary<string, Dictionary<string, FasterList<PrebuiltRobotOption>>> _prebuiltRobotDataByClass = new Dictionary<string, Dictionary<string, FasterList<PrebuiltRobotOption>>>();

		private MothershipPropActivator _mothershipPropActivator;

		private PrebuiltRobotView _view;

		[Inject]
		private ICommandFactory commandFactory
		{
			get;
			set;
		}

		[Inject]
		private IServiceRequestFactory serviceFactory
		{
			get;
			set;
		}

		[Inject]
		private LoadingIconPresenter loadingIconPresenter
		{
			get;
			set;
		}

		[Inject]
		private PrebuiltRobotBuilder prebuiltRobotBuilder
		{
			get;
			set;
		}

		public IEnumerator LoadData()
		{
			Dictionary<string, PrebuiltRobotPart> prebuiltRobotsData = new Dictionary<string, PrebuiltRobotPart>();
			FasterList<string> defaultRobotPartIds = new FasterList<string>();
			ILoadPrebuiltRobotDataRequest request = serviceFactory.Create<ILoadPrebuiltRobotDataRequest>();
			TaskService<PrebuiltRobotsDependency> task = new TaskService<PrebuiltRobotsDependency>(request);
			yield return task;
			if (task.succeeded)
			{
				using (Dictionary<string, RobotPartData>.Enumerator enumerator = task.result.prebuiltRobotsById.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						RobotPartData value = enumerator.Current.Value;
						PrebuiltRobotPart prebuiltRobotPart = new PrebuiltRobotPart(enumerator.Current.Key, value);
						prebuiltRobotPart.machineModel = new MachineModel(value.Data);
						prebuiltRobotPart.machineModel.SetColorData(value.ColourData);
						prebuiltRobotsData.Add(value.RobotId, prebuiltRobotPart);
					}
				}
			}
			else
			{
				ErrorWindow.ShowServiceErrorWindow(task.behaviour);
			}
			Dictionary<string, FasterList<RobotPartData>> prebuiltRobotsByClass = task.result.prebuiltRobotsByClass;
			List<string> categories = new List<string>();
			using (Dictionary<string, FasterList<RobotPartData>>.Enumerator enumerator2 = prebuiltRobotsByClass.GetEnumerator())
			{
				if (enumerator2.MoveNext())
				{
					KeyValuePair<string, FasterList<RobotPartData>> current = enumerator2.Current;
					for (int i = 0; i < current.Value.get_Count(); i++)
					{
						RobotPartData robotPartData = current.Value.get_Item(i);
						string categoryStrKey = robotPartData.CategoryStrKey;
						if (!categories.Contains(categoryStrKey))
						{
							defaultRobotPartIds.Add(robotPartData.RobotId);
							categories.Add(categoryStrKey);
						}
					}
				}
			}
			PopulateLists(prebuiltRobotsByClass);
			yield return prebuiltRobotBuilder.Initialise(prebuiltRobotsData, defaultRobotPartIds);
		}

		public void Show(bool enable)
		{
			if (enable)
			{
				_view.classContainer.ResetOptions();
				ShowCategoriesForRobotClass();
			}
			prebuiltRobotBuilder.ShowRobotBuilder(enable);
			_view.Show(enable);
		}

		public void ShowLoadingIcon(bool enable)
		{
			if (enable)
			{
				loadingIconPresenter.NotifyLoading("RobotBuilding", StringTableBase<StringTable>.Instance.GetString("strRobotBuilding"));
			}
			else
			{
				loadingIconPresenter.NotifyLoadingDone("RobotBuilding");
			}
		}

		public bool IsActive()
		{
			return _view.get_gameObject().get_activeSelf();
		}

		public void SetMothershipPropActivator(MothershipPropActivator mothershipPropActivator)
		{
			_mothershipPropActivator = mothershipPropActivator;
		}

		public void SetView(PrebuiltRobotView view)
		{
			_view = view;
		}

		public void ShowColors(PaletteColor[] colors)
		{
			_view.ShowColors(colors);
		}

		public void OptionChanged(PrebuiltRobotOption selectedOption)
		{
			if (selectedOption.optionType == PrebuiltRobotType.RobotClass)
			{
				ShowCategoriesForRobotClass();
			}
			_selectedRobotIds.Clear();
			PrebuiltRobotOptionView[] categoriesContainer = _view.categoriesContainer;
			foreach (PrebuiltRobotOptionView prebuiltRobotOptionView in categoriesContainer)
			{
				_selectedRobotIds.Add(prebuiltRobotOptionView.selectedOption.id);
			}
			prebuiltRobotBuilder.BuildSelectedRobotIds(_selectedRobotIds);
		}

		public void ButtonClicked(ButtonType buttonType)
		{
			switch (buttonType)
			{
			case ButtonType.NextRandomColor:
				prebuiltRobotBuilder.NextRandomColor();
				prebuiltRobotBuilder.ColorRobot();
				break;
			case ButtonType.CreateRobot:
				commandFactory.Build<CreatePrebuiltRobotCommand>().Inject(prebuiltRobotBuilder.prebuiltRobotMachineMap).Execute();
				break;
			}
		}

		private void PopulateLists(Dictionary<string, FasterList<RobotPartData>> prebuiltRobotDataByClass)
		{
			FasterList<PrebuiltRobotOption> val = new FasterList<PrebuiltRobotOption>();
			foreach (KeyValuePair<string, FasterList<RobotPartData>> item in prebuiltRobotDataByClass)
			{
				string key = item.Key;
				val.Add(new PrebuiltRobotOption(key));
				_prebuiltRobotDataByClass.Add(key, new Dictionary<string, FasterList<PrebuiltRobotOption>>());
				for (int i = 0; i < item.Value.get_Count(); i++)
				{
					RobotPartData robotPartData = item.Value.get_Item(i);
					string categoryStrKey = robotPartData.CategoryStrKey;
					if (!_prebuiltRobotDataByClass[key].ContainsKey(categoryStrKey))
					{
						_prebuiltRobotDataByClass[key].Add(categoryStrKey, new FasterList<PrebuiltRobotOption>());
					}
					_prebuiltRobotDataByClass[key][categoryStrKey].Add(new PrebuiltRobotOption(robotPartData.RobotId, robotPartData.NameStrKey));
				}
			}
			_view.classContainer.SetOptions(val);
		}

		private void ShowCategoriesForRobotClass()
		{
			string strKey = _view.classContainer.selectedOption.strKey;
			Dictionary<string, FasterList<PrebuiltRobotOption>> dictionary = _prebuiltRobotDataByClass[strKey];
			object.Equals(dictionary.Count, _view.categoriesContainer.Length);
			int num = 0;
			using (Dictionary<string, FasterList<PrebuiltRobotOption>>.Enumerator enumerator = dictionary.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					PrebuiltRobotOptionView prebuiltRobotOptionView = _view.categoriesContainer[num++];
					prebuiltRobotOptionView.SetOptions(enumerator.Current.Value);
				}
			}
		}
	}
}
