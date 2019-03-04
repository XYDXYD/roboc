using Fabric;
using Svelto.DataStructures;
using System;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class MechLegAudioManager
	{
		private static readonly Dictionary<int, string> startStopSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_Start_Stop"
			},
			{
				5,
				"MechLegs_T5_Start_Stop"
			},
			{
				12,
				"SprinterLegs_T1_Start_Stop"
			},
			{
				13,
				"SprinterLegs_T2_Start_Stop"
			},
			{
				9,
				"MechLegs_T9_Start_Stop"
			},
			{
				11,
				"MechLegs_TX_Start_Stop"
			}
		};

		private static readonly Dictionary<int, string> longJumpLegMovementSounds = new Dictionary<int, string>
		{
			{
				12,
				"SprinterLegs_T1_LegMovement"
			},
			{
				13,
				"SprinterLegs_T2_LegMovement"
			}
		};

		private static readonly Dictionary<int, string> startLongJumpSounds = new Dictionary<int, string>
		{
			{
				12,
				"SprinterLegs_T1_LongJump"
			},
			{
				13,
				"SprinterLegs_T2_LongJump"
			}
		};

		private static readonly Dictionary<int, string> footstepSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_Footsteps"
			},
			{
				5,
				"MechLegs_T5_Footsteps"
			},
			{
				12,
				"SprinterLegs_T1_Footsteps"
			},
			{
				13,
				"SprinterLegs_T2_Footsteps"
			},
			{
				9,
				"MechLegs_T9_Footsteps"
			},
			{
				11,
				"MechLegs_TX_Footsteps"
			}
		};

		private static readonly Dictionary<int, string> footstepSoundsEnemy = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_Footsteps_Enemy"
			},
			{
				5,
				"MechLegs_T5_Footsteps_Enemy"
			},
			{
				12,
				"SprinterLegs_T1_Footsteps_Enemy"
			},
			{
				13,
				"SprinterLegs_T2_Footsteps_Enemy"
			},
			{
				14,
				"SprinterLegs_T3_Footsteps_Enemy"
			},
			{
				9,
				"MechLegs_T9_Footsteps_Enemy"
			},
			{
				11,
				"MechLegs_TX_Footsteps_Enemy"
			}
		};

		private static readonly Dictionary<int, string> crouchFootstepSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_CrouchFootsteps"
			},
			{
				5,
				"MechLegs_T5_CrouchFootsteps"
			},
			{
				12,
				"SprinterLegs_T1_CrouchFootsteps"
			},
			{
				13,
				"SprinterLegs_T2_CrouchFootsteps"
			},
			{
				9,
				"MechLegs_T9_CrouchFootsteps"
			},
			{
				11,
				"MechLegs_TX_CrouchFootsteps"
			}
		};

		private static readonly Dictionary<int, string> walkLoopSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_Loop"
			},
			{
				5,
				"MechLegs_T5_Loop"
			},
			{
				12,
				"SprinterLegs_T1_Loop"
			},
			{
				13,
				"SprinterLegs_T2_Loop"
			},
			{
				9,
				"MechLegs_T9_Loop"
			},
			{
				11,
				"MechLegs_TX_Loop"
			}
		};

		private static readonly Dictionary<int, string> jumpingSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_Jump"
			},
			{
				5,
				"MechLegs_T5_Jump"
			},
			{
				12,
				"SprinterLegs_T1_Jump"
			},
			{
				13,
				"SprinterLegs_T2_Jump"
			},
			{
				9,
				"MechLegs_T9_Jump"
			},
			{
				11,
				"MechLegs_TX_Jump"
			}
		};

		private static readonly Dictionary<int, string> landingSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_Land"
			},
			{
				5,
				"MechLegs_T5_Land"
			},
			{
				12,
				"SprinterLegs_T1_Land"
			},
			{
				13,
				"SprinterLegs_T2_Land"
			},
			{
				9,
				"MechLegs_T9_Land"
			},
			{
				11,
				"MechLegs_TX_Land"
			}
		};

		private static readonly Dictionary<int, string> slidingSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_Slide"
			},
			{
				5,
				"MechLegs_T5_Slide"
			},
			{
				12,
				"SprinterLegs_T1_Slide"
			},
			{
				13,
				"SprinterLegs_T2_Slide"
			},
			{
				9,
				"MechLegs_T9_Slide"
			},
			{
				11,
				"MechLegs_TX_Slide"
			}
		};

		private static readonly Dictionary<int, string> skidLoopSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_Slide_Loop"
			},
			{
				5,
				"MechLegs_T5_Slide_Loop"
			},
			{
				12,
				"SprinterLegs_T1_Slide_Loop"
			},
			{
				13,
				"SprinterLegs_T2_Slide_Loop"
			},
			{
				9,
				"MechLegs_T9_Slide_Loop"
			},
			{
				11,
				"MechLegs_TX_Slide_Loop"
			}
		};

		private static readonly Dictionary<int, string> crouchSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_Crouch"
			},
			{
				5,
				"MechLegs_T5_Crouch"
			},
			{
				12,
				"SprinterLegs_T1_Crouch"
			},
			{
				13,
				"SprinterLegs_T2_Crouch"
			},
			{
				9,
				"MechLegs_T9_Crouch"
			},
			{
				11,
				"MechLegs_TX_Crouch"
			}
		};

		private static readonly Dictionary<int, string> unCrouchSounds = new Dictionary<int, string>
		{
			{
				1,
				"MechLegs_T1_UnCrouch"
			},
			{
				5,
				"MechLegs_T5_UnCrouch"
			},
			{
				12,
				"SprinterLegs_T1_UnCrouch"
			},
			{
				13,
				"SprinterLegs_T2_UnCrouch"
			},
			{
				9,
				"MechLegs_T9_UnCrouch"
			},
			{
				11,
				"MechLegs_TX_UnCrouch"
			}
		};

		private GameObject _soundObj;

		private int _avgTier = -1;

		private string _mainLoopingSound;

		private string _skidLoopingSound;

		private bool _isMoving;

		private bool _isAtMaxSpeed;

		private bool _isJumping;

		private bool _isLongJumping;

		private bool _isLongJumpDescending;

		private bool _isLanded;

		private bool _isSkidding;

		private bool _isCrouching;

		private int _lastFootstepSyncGroupPlayed = -1;

		private bool _isLocalPlayer;

		public MechLegAudioManager(GameObject robotObj, bool isLocalPlayer)
		{
			_isLocalPlayer = isLocalPlayer;
			_soundObj = robotObj.GetComponentInChildren<MachineCenter>().get_gameObject();
		}

		public void StopAllLoops()
		{
			if (EventManager.get_Instance() != null)
			{
				StopMovingLoop();
				StopSkiddingLoop();
			}
		}

		public void PlayStartStopMovementSound(GameObject soundObject)
		{
			try
			{
				EventManager.get_Instance().PostEvent(startStopSounds[_avgTier], 0, (object)null, soundObject);
			}
			catch (Exception arg)
			{
				Console.Log("Sometimes Fabric throws error, maybe due to the update. " + arg);
			}
		}

		public void PlayNormalFootstepSound(CubeMechLeg leg, int numSyncGroups, int numLegs)
		{
			string soundEffect = (!_isLocalPlayer) ? footstepSoundsEnemy[_avgTier] : footstepSounds[_avgTier];
			PlayFootstepSound(leg, soundEffect, numSyncGroups, numLegs);
		}

		public void PlayCrouchFootstepSound(CubeMechLeg leg, int numSyncGroups, int numLegs)
		{
			PlayFootstepSound(leg, crouchFootstepSounds[_avgTier], numSyncGroups, numLegs);
		}

		public void UpdateLoopingValues(FasterList<CubeMechLeg> legs)
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			bool flag6 = false;
			bool flag7 = false;
			bool flag8 = false;
			float num = 0f;
			for (int i = 0; i < legs.get_Count(); i++)
			{
				CubeMechLeg cubeMechLeg = legs.get_Item(i);
				MechLegData legData = cubeMechLeg.legData;
				MechLegGraphics legGraphics = cubeMechLeg.legGraphics;
				if (!(cubeMechLeg.T == null))
				{
					flag |= (legData.legGrounded && (!legData.stopped || (legGraphics.isAnimating && !legGraphics.quietAnimation)));
					flag2 |= legData.jumping;
					flag3 |= (legData.legGrounded && legGraphics.justLanded);
					flag4 |= legData.legSliding;
					flag5 |= legData.crouching;
					flag6 |= legData.longJumping;
					flag7 |= legData.descending;
					flag8 |= (cubeMechLeg.legMovement.longJumpForce > 0f && legData.legGrounded && legData.currentSpeedRatio > 0.8f);
					num += (float)cubeMechLeg.tier;
				}
			}
			_avgTier = Mathf.RoundToInt(num / (float)legs.get_Count());
			UpdateAvgTier();
			if (legs.get_Count() == 0)
			{
				flag = false;
				flag4 = false;
			}
			if (flag6)
			{
				flag2 = false;
			}
			if (flag2 || flag6)
			{
				flag = false;
			}
			UpdateMovingSound(flag, flag8);
			UpdateJumpingSound(flag2);
			UpdateLongJumpingSound(flag6);
			UpdateLongJumpDescendingSound(flag6 && flag7);
			UpdateLandingSound(flag3);
			UpdateSkiddingSound(flag4);
			UpdateCrouchingSound(flag5);
		}

		private void PlayFootstepSound(CubeMechLeg leg, string soundEffect, int numSyncGroups, int numLegs)
		{
			if (numLegs == 1 || (numSyncGroups >= 1 && leg.legGraphics.syncGroup != _lastFootstepSyncGroupPlayed))
			{
				EventManager.get_Instance().PostEvent(soundEffect, 0, (object)null, leg.get_gameObject());
				_lastFootstepSyncGroupPlayed = leg.legGraphics.syncGroup;
			}
		}

		private void StartMovingLoop()
		{
			StopMovingLoop();
			if (_avgTier >= 0)
			{
				_mainLoopingSound = walkLoopSounds[_avgTier];
				PlayStartStopMovementSound(_soundObj.get_gameObject());
				EventManager.get_Instance().PostEvent(_mainLoopingSound, 0, (object)null, _soundObj);
			}
		}

		private void StopMovingLoop()
		{
			if (_mainLoopingSound != null)
			{
				PlayStartStopMovementSound(_soundObj.get_gameObject());
				EventManager.get_Instance().PostEvent(_mainLoopingSound, 1, (object)null, _soundObj);
				_mainLoopingSound = null;
			}
		}

		private void PlayLongJumpDescendingSound()
		{
			EventManager.get_Instance().PostEvent(longJumpLegMovementSounds[_avgTier], 0, (object)null, _soundObj);
		}

		private void PlayJumpingSound()
		{
			PlayStartStopMovementSound(_soundObj.get_gameObject());
			EventManager.get_Instance().PostEvent(jumpingSounds[_avgTier], 0, (object)null, _soundObj);
		}

		private void PlayLongJumpingSound()
		{
			PlayStartStopMovementSound(_soundObj.get_gameObject());
			EventManager.get_Instance().PostEvent(startLongJumpSounds[_avgTier], 0, (object)null, _soundObj);
		}

		private void PlayLandingSound()
		{
			PlayStartStopMovementSound(_soundObj.get_gameObject());
			EventManager.get_Instance().PostEvent(landingSounds[_avgTier], 0, (object)null, _soundObj);
		}

		private void PlayCrouchSound()
		{
			EventManager.get_Instance().PostEvent(crouchSounds[_avgTier], 0, (object)null, _soundObj);
		}

		private void PlayUnCrouchSound()
		{
			EventManager.get_Instance().PostEvent(unCrouchSounds[_avgTier], 0, (object)null, _soundObj);
		}

		private void StartSkiddingLoop()
		{
			StopSkiddingLoop();
			if (_avgTier >= 0)
			{
				_skidLoopingSound = skidLoopSounds[_avgTier];
				EventManager.get_Instance().PostEvent(_skidLoopingSound, 0, (object)null, _soundObj.get_gameObject());
				EventManager.get_Instance().PostEvent(slidingSounds[_avgTier], 0, (object)null, _soundObj.get_gameObject());
			}
		}

		private void StopSkiddingLoop()
		{
			if (_skidLoopingSound != null)
			{
				EventManager.get_Instance().PostEvent(_skidLoopingSound, 1, (object)null, _soundObj.get_gameObject());
				_skidLoopingSound = null;
			}
		}

		private void UpdateMovingSound(bool moving, bool maxSpeed)
		{
			if (!_isMoving && moving)
			{
				_isMoving = moving;
				StartMovingLoop();
			}
			else if (_isMoving && !moving)
			{
				_isMoving = moving;
				StopMovingLoop();
			}
			if (maxSpeed != _isAtMaxSpeed)
			{
				_isAtMaxSpeed = maxSpeed;
				if (_isMoving)
				{
					UpdateMovingLoopParameter();
				}
			}
			_isMoving = moving;
		}

		private void UpdateMovingLoopParameter()
		{
			EventManager.get_Instance().SetParameter(walkLoopSounds[_avgTier], "SPEED", (float)(_isAtMaxSpeed ? 1 : 0), _soundObj);
		}

		private void UpdateLongJumpDescendingSound(bool jumping)
		{
			if (!_isLongJumpDescending && jumping)
			{
				_isLongJumpDescending = jumping;
				PlayLongJumpDescendingSound();
			}
			_isLongJumpDescending = jumping;
		}

		private void UpdateJumpingSound(bool jumping)
		{
			if (!_isJumping && jumping)
			{
				_isJumping = jumping;
				PlayJumpingSound();
			}
			_isJumping = jumping;
		}

		private void UpdateLongJumpingSound(bool jumping)
		{
			if (!_isLongJumping && jumping)
			{
				_isLongJumping = jumping;
				PlayLongJumpingSound();
			}
			_isLongJumping = jumping;
		}

		private void UpdateLandingSound(bool landed)
		{
			if (!_isLanded && landed)
			{
				_isLanded = landed;
				PlayLandingSound();
			}
			_isLanded = landed;
		}

		private void UpdateSkiddingSound(bool skidding)
		{
			if (!_isSkidding && skidding)
			{
				_isSkidding = skidding;
				StartSkiddingLoop();
			}
			else if (_isSkidding && !skidding)
			{
				_isSkidding = skidding;
				StopSkiddingLoop();
			}
			_isSkidding = skidding;
		}

		private void UpdateCrouchingSound(bool crouching)
		{
			if (!_isCrouching && crouching)
			{
				_isCrouching = crouching;
				PlayCrouchSound();
			}
			else if (_isCrouching && !crouching)
			{
				_isCrouching = crouching;
				PlayUnCrouchSound();
			}
			_isCrouching = crouching;
		}

		private void UpdateAvgTier()
		{
			if (_avgTier >= 1 && _avgTier < 5)
			{
				_avgTier = 1;
			}
			else if (_avgTier >= 5 && _avgTier < 8)
			{
				_avgTier = 5;
			}
			else if (_avgTier >= 8 && _avgTier < 10)
			{
				_avgTier = 9;
			}
			else if (_avgTier >= 10 && _avgTier <= 11)
			{
				_avgTier = 11;
			}
			else if (_avgTier > 11 && _avgTier < 13)
			{
				_avgTier = 12;
			}
			else if (_avgTier >= 13 && _avgTier < 14)
			{
				_avgTier = 13;
			}
			else if (_avgTier >= 14)
			{
				_avgTier = 14;
			}
		}
	}
}
