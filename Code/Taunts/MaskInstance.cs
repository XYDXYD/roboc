using System.Collections.Generic;
using UnityEngine;

namespace Taunts
{
	public class MaskInstance
	{
		public Vector3 MaskAnchorLocation;

		public string MaskGroupName;

		public MaskOrientation MaskOrientation;

		public List<uint> PartsMissing = new List<uint>();

		public MaskInstance(Vector3 anchorLocation_, string groupName_, MaskOrientation maskOrientationInformation_)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			MaskAnchorLocation = anchorLocation_;
			MaskGroupName = groupName_;
			MaskOrientation = maskOrientationInformation_;
		}
	}
}
