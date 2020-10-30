
{REDUND_ERROR} FUNCTION_BLOCK TransferConveyorStateMgrFBK (*Transfer Conveyor State Manager*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		Enable : BOOL;
		PartPresent : {REDUND_UNREPLICABLE} BOOL;
		CanTransfer : {REDUND_UNREPLICABLE} BOOL;
		SettleTime : {REDUND_UNREPLICABLE} TIME;
		CLOCK_MS : {REDUND_UNREPLICABLE} TIME;
	END_VAR
	VAR_OUTPUT
		StandingStill : {REDUND_UNREPLICABLE} BOOL;
		Settling : {REDUND_UNREPLICABLE} BOOL;
	END_VAR
	VAR
		state : TransferConveyorStateEnum;
		settleTON : {REDUND_UNREPLICABLE} TestableTON;
		zzEdge00000 : BOOL;
		partPresentPosEdge : BOOL;
	END_VAR
END_FUNCTION_BLOCK

FUNCTION_BLOCK TT_Mark_DKFV_CtrlFBK
	VAR_INPUT
		Enable : BOOL;
		PartPresent : BOOL;
		CanTransfer : BOOL;
		ShouldEject : BOOL;
		AtHomeReference : BOOL;
		AtPause : BOOL;
		GateSquaringTime : TIME;
		CoastPastHomeTime : TIME;
		CoastPastPauseTime : TIME;
		PauseTime : TIME;
		CLOCK_MS : TIME;
		State : REFERENCE TO TTCtrlStateType;
		TestPush : BOOL;
		TestHome : BOOL;
		TestPusherFwd : BOOL;
		TestPusherRev : BOOL;
		TestGateUp : BOOL;
		TestConvFwd : BOOL;
	END_VAR
	VAR_OUTPUT
		ConveyorFwd : BOOL;
		GateUp : BOOL;
		PusherFwd : BOOL;
		PusherRev : BOOL;
	END_VAR
	VAR
		InputConfig : TT_Mark_DKFV_InputConfigFBK;
		PusherStateMgr : ChainPushStateMgrFBK;
		OutputConfig : TT_Mark_DKFV_OutputConfigFBK;
	END_VAR
END_FUNCTION_BLOCK

{REDUND_ERROR} FUNCTION_BLOCK TT_Mark_DKFV_InputConfigFBK (*Input logic for the MarkTT Transfer Table*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		Enable : BOOL;
		PartPresent : BOOL;
		CanTransfer : BOOL;
		ShouldEject : BOOL;
		GateSquaringTime : TIME;
		CLOCK_MS : TIME;
		OnHomedEvent : BOOL;
		PushersMovedEvent : BOOL;
		IsHomed : REFERENCE TO BOOL;
	END_VAR
	VAR_OUTPUT
		RequestConveyorFwd : BOOL;
		RequestGateUp : BOOL;
		RequestHome : BOOL;
		RequestPush : BOOL;
	END_VAR
	VAR
		ConveyorStateMgr : TransferConveyorStateMgrFBK;
		PushRequested : BOOL;
		zzEdge00000 : BOOL;
		zzEdge00001 : BOOL;
		zzEdge00002 : BOOL;
		zzEdge00003 : BOOL;
	END_VAR
END_FUNCTION_BLOCK

{REDUND_ERROR} FUNCTION TT_Mark_DKFV_IsClearToSendToFunc : BOOL (*Returns True is Clear To send*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		SentCount : USINT;
		PartPassRecvd : SINT;
		PartPassSent : SINT;
		ChainPusherState : ChainPusherStateEnum;
		PartAtPause : BOOL;
	END_VAR
	VAR
		IsReadyToReceive : BOOL;
	END_VAR
END_FUNCTION

{REDUND_ERROR} FUNCTION_BLOCK TT_Mark_DKFV_MgrFBK (*Wires up TTCtrl to the Part Passing System*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		CLOCK_MS : TIME;
		CoastPastHomeTime : TIME;
		CoastPastPauseTime : TIME;
		Enable : BOOL;
		LegInSentCount : USINT;
		LegOutWaitingFor : UDINT;
		MarkLowerLockSet : LockTypesEnum;
		MarkType : MarkTypesEnum;
		MarkUpperLockSet : LockTypesEnum;
		MyPartPassNext : SINT;
		MyPartPassRecvd : SINT;
		MyPartPassSent : SINT;
		NextPartLockType : LockTypesEnum;
		NextPartSides : SINT;
		PartAtPause : BOOL;
		PartPresent : BOOL;
		PassThroughLockSet : MarkPassThruRollEnum;
		PauseTime : TIME;
		PusherAtHomeRef : BOOL;
		TestPush : BOOL;
		TestHome : BOOL;
		TestPusherFwd : BOOL;
		TestPusherRev : BOOL;
		TestGateUp : BOOL;
		TestConvFwd : BOOL;
		TTCtrlState : REFERENCE TO TTCtrlStateType;
	END_VAR
	VAR_OUTPUT
		CmdFinishedRunning : BOOL;
		CmdTakeSent : BOOL;
		CmdSendRecvd : BOOL;
		ConveyorFwd : BOOL;
		GateUp : BOOL;
		IsReadyToReceive : BOOL;
		PusherFwd : BOOL;
		PusherRev : BOOL;
	END_VAR
	VAR
		PartPauseFilter : InputFilterFBK;
		PartPresentFilter : InputFilterFBK;
		PusherHomeFilter : InputFilterFBK;
		TT_Mark_DKFV_Ctrl : TT_Mark_DKFV_CtrlFBK;
		TTPartPassCtrl : TT_Mark_DKFV_PartPassCtrlFBK;
	END_VAR
END_FUNCTION_BLOCK

{REDUND_ERROR} FUNCTION_BLOCK TT_Mark_DKFV_OutputConfigFBK (*Output config for the Mark TT DKFV*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		CmdConveyorFwd : BOOL;
		CmdGateUp : BOOL;
		Enable : BOOL;
		PusherState : ChainPusherStateEnum;
	END_VAR
	VAR_OUTPUT
		ConveyorFwd : BOOL;
		GateUp : BOOL;
		PusherFwd : BOOL;
	END_VAR
END_FUNCTION_BLOCK

{REDUND_ERROR} FUNCTION_BLOCK TT_Mark_DKFV_PartPassCtrlFBK (*TODO: Add your comment here*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		Enable : BOOL;
		ChainPusherState : ChainPusherStateEnum;
		ShouldEject : BOOL;
		LegInSentCount : USINT;
		PartPassRecvd : SINT;
		PartPresent : BOOL;
	END_VAR
	VAR_OUTPUT
		CmdFinishedRunning : BOOL;
		CmdTakeSent : BOOL;
		CmdSendRecvd : BOOL;
	END_VAR
END_FUNCTION_BLOCK

{REDUND_ERROR} FUNCTION TT_Mark_DKFV_ShouldEject : BOOL (*TODO: Add your comment here*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		PartPassNext : SINT;
		NextPartSides : SINT;
		MarkType : MarkTypesEnum;
		PassThroughLockSet : MarkPassThruRollEnum;
		NextPartLockType : LockTypesEnum;
		MarkLowerLockSet : LockTypesEnum;
		MarkUpperLockSet : LockTypesEnum;
	END_VAR
	VAR
		shouldEject : BOOL;
	END_VAR
END_FUNCTION

FUNCTION_BLOCK TT_TDC_DKFV_CtrlFBK
	VAR_INPUT
		Enable : BOOL;
		PartPresent : BOOL;
		CanTransfer : BOOL;
		AtHomeReference : BOOL;
		AtPause : BOOL;
		GateSquaringTime : TIME;
		CoastPastHomeTime : TIME;
		CoastPastPauseTime : TIME;
		PauseTime : TIME;
		CLOCK_MS : TIME;
		State : REFERENCE TO TTCtrlStateType;
		TestPush : BOOL;
		TestHome : BOOL;
		TestPusherFwd : BOOL;
		TestPusherRev : BOOL;
		TestGateUp : BOOL;
		TestConvFwd : BOOL;
	END_VAR
	VAR_OUTPUT
		ConveyorFwd : BOOL;
		GateUp : BOOL;
		PusherFwd : BOOL;
		PusherRev : BOOL;
	END_VAR
	VAR
		InputConfig : TT_TDC_DKFV_InputConfigFBK;
		PusherStateMgr : ChainPushStateMgrFBK;
		OutputConfig : TT_TDC_DKFV_OutputConfigFBK;
	END_VAR
END_FUNCTION_BLOCK

{REDUND_ERROR} FUNCTION_BLOCK TT_TDC_DKFV_InputConfigFBK (*Input logic for the MarkTT Transfer Table*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		Enable : {REDUND_UNREPLICABLE} BOOL;
		PartPresent : {REDUND_UNREPLICABLE} BOOL;
		AtPause : {REDUND_UNREPLICABLE} BOOL;
		CanTransfer : {REDUND_UNREPLICABLE} BOOL;
		GateSquaringTime : TIME;
		CLOCK_MS : TIME;
		OnHomedEvent : {REDUND_UNREPLICABLE} BOOL;
		IsHomed : REFERENCE TO BOOL;
		PushersMovedEvent : BOOL;
	END_VAR
	VAR_OUTPUT
		RequestConveyorFwd : BOOL;
		RequestGateUp : BOOL;
		RequestHome : BOOL;
		RequestPush : BOOL;
		RequestStop : BOOL;
	END_VAR
	VAR
		ConveyorStateMgr : TransferConveyorStateMgrFBK;
		PushRequested : BOOL;
		zzEdge00000 : BOOL;
		zzEdge00001 : BOOL;
		zzEdge00002 : BOOL;
		zzEdge00003 : BOOL;
		stagingState : TDCStagingStateEnum;
	END_VAR
END_FUNCTION_BLOCK

{REDUND_ERROR} FUNCTION_BLOCK TT_TDC_DKFV_OutputConfigFBK (*Output config for the Mark TT DKFV*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		CmdConveyorFwd : BOOL;
		CmdGateUp : BOOL;
		Enable : BOOL;
		PusherState : ChainPusherStateEnum;
	END_VAR
	VAR_OUTPUT
		ConveyorFwd : BOOL;
		GateUp : BOOL;
		PusherFwd : BOOL;
	END_VAR
END_FUNCTION_BLOCK

{REDUND_ERROR} FUNCTION_BLOCK TT_TDC_DKFV_PartPassCtrlFBK (*TODO: Add your comment here*) (*$GROUP=User,$CAT=User,$GROUPICON=User.png,$CATICON=User.png*)
	VAR_INPUT
		Enable : BOOL;
		ChainPusherState : ChainPusherStateEnum;
		ShouldEject : BOOL;
		LegInSentCount : USINT;
		PartPassRecvd : SINT;
		PartPresent : BOOL;
	END_VAR
	VAR_OUTPUT
		CmdFinishedRunning : BOOL;
		CmdTakeSent : BOOL;
		CmdSendRecvd : BOOL;
	END_VAR
END_FUNCTION_BLOCK
