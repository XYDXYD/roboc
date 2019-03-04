using UnityEngine;

namespace Mothership.RobotConfiguration
{
	public interface IRobotConfigListDisplayComponent
	{
		GameObject mothershipBaylistItemTemplateGO
		{
			get;
		}

		GameObject spawnEffectslistItemTemplateGO
		{
			get;
		}

		GameObject deathEffectslistItemTemplateGO
		{
			get;
		}

		Transform listParentTransform
		{
			get;
		}

		ListGroupSelection listDisplayMode
		{
			set;
		}
	}
}
