using UnityEngine;

namespace Mothership
{
	public class SearchClansView : ClanSectionViewBase
	{
		[SerializeField]
		protected GameObject ClanListTemplateGO;

		[SerializeField]
		protected GameObject ClanListItemTemplateGO;

		[SerializeField]
		protected GameObject ClanListContainerAreaGO;

		[SerializeField]
		protected GameObject SearchTextEntryTemplateGO;

		[SerializeField]
		protected GameObject SearchTextEntryContainerAreaGO;

		[SerializeField]
		protected GameObject LoadMoreButtonGO;

		internal override ClanSectionType SectionType => ClanSectionType.SearchClan;

		public GameObject ClanListTemplate => ClanListTemplateGO;

		public GameObject ClanListItemTemplate => ClanListItemTemplateGO;

		public GameObject ClanListContainerArea => ClanListContainerAreaGO;

		public GameObject SearchTextEntryContainerArea => SearchTextEntryContainerAreaGO;

		public GameObject SearchTextEntryTemplate => SearchTextEntryTemplateGO;

		public GameObject LoadMoreButtonTemplate => LoadMoreButtonGO;
	}
}
