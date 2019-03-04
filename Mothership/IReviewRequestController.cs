using System.Collections;

namespace Mothership
{
	internal interface IReviewRequestController
	{
		bool ConditionalShowReviewRequest();

		IEnumerator LoadGUIData();

		bool IsActive();

		void Show();

		void Hide();
	}
}
