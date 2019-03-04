using Svelto.ECS;

namespace Mothership.RobotConfiguration
{
	public interface IRobotControlCustomisationsComponent
	{
		DispatchOnChange<ControlType> controlTypeDropDownChosen
		{
			get;
		}

		DispatchOnChange<bool> cameraRelativeTiltCheckboxSet
		{
			get;
		}

		DispatchOnChange<bool> tankTracksTurnToFaceCheckboxSet
		{
			get;
		}

		DispatchOnChange<bool> sideWaysDrivingCheckboxSet
		{
			get;
		}

		DispatchOnChange<bool> mothershipSkinTabPressed
		{
			get;
		}

		DispatchOnChange<bool> spawnEffectsTabPressed
		{
			get;
		}

		DispatchOnChange<bool> deathEffectsTabPressed
		{
			get;
		}

		bool cameraRelativeTiltCheckbox
		{
			set;
		}

		bool tankTracksTurnToFaceCheckbox
		{
			set;
		}

		bool sideWaysDrivingCheckbox
		{
			set;
		}

		ControlType controlTypeInDropDown
		{
			set;
		}
	}
}
