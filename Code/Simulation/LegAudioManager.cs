using Fabric;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class LegAudioManager
	{
		private static Dictionary<int, string> startStepSounds = new Dictionary<int, string>
		{
			{
				7,
				"WalkerLeg_ServoStart_T7"
			},
			{
				8,
				"WalkerLeg_ServoStart_T8"
			},
			{
				9,
				"WalkerLeg_ServoStart_T9"
			},
			{
				10,
				"WalkerLeg_ServoStart_T10"
			}
		};

		private static Dictionary<int, string> endStepSounds = new Dictionary<int, string>
		{
			{
				7,
				"WalkerLeg_FootStep_T7"
			},
			{
				8,
				"WalkerLeg_FootStep_T8"
			},
			{
				9,
				"WalkerLeg_FootStep_T9"
			},
			{
				10,
				"WalkerLeg_FootStep_T10"
			}
		};

		private static Dictionary<int, string> moveLoopSounds = new Dictionary<int, string>
		{
			{
				7,
				"WalkerLeg_ServoLoop_T7"
			},
			{
				8,
				"WalkerLeg_ServoLoop_T8"
			},
			{
				9,
				"WalkerLeg_ServoLoop_T9"
			},
			{
				10,
				"WalkerLeg_ServoLoop_T10"
			}
		};

		private static Dictionary<int, string> jumpingSounds = new Dictionary<int, string>
		{
			{
				7,
				"WalkerLeg_Jump_T7"
			},
			{
				8,
				"WalkerLeg_Jump_T8"
			},
			{
				9,
				"WalkerLeg_Jump_T9"
			},
			{
				10,
				"WalkerLeg_Jump_T10"
			}
		};

		private static Dictionary<int, string> landingSounds = new Dictionary<int, string>
		{
			{
				7,
				"WalkerLeg_Land_T7"
			},
			{
				8,
				"WalkerLeg_Land_T8"
			},
			{
				9,
				"WalkerLeg_Land_T9"
			},
			{
				10,
				"WalkerLeg_Land_T10"
			}
		};

		private GameObject _soundObj;

		private int _avgTier = -1;

		private string _loopingSound;

		private bool _wasMoving;

		private bool _wasJumping;

		public LegAudioManager(GameObject robotObj)
		{
			_soundObj = robotObj.GetComponentInChildren<MachineCenter>().get_gameObject();
		}

		public void StartStep(CubeLeg leg)
		{
		}

		public void EndStep(CubeLeg leg)
		{
			EventManager.get_Instance().PostEvent(endStepSounds[leg.tier], 0, (object)null, leg.get_gameObject());
		}

		public void UpdateLoopingValues(List<CubeLeg> legs)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			float num = 0f;
			float num2 = 0f;
			for (int i = 0; i < legs.Count; i++)
			{
				CubeLeg cubeLeg = legs[i];
				LegData legData = cubeLeg.legData;
				LegGraphics legGraphics = cubeLeg.legGraphics;
				if (!(cubeLeg.T == null))
				{
					flag |= (!legData.stopped || (legGraphics.isAnimating && !legGraphics.quietAnimation));
					flag2 |= legData.jumping;
					flag3 |= legData.legGrounded;
					num += legData.currentSpeed;
					num2 += (float)cubeLeg.tier;
				}
			}
			_avgTier = Mathf.RoundToInt(num2 / (float)legs.Count);
			if (flag2)
			{
				PlayJumpAudio();
			}
			else if (_wasJumping && flag3)
			{
				PlayLandAudio();
			}
			else if (flag3 && _wasMoving != flag)
			{
				_wasMoving = flag;
				SetMovingRobot(flag);
			}
		}

		internal void StopSound()
		{
			EndMovingRobot();
		}

		private void SetMovingRobot(bool isMoving)
		{
			if (isMoving)
			{
				StartMovingRobot();
			}
			else
			{
				EndMovingRobot();
			}
		}

		private void StartMovingRobot()
		{
			EndMovingRobot();
			if (_avgTier >= 0)
			{
				_loopingSound = moveLoopSounds[_avgTier];
				EventManager.get_Instance().PostEvent(startStepSounds[_avgTier], 0, (object)null, _soundObj);
				EventManager.get_Instance().PostEvent(_loopingSound, 0, (object)null, _soundObj);
			}
		}

		private void EndMovingRobot()
		{
			if (_loopingSound != null)
			{
				EventManager.get_Instance().PostEvent(_loopingSound, 1, (object)null, _soundObj);
				_loopingSound = null;
			}
		}

		private void PlayJumpAudio()
		{
			if (!_wasJumping)
			{
				EndMovingRobot();
				_wasMoving = false;
				_wasJumping = true;
				EventManager.get_Instance().PostEvent(jumpingSounds[_avgTier], 0, (object)null, _soundObj);
			}
		}

		private void PlayLandAudio()
		{
			if (_wasJumping)
			{
				_wasJumping = false;
				EventManager.get_Instance().PostEvent(landingSounds[_avgTier], 0, (object)null, _soundObj);
			}
		}
	}
}
