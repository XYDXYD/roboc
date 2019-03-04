public enum AudioFabricGameEvents
{
	TransitionTeleTo,
	TransitionTeleFrom,
	UIMenuOpen,
	UIMenuClosed,
	UIButtonSelect,
	UIButtonError,
	UIButtonHover,
	UIAddToBasket,
	UIRemoveFromBasket,
	UICancelPurchase,
	UIConfirmPurchase,
	UIPurchased,
	UIShopOpen,
	UIShopClosed,
	UITooManyCubes,
	CubePlace,
	CubeDelete,
	CubeRotate,
	LobbyCountdownQuiet,
	LobbyCountdownLoud,
	SubMenuSwap,
	ProtoniumCubeHit,
	ProtoniumCubeDestroyed,
	TeamBaseLoop,
	TeamBaseLoopDamaged,
	Supernova,
	FusionShieldEnter,
	FusionShieldOn,
	FusionShieldOff,
	FusionShieldOnVO,
	FusionShieldOffVO,
	TeamBaseUnderAttack,
	TeamBaseLowHealthVOAlly,
	TeamBaseLowHealthVOEnemy,
	TFTCapturedVO,
	MFTCapturedVO,
	BFTCapturedVO,
	UIRent,
	TFTUnderAttackVO,
	MFTUnderAttackVO,
	BFTUnderAttackVO,
	UI_Dropdown,
	TFTEnemyCapturedVO,
	MFTEnemyCapturedVO,
	BFTEnemyCapturedVO,
	UI_Dropdown_Sel,
	FirstFusionTowerCaptured,
	FusionShieldExit,
	UI_Dropdown_Close,
	ClockCyclesBoosterEnabledVO,
	SecondFusionTowerCaptured,
	ThirdFusionTowerCaptured,
	Gui_LevelUp_Close,
	SecondFusionTowerUncaptured,
	Pit_End_Victory,
	Pit_End_Defeated,
	ClockCyclesBoosterDisabledVO,
	TeamMemberDisconnected,
	TeamMemberReconnected,
	GUI_KubeMenuHover_Left,
	GUI_KubeMenuHover_Top,
	GUI_ScrollSlider,
	BUILD_Kube_Impossible,
	BUILD_Robot_Detatched,
	BUILD_KubeCopy,
	ProtCore_PreSpawnNot,
	ProtCore_CoreSpawned,
	ProtCore_AllyHasCore,
	ProtCore_EnemyHasCore,
	ProtCore_AllySpawnOPBot,
	ProtCore_AllySpawnMegaBot,
	ProtCore_EnemySpawnOPBot,
	ProtCore_EnemySpawnMegaBot,
	UIButtonLocked,
	LobbyCountdown03to00,
	LobbyCountdown10to04,
	LobbyCountdown30to11,
	WeaponZoomIn,
	WeaponZoomOut,
	ToolTipAppear,
	ToolTipDisappear,
	ToolTipEyeAppear,
	ToolTipEyeDisappear,
	CubesDestroyed_1_5,
	CubesDestroyed_6_14,
	CubesDestroyed_15_35,
	CubesDestroyed_36_59,
	CubesDestroyed_60,
	ETFTEnemyCapturedVO,
	EMFTEnemyCapturedVO,
	EBFTEnemyCapturedVO,
	TFTFirstCaptureVO,
	BFTFirstCaptureVO,
	MFTFirstCaptureVO,
	EnemyFusionShieldOffVO,
	CrystalMultiDestroyed,
	Color_Select,
	Color_Kube,
	PaintMode_Enter,
	PaintMode_Exit,
	Color_AllRobot,
	Color_Miss,
	Color_Beam_Start,
	Color_Beam_Reverse,
	Color_Beam_Loop,
	Color_Impact,
	Color_Locked,
	GUI_Autoregen_Countdown,
	GUI_Healed100,
	GUI_SelfHeal_5sec,
	GUI_SelfHeal_Canceled,
	GUI_Garage_Arrow,
	GUI_AutoRegen,
	GUI_AutoRegen2c,
	WeaponSwitch_AeroFlak,
	WeaponSwitch_Laser,
	WeaponSwitch_Nano,
	WeaponSwitch_Plasma,
	WeaponSwitch_Rail,
	WeaponSwitch_Tesla,
	WeaponSwitch_RocketLauncher,
	GUI_WeaponDestroyed,
	WeaponSwitch_Destroyed,
	WeaponSwitch_Seeker,
	WeaponSwitch_IonDistorter,
	UI_Cube_Forge,
	UI_Cube_Recycle,
	TeamMemberDisconnected_TDM,
	WeaponSwitch_ChainGun,
	GUI_Clan_Create,
	GUI_Clan_Request_Accepted,
	GUI_Clan_Request_Declined,
	GUI_Clan_Request_Received,
	GUI_Clan_Request_Sent,
	GUI_Friend_Request_Accepted,
	GUI_Friend_Request_Declined,
	GUI_Friend_Request_Received,
	GUI_Friend_Request_Sent,
	GUI_Party_Request_Pending,
	GUI_Party_Request_Accepted,
	GUI_Party_Request_Declined,
	GUI_Party_Request_Received,
	GUI_Party_Request_Sent,
	VO_RBA_Intro,
	VO_RBA_MT_E_Captured,
	VO_RBA_MT_Captured,
	VO_RBA_MT_UAttack,
	VO_RBA_FT_E_Captured,
	VO_RBA_FT_Captured,
	VO_RBA_FT_UAttack,
	VO_RBA_NT_E_Captured,
	VO_RBA_NT_Captured,
	VO_RBA_NT_UAttack,
	VO_RBA_PR_UAttack,
	VO_RBA_BS_Down,
	VO_RBA_BS_E_Down,
	VO_RBA_EQ_Warning,
	VO_RBA_EQ_DEF_Activate,
	VO_RBA_EQ_ATT_Activate,
	VO_RBA_EQ_Deactivated,
	VO_RBA_EQ_Destroyed,
	VO_RBA_EQ_Failed,
	VO_RBA_EQ_Defended,
	VO_RBA_ANN_Charged50,
	VO_RBA_ANN_E_Charged50,
	VO_RBA_ANN_Charged75,
	VO_RBA_ANN_E_Charged75,
	VO_RBA_ANN_Charged100,
	VO_RBA_ANN_E_Charged100,
	VO_RBA_Victory,
	VO_RBA_Defeat,
	VO_RBA_Disconnect,
	VO_RBA_Reconnect,
	VO_RBA_PlayerReturn,
	VO_RBA_Dominating,
	VO_TDM_Intro,
	VO_RBA_Draw,
	VO_RBA_TeamDisconnected_TeamBuff,
	VO_RBA_EnemyDisconnected_EnemyTeamBuff,
	VO_RBA_TeamBuff_Removed,
	VO_RBA_EnemyDisconnected,
	VO_RBA_TeamDisconnected,
	WeaponSwitch_Mortar
}
