using System.Collections;

namespace Mothership
{
	internal sealed class ReviewRequestController_Tencent : IReviewRequestController
	{
		public bool ConditionalShowReviewRequest()
		{
			return false;
		}

		public IEnumerator LoadGUIData()
		{
			yield break;
		}

		public bool IsActive()
		{
			return false;
		}

		public void Show()
		{
		}

		public void Hide()
		{
		}
	}
}
