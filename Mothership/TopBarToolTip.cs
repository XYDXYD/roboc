using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal class TopBarToolTip : MonoBehaviour
	{
		public float openWaitTime = 5f;

		public GameObject buttonToolTip;

		public UILabel buttonToolTipLabel;

		public string buttonToolTipStrKey;

		private bool _forceHide;

		public TopBarToolTip()
			: this()
		{
		}

		private void OnDisable()
		{
			buttonToolTip.SetActive(false);
		}

		public void OnHover(bool over)
		{
			if (over)
			{
				_forceHide = false;
				UpdateToolTipString();
				TaskRunner.get_Instance().Run((Func<IEnumerator>)OpenToolTip);
			}
			else
			{
				_forceHide = true;
				buttonToolTip.SetActive(false);
			}
		}

		public void OnClick()
		{
			_forceHide = true;
			buttonToolTip.SetActive(false);
		}

		private IEnumerator OpenToolTip()
		{
			float time = Time.get_time();
			float elapsedTime = 0f;
			bool passedWaitTime = false;
			while (!passedWaitTime)
			{
				elapsedTime += Time.get_time() - time;
				if (!_forceHide && elapsedTime < openWaitTime)
				{
					yield return null;
				}
				else
				{
					passedWaitTime = true;
				}
			}
			if (!_forceHide)
			{
				buttonToolTip.SetActive(true);
			}
		}

		private void UpdateToolTipString()
		{
			string replaceStringWithInputActionKeyMap = StringTableBase<StringTable>.Instance.GetReplaceStringWithInputActionKeyMap(buttonToolTipStrKey);
			buttonToolTipLabel.set_text(replaceStringWithInputActionKeyMap);
		}
	}
}
