public class AudioFabricEvent
{
	public static string[] StringEvents = new string[183]
	{
		"KUB_DEMO_fabric_GUI_TransitionTeleTO",
		"KUB_DEMO_fabric_GUI_TransitionTeleFROM",
		"KUB_DEMO_fabric_GUI_KubeMenuOpen",
		"KUB_DEMO_fabric_GUI_KubeMenuClose",
		"GUI_Click",
		"KUB_DEMO_fabric_GUI_KubeMenuError",
		"KUB_DEMO_fabric_GUI_KubeMenuHover",
		"KUB_DEMO_fabric_GUI_KubeMenuAddToBasket",
		"KUB_DEMO_fabric_GUI_KubeMenuRemoveFromBasket",
		"KUB_DEMO_fabric_GUI_KubeMenuCancelPurchase",
		"KUB_DEMO_fabric_GUI_KubeMenuConfirmPurchase",
		"KUB_DEMO_fabric_GUI_KubeMenuPurchased",
		"KUB_DEMO_fabric_GUI_KubeShopOpen",
		"KUB_DEMO_fabric_GUI_KubeShopClose",
		"KUB_DEMO_fabric_GUI_KubeTooManyUsed",
		"KUB_DEMO_fabric_build_KubePlace",
		"KUB_DEMO_fabric_build_KubeDelete",
		"KUB_DEMO_fabric_build_KubeRotate",
		"KUB_DEMO_fabric_GUI_LobbyCountdown_Quiet",
		"KUB_DEMO_fabric_GUI_LobbyCountdown_Loud",
		"KUB_DEMO_fabric_GUI_KubeMenuSubMenuSwap",
		"Crystal_Hit",
		"Crystal_Destroyed",
		"Crystal_Loop",
		"Crystal_Loop_Damaged",
		"SuperNova_Expl",
		"FusionShield_Enter",
		"FusionShield_ON",
		"FusionShield_OFF",
		"VO_Gameplay_VO_FS_Active",
		"VO_Gameplay_VO_FS_Deactivated",
		"VO_Gameplay_VO_PC_UAttack",
		"VO_Gameplay_VO_PC_NDestroyed",
		"VO_Gameplay_VO_EPC_NDestroyed",
		"VO_Gameplay_VO_TFT_Captured",
		"VO_Gameplay_VO_MFT_Captured",
		"VO_Gameplay_VO_BFT_Captured",
		"GUI_Rent",
		"VO_Gameplay_VO_TFT_UAttack",
		"VO_Gameplay_VO_MFT_UAttack",
		"VO_Gameplay_VO_BFT_UAttack",
		"GUI_Dropdown",
		"VO_Gameplay_VO_ETFT_First_Enemy_Captured",
		"VO_Gameplay_VO_EMFT_First_Enemy_Captured",
		"VO_Gameplay_VO_EBFT_First_Enemy_Captured",
		"GUI_Dropdown_Sel",
		"FusionTower1_ShieldActive",
		"FusionShield_Exit",
		"GUI_Dropdown_Close",
		"VO_Gameplay_VO_CCB_Activated",
		"FusionTower2_CCBooster_ON",
		"FusionTower4_EnemyShieldDeactivate",
		"KUB_DEMO_fabric_pickup_gui_levelup_close",
		"FusionTower2_CCBooster_OFF",
		"GUI_Player_Team_Battle_Victory",
		"GUI_Player_Team_Battle_Defeated",
		"VO_Gameplay_VO_CCB_Deactivated",
		"VO_Gameplay_VO_Player_Disconnected",
		"VO_Gameplay_VO_Player_Reconnected",
		"GUI_KubeMenuHover_Left",
		"GUI_KubeMenuHover_Top",
		"GUI_ScrollSlider",
		"BUILD_Kube_Impossible",
		"BUILD_Robot_Detatched",
		"BUILD_KubeCopy",
		"VO_Gameplay_VO_ProtCore_60Sec",
		"VO_Gameplay_VO_ProtCore_Appeared",
		"VO_Gameplay_VO_ProtCore_Allies",
		"VO_Gameplay_VO_ProtCore_Enemy",
		"VO_Gameplay_VO_Allies_Spawn_OPRobot",
		"VO_Gameplay_VO_Allies_Spawn_Megabot",
		"VO_Gameplay_VO_Enemy_Spawn_OPRobot",
		"VO_Gameplay_VO_Enemy_Spawn_Megabot",
		"GUI_Click_Locked",
		"GUI_lobby_countdown_03to00",
		"GUI_lobby_countdown_10to04",
		"GUI_lobby_countdown_30to11",
		"Ingame_ZoomIn",
		"Ingame_ZoomOut",
		"GUI_Tooltip_Appear",
		"GUI_Tooltip_Disappear",
		"KUB_DEMO_fabric_GUI_HelpTickerAppear",
		"KUB_DEMO_fabric_GUI_HelpTickerDisappear",
		"1to5CubesDestroyed",
		"6to14CubesDestroyed",
		"15to35CubesDestroyed",
		"36to59CubesDestroyed",
		"MoreThan60CubesDestroyed",
		"VO_Gameplay_VO_ETFT_Captured",
		"VO_Gameplay_VO_EMFT_Captured",
		"VO_Gameplay_VO_EBFT_Captured",
		"VO_Gameplay_VO_ETFT_First_Captured",
		"VO_Gameplay_VO_EBFT_First_Captured",
		"VO_Gameplay_VO_EMFT_First_Captured",
		"VO_Gameplay_VO_FS_EDeactivated",
		"Crystal_MultiDestroyed",
		"GUI_Color_Select",
		"GUI_Color_Kube",
		"GUI_PaintMode_Enter",
		"GUI_PaintMode_Exit",
		"GUI_Color_AllRobot",
		"GUI_Color_Miss",
		"GUI_Color_Beam_Start",
		"GUI_Color_Beam_Reverse",
		"GUI_Color_Beam_Loop",
		"GUI_Color_Impact",
		"GUI_Color_Reset",
		"GUI_AutoRegen_CountDown",
		"GUI_Healed100",
		"GUI_SelfHeal_5sec",
		"GUI_SelfHeal_Canceled",
		"GUI_Garage_Arrow",
		"GUI_AutoRegen",
		"GUI_AutoRegen2c",
		"WeaponSwitch_AeroFlak",
		"WeaponSwitch_Laser",
		"WeaponSwitch_Nano",
		"WeaponSwitch_Plasma",
		"WeaponSwitch_Rail",
		"WeaponSwitch_Tesla",
		"WeaponSwitch_RocketLauncher",
		"GUI_WeaponDestroyed",
		"WeaponSwitch_Destroyed",
		"WeaponSwitch_Seeker",
		"WeaponSwitch_IonDistorter",
		"Cube_Forge",
		"Cube_Recycle",
		"VO_Gameplay_VO_Player_Disconnected_TDM",
		"WeaponSwitch_ChainGun",
		"GUI_Clan_Create",
		"GUI_Clan_Request_Accepted",
		"GUI_Clan_Request_Declined",
		"GUI_Clan_Request_Received",
		"GUI_Clan_Request_Sent",
		"GUI_Friend_Request_Accepted",
		"GUI_Friend_Request_Declined",
		"GUI_Friend_Request_Received",
		"GUI_Friend_Request_Sent",
		"GUI_Party_Request_Pending",
		"GUI_Party_Request_Accepted",
		"GUI_Party_Request_Declined",
		"GUI_Party_Request_Received",
		"GUI_Party_Request_Sent",
		"VO_RBA_Intro",
		"VO_RBA_MT_E_Captured",
		"VO_RBA_MT_Captured",
		"VO_RBA_MT_UAttack",
		"VO_RBA_FT_E_Captured",
		"VO_RBA_FT_Captured",
		"VO_RBA_FT_UAttack",
		"VO_RBA_NT_E_Captured",
		"VO_RBA_NT_Captured",
		"VO_RBA_NT_UAttack",
		"VO_RBA_PR_UAttack",
		"VO_RBA_BS_Down",
		"VO_RBA_BS_E_Down",
		"VO_RBA_EQ_Warning",
		"VO_RBA_EQ_DEF_Activate",
		"VO_RBA_EQ_ATT_Activate",
		"VO_RBA_EQ_Deactivated",
		"VO_RBA_EQ_Destroyed",
		"VO_RBA_EQ_Failed",
		"VO_RBA_EQ_Defended",
		"VO_RBA_ANN_Charged50",
		"VO_RBA_ANN_E_Charged50",
		"VO_RBA_ANN_Charged75",
		"VO_RBA_ANN_E_Charged75",
		"VO_RBA_ANN_Charged100",
		"VO_RBA_ANN_E_Charged100",
		"VO_RBA_Victory",
		"VO_RBA_Defeat",
		"VO_RBA_Disconnect",
		"VO_RBA_Reconnect",
		"VO_RBA_PlayerReturn",
		"VO_RBA_Dominating",
		"VO_TDM_Intro",
		"VO_RBA_Draw",
		"VO_RBA_TeamBuff",
		"VO_RBA_TeamBuff_E",
		"VO_RBA_TeamBuff_Removed",
		"VO_RBA_TeamAllyBuff_E",
		"VO_RBA_TeamEnemyBuff",
		"WeaponSwitch_Mortar"
	};

	public static string Name(AudioFabricGameEvents eventEnum)
	{
		return StringEvents[(int)eventEnum];
	}
}
