using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership
{
	internal class CubeCategoryTooltip : MonoBehaviour
	{
		public float openWaitTime = 2f;

		public GameObject buttonToolTip;

		private bool _forceHide;

		public CubeCategoryTooltip()
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
	}
}
