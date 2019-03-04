using UnityEngine;

namespace Simulation
{
	internal sealed class BaseBeamTeamSwitcher : MonoBehaviour
	{
		public GameObject friendlyBeam;

		public GameObject enemyBeam;

		public BaseBeamTeamSwitcher()
			: this()
		{
		}

		public void SwitchBeam(bool myTeam)
		{
			friendlyBeam.SetActive(myTeam);
			enemyBeam.SetActive(!myTeam);
		}
	}
}
