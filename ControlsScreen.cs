using Rewired;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections.Generic;
using UnityEngine;

internal sealed class ControlsScreen : MonoBehaviour, IChainListener, IInitialize
{
	public GameObject actionMappingRowTemplate;

	public GameObject categoryDividerTemplate;

	public GameObject pressAnyKeyDialog;

	public UIScrollBar scrollBar;

	public UIScrollView scrollView;

	public string unassignedButtonText = "strClickToAssign";

	public string unassignedListboxText = "strClickToSelect";

	public int _mappingRowHeight = 35;

	public int _categoryDividerHeight = 70;

	public int _categoryDividerOffset = -35;

	public int _offsetAtTheTop = 35;

	private bool _isActive;

	private Transform _slidingPanel;

	private float _initialHeight;

	private float _maxWindowSize = 360f;

	private Action<int, int, InputAssignmentButton.InputType> _onRemapButtonPressed = delegate
	{
	};

	private Action<int, int, InputAssignmentButton.InputType, string> _onRemapListChanged = delegate
	{
	};

	private FasterList<GameObject> _rows = new FasterList<GameObject>();

	[Inject]
	internal ControlsDisplay controlsDisplay
	{
		private get;
		set;
	}

	[Inject]
	internal GameObjectPool gameObjectPool
	{
		private get;
		set;
	}

	public ControlsScreen()
		: this()
	{
	}

	public void OnDependenciesInjected()
	{
		gameObjectPool.Preallocate(actionMappingRowTemplate.get_name(), 60, (Func<GameObject>)(() => OnGameObjectAllocated(actionMappingRowTemplate)));
		gameObjectPool.Preallocate(categoryDividerTemplate.get_name(), 5, (Func<GameObject>)(() => OnGameObjectAllocated(categoryDividerTemplate)));
		Initialise();
		controlsDisplay.SetScreen(this);
		this.get_gameObject().SetActive(false);
	}

	private GameObject OnGameObjectAllocated(GameObject prefab)
	{
		GameObject val = GameObjectPool.CreateGameObjectFromPrefab(prefab);
		val.SetActive(false);
		return val;
	}

