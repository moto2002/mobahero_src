using System;

public enum MoveState : byte
{
	MoveState_Null,
	MoveState_StartMove,
	MoveState_NextPos,
	MoveState_StopMove,
	MoveState_SpecialMove,
	MoveState_StopSpecialMove,
	MoveState_ForceMove,
	MoveState_MoveEnd,
	MoveState_SpecialMoveEnd_CB
}
