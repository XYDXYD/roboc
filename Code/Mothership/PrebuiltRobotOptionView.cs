using Svelto.DataStructures;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal class PrebuiltRobotOptionView : MonoBehaviour
	{
		[SerializeField]
		private UILocalize OptionUILocalise;

		private int _index;

		private FasterList<PrebuiltRobotOption> _options = new FasterList<PrebuiltRobotOption>();

		private BubbleSignal<IChainRoot> _bubbleSignal;

		public PrebuiltRobotOption selectedOption => _options.get_Item(_index);

		public PrebuiltRobotOptionView()
			: this()
		{
		}

		private void Start()
		{
			_bubbleSignal = new BubbleSignal<IChainRoot>(this.get_transform());
		}

		public void SetOptions(FasterList<PrebuiltRobotOption> options)
		{
			_options = options;
			ResetOptions();
		}

		public void ResetOptions()
		{
			_index = 0;
			OptionUILocalise.key = selectedOption.strKey;
			OptionUILocalise.set_value(StringTableBase<StringTable>.Instance.GetString(selectedOption.strKey));
		}

		public void Previous()
		{
			if (_index == 0)
			{
				_index = _options.get_Count();
			}
			_index = (_index - 1) % _options.get_Count();
			ShowSelectedOption();
		}

		public void Next()
		{
			_index = (_index + 1) % _options.get_Count();
			ShowSelectedOption();
		}

		private void ShowSelectedOption()
		{
			OptionUILocalise.set_value(StringTableBase<StringTable>.Instance.GetString(selectedOption.strKey));
			_bubbleSignal.Dispatch<PrebuiltRobotOption>(selectedOption);
		}
	}
}
