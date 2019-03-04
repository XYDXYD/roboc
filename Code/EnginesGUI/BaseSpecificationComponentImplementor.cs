using UnityEngine;

namespace EnginesGUI
{
	public class BaseSpecificationComponentImplementor<T> : MonoBehaviour where T : class
	{
		public static string DEFAULT_CONFIG_NAME = "Default";

		public T[] ConfigToApply;

		public string[] RuleNames;

		public GameObject PanelTarget;

		public int TargetPanelInstanceId => PanelTarget.GetComponent<UIPanel>().GetInstanceID();

		public BaseSpecificationComponentImplementor()
			: this()
		{
			if (RuleNames == null || ConfigToApply == null)
			{
				ConfigToApply = new T[1];
				RuleNames = new string[1]
				{
					DEFAULT_CONFIG_NAME
				};
			}
			if (RuleNames.Length == 0 || ConfigToApply.Length == 0)
			{
				ConfigToApply = new T[1];
				RuleNames = new string[1]
				{
					DEFAULT_CONFIG_NAME
				};
			}
		}

		public void Awake()
		{
			if (!(PanelTarget == null))
			{
				return;
			}
			Transform val = this.get_gameObject().get_transform();
			ScreenConfigPresetsEntityDescriptorHolder component;
			while (true)
			{
				if (val != null)
				{
					component = val.get_gameObject().GetComponent<ScreenConfigPresetsEntityDescriptorHolder>();
					if (component != null)
					{
						break;
					}
					val = val.get_gameObject().get_transform().get_parent();
					continue;
				}
				return;
			}
			PanelTarget = component.get_gameObject();
		}

		public bool ConfigurationForRule(string ruleName, out T configResult)
		{
			configResult = (T)null;
			int num = 0;
			string[] ruleNames = RuleNames;
			foreach (string text in ruleNames)
			{
				if (text.CompareTo(ruleName) == 0)
				{
					configResult = ConfigToApply[num];
					return true;
				}
				num++;
			}
			return false;
		}
	}
}