	private void Initialise()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		_slidingPanel = actionMappingRowTemplate.get_transform().get_parent();
		Vector3 localPosition = _slidingPanel.get_localPosition();
		_initialHeight = localPosition.y;
		actionMappingRowTemplate.SetActive(false);
		categoryDividerTemplate.SetActive(false);
		ResetScrollbar();
	}

	public void Show(Action<int, int, InputAssignmentButton.InputType> onRemapButtonPressed, Action<int, int, InputAssignmentButton.InputType, string> onRemapListChanged)
	{
		_onRemapButtonPressed = onRemapButtonPressed;
		_onRemapListChanged = onRemapListChanged;
		pressAnyKeyDialog.SetActive(false);
		this.get_gameObject().SetActive(true);
		_isActive = true;
	}

	public void Hide()
	{
		_isActive = false;
		pressAnyKeyDialog.SetActive(false);
		this.get_gameObject().SetActive(false);
		_onRemapButtonPressed = null;
		_onRemapListChanged = null;
	}

	public bool IsActive()
	{
		return _isActive;
	}

	public void Populate(Dictionary<int, ButtonActionMap> categories, FasterList<string> possibleMouseAxes, FasterList<string> possibleJoypadAxes, FasterList<string> possibleMouseAxesSplit, FasterList<string> possibleJoypadAxesSplit)
	{
		ClearRows();
		int num = 0;
		int num2 = 0;
		possibleMouseAxes.Add(StringTableBase<StringTable>.Instance.GetString("strKeyboardNONE"));
		possibleMouseAxesSplit.Add(StringTableBase<StringTable>.Instance.GetString("strKeyboardNONE"));
		foreach (KeyValuePair<int, ButtonActionMap> category in categories)
		{
			int key = category.Key;
			ButtonActionMap value = category.Value;
			int localYPos = -_mappingRowHeight * num - _categoryDividerHeight * num2 + _offsetAtTheTop;
			AddCategoryDivider(localYPos, value.categoryName);
			num2++;
			Dictionary<int, ButtonReassignData> map = value.map;
			for (int i = 0; i < map.Count; i += 3)
			{
				string actionName = map[i].ActionName();
				localYPos = -_mappingRowHeight * num - _categoryDividerHeight * num2 + _offsetAtTheTop;
				InputAssignmentRow inputAssignmentRow = AddActionRow(i, key, localYPos, actionName);
				num++;
				FasterList<string> possibleAxes;
				FasterList<string> possibleAxes2;
				if (map[i].IsSplit())
				{
					possibleAxes = possibleMouseAxesSplit;
					possibleAxes2 = possibleJoypadAxesSplit;
					UnblockReassignmentButton(inputAssignmentRow.keyboard);
					PopulateReassignmentButton(inputAssignmentRow.keyboard, map[i]);
				}
				else
				{
					possibleAxes = possibleMouseAxes;
					possibleAxes2 = possibleJoypadAxes;
					BlockReassignmentButton(inputAssignmentRow.keyboard);
				}
				PopulateReassignmentListBox(inputAssignmentRow.mouseAxis, map[i + 1], possibleAxes);
				PopulateReassignmentListBox(inputAssignmentRow.joypadAxis, map[i + 2], possibleAxes2);
			}
		}
	}

	public void SetPressAnyKeyDialogActive(bool active)
	{
		pressAnyKeyDialog.SetActive(active);
	}

	public void OnValueChange(float value)
	{
		SetSlidingPanelOffset(value);
	}

	public void Listen(object message)
	{
		if (message is InputAssignmentButton)
		{
			InputAssignmentButton inputAssignmentButton = message as InputAssignmentButton;
			_onRemapButtonPressed(inputAssignmentButton.categoryId, inputAssignmentButton.buttonId, inputAssignmentButton.inputType);
		}
		else if (message is InputAssignmentListBox)
		{
			InputAssignmentListBox inputAssignmentListBox = message as InputAssignmentListBox;
			string[] array = inputAssignmentListBox.listBox.items.ToArray();
			if (inputAssignmentListBox.listBox.get_value().CompareTo(array[array.Length - 1]) == 0)
			{
				_onRemapListChanged(inputAssignmentListBox.categoryId, inputAssignmentListBox.buttonId, inputAssignmentListBox.inputType, null);
			}
			else
			{
				_onRemapListChanged(inputAssignmentListBox.categoryId, inputAssignmentListBox.buttonId, inputAssignmentListBox.inputType, inputAssignmentListBox.listBox.get_value());
			}
		}
		else
		{
			if (!(message is ButtonType))
			{
				throw new Exception("Unhandled message from KeyConfiguration.");
			}
			ButtonType buttonType = (ButtonType)message;
			controlsDisplay.ButtonClicked(buttonType);
		}
	}

	public void ResetScrollbar()
	{
		scrollBar.Set(0f, true);
		scrollView.ResetPosition();
	}

	private void ClearRows()
	{
		for (int i = 0; i < _rows.get_Count(); i++)
		{
			GameObject val = _rows.get_Item(i);
			gameObjectPool.Recycle(val, val.get_name());
			val.SetActive(false);
		}
		_rows.FastClear();
	}

	private void AddCategoryDivider(int localYPos, string categoryName)
	{
		CategoryDivider categoryDivider = CreateCategoryDivider(localYPos);
		categoryDivider.categoryName.set_text(StringTableBase<StringTable>.Instance.GetString(categoryName));
	}

	private InputAssignmentRow AddActionRow(int buttonIdOffset, int categoryId, int localYPos, string actionName)
	{
		InputAssignmentRow inputAssignmentRow = CreateAssignmentRow(buttonIdOffset, categoryId, actionName, localYPos);
		inputAssignmentRow.actionName.set_text(actionName);
		return inputAssignmentRow;
	}

	private void PopulateReassignmentButton(InputAssignmentButton assignmentButton, ButtonReassignData buttonData)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		assignmentButton.set_enabled(true);
		SetAssignedActions(assignmentButton, buttonData.KeyCode());
	}

	private void UnblockReassignmentButton(InputAssignmentButton assignmentButton)
	{
		assignmentButton.ResetBlock();
	}

	private void DeactivateReassignmentButton(InputAssignmentButton assignmentButton)
	{
		assignmentButton.Disable();
	}

	private void BlockReassignmentButton(InputAssignmentButton assignmentButton)
	{
		assignmentButton.inputText.set_text(string.Empty);
		assignmentButton.Block();
	}

	private void PopulateReassignmentListBox(InputAssignmentListBox assignmentListBox, ButtonReassignData buttonData, FasterList<string> possibleAxes)
	{
		assignmentListBox.set_enabled(true);
		assignmentListBox.listBox.set_enabled(true);
		string currentAxis = buttonData.ActionsToString();
		SetAssignedAxis(assignmentListBox, currentAxis, possibleAxes);
	}

	private void DeactivateReassignmentListBox(InputAssignmentListBox assignmentListBox)
	{
		assignmentListBox.listBox.items.Clear();
		assignmentListBox.listBox.set_enabled(false);
		assignmentListBox.set_enabled(false);
	}

	private void SetAssignedActions(InputAssignmentButton assignmentButton, KeyboardKeyCode keyCode)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if ((int)keyCode == 0)
		{
			assignmentButton.inputText.set_text(StringTableBase<StringTable>.Instance.GetString(unassignedButtonText));
		}
		else
		{
			assignmentButton.inputText.set_text(StringTableBase<StringTable>.Instance.GetKeyboardKeyString(keyCode));
		}
	}

	private void SetAssignedAxis(InputAssignmentListBox assignmentListBox, string currentAxis, FasterList<string> options)
	{
		assignmentListBox.listBox.items.Clear();
		assignmentListBox.listBox.items.Add(StringTableBase<StringTable>.Instance.GetString(unassignedListboxText));
		assignmentListBox.listBox.items.AddRange((IEnumerable<string>)options);
		if (string.IsNullOrEmpty(currentAxis))
		{
			assignmentListBox.initialValue = assignmentListBox.listBox.items[0];
			assignmentListBox.listBox.set_value(assignmentListBox.listBox.items[0]);
		}
		else
		{
			assignmentListBox.initialValue = currentAxis;
			assignmentListBox.listBox.set_value(currentAxis);
		}
	}

	private InputAssignmentRow CreateAssignmentRow(int buttonIdOffset, int categoryId, string actionName, int localYPos)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = gameObjectPool.Use(actionMappingRowTemplate.get_name(), (Func<GameObject>)(() => OnGameObjectAllocated(actionMappingRowTemplate)));
		_rows.Add(val);
		val.get_transform().set_parent(actionMappingRowTemplate.get_transform().get_parent());
		Vector3 localPosition = actionMappingRowTemplate.get_transform().get_localPosition();
		localPosition.y = localYPos;
		val.get_transform().set_localPosition(localPosition);
		val.get_transform().set_localScale(Vector3.get_one());
		val.SetActive(true);
		InputAssignmentRow component = val.GetComponent<InputAssignmentRow>();
		component.keyboard.buttonId = buttonIdOffset;
		component.mouseAxis.buttonId = buttonIdOffset + 1;
		component.joypadAxis.buttonId = buttonIdOffset + 2;
		component.keyboard.categoryId = categoryId;
		component.mouseAxis.categoryId = categoryId;
		component.joypadAxis.categoryId = categoryId;
		return component;
	}

	private CategoryDivider CreateCategoryDivider(int localYPos)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		GameObject val = gameObjectPool.Use(categoryDividerTemplate.get_name(), (Func<GameObject>)(() => OnGameObjectAllocated(categoryDividerTemplate)));
		_rows.Add(val);
		val.get_transform().set_parent(actionMappingRowTemplate.get_transform().get_parent());
		Vector3 localPosition = actionMappingRowTemplate.get_transform().get_localPosition();
		localPosition.y = localYPos + _categoryDividerOffset;
		val.get_transform().set_localPosition(localPosition);
		val.get_transform().set_localScale(Vector3.get_one());
		val.SetActive(true);
		return val.GetComponent<CategoryDivider>();
	}

	private void SetSlidingPanelOffset(float value)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		int num = _mappingRowHeight * _rows.get_Count();
		float num2 = ((float)num - _maxWindowSize) * value;
		Vector3 localPosition = _slidingPanel.get_localPosition();
		localPosition.y = num2 + _initialHeight;
		_slidingPanel.set_localPosition(localPosition);
	}
}
