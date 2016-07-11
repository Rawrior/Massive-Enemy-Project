using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class ControllerClass
{
	#region Variables
	public readonly int ControllerNumber;
	public PlayerIndex Index;
	public const float DeadZone = 0.2f;
	public Vector2 LastAim = new Vector2(1, 0);
	public float VibrateTimer = 0f;

	public GamePadState State;

	public Dictionary<string, bool> Buttons = new Dictionary<string, bool>(12);
	public Dictionary<string, bool> PreviousButtons;
	#endregion
	#region Functions
	public ControllerClass(int controllerNumber)
	{
		ControllerNumber = controllerNumber;
		Index = (PlayerIndex)controllerNumber;
		State = GamePad.GetState(Index);
		UpdateState();
	}

	public void UpdateState()
	{
		State = GamePad.GetState(Index);
		PreviousButtons = new Dictionary<string, bool>(Buttons);

		Buttons["A"] = State.Buttons.A == ButtonState.Pressed;
		Buttons["B"] = State.Buttons.B == ButtonState.Pressed;
		Buttons["X"] = State.Buttons.X == ButtonState.Pressed;
		Buttons["Y"] = State.Buttons.Y == ButtonState.Pressed;
		Buttons["LB"] = State.Buttons.LeftShoulder == ButtonState.Pressed;
		Buttons["RB"] = State.Buttons.RightShoulder == ButtonState.Pressed;
		Buttons["Select"] = State.Buttons.Back == ButtonState.Pressed;
		Buttons["Start"] = State.Buttons.Start == ButtonState.Pressed;
		Buttons["LS"] = State.Buttons.LeftStick == ButtonState.Pressed;
		Buttons["RS"] = State.Buttons.RightStick == ButtonState.Pressed;
		Buttons["Guide"] = State.Buttons.Guide == ButtonState.Pressed;

		if (Time.time > VibrateTimer)
		{
			VibrateTimer = 0f;
			StopVibration();
		}
	}

	public bool AxisActive(Vector2 axis)
	{
		return Mathf.Abs(axis.x) > DeadZone || Mathf.Abs(axis.y) > DeadZone;
	}
	#endregion
	#region Axis
	public Vector2 DPad
	{
		get
		{
			Vector2 dPadVector = Vector2.zero;

			if (State.DPad.Up == ButtonState.Pressed)
				dPadVector += new Vector2(0, 1);
			if (State.DPad.Down == ButtonState.Pressed)
				dPadVector += new Vector2(0, -1);
			if (State.DPad.Left == ButtonState.Pressed)
				dPadVector += new Vector2(-1, 0);
			if (State.DPad.Right == ButtonState.Pressed)
				dPadVector += new Vector2(1, 0);

			return dPadVector;
		}
	}

	public Vector2 LeftThumbstick
	{
		get { return new Vector2(State.ThumbSticks.Left.X, State.ThumbSticks.Left.Y); }
	}

	public Vector2 RightThumbstick
	{
		get { return new Vector2(State.ThumbSticks.Right.X, State.ThumbSticks.Right.Y); }
	}

	public Vector2 Movement
	{
		get
		{
			if (AxisActive(DPad))
				return DPad;
			else
				return LeftThumbstick;
		}
	}

	#endregion
	#region Aim Axis
	
	public Vector2 Aim
	{
		get
		{
			Vector2 resultVector = Vector2.zero;
			if (AxisActive(RightThumbstick))
				resultVector = RightThumbstick;
			else if (AxisActive(DPad))
				resultVector = DPad;
			else if (AxisActive(LeftThumbstick))
				resultVector = LeftThumbstick;
			else
				resultVector = LastAim;

			LastAim = resultVector;
			return resultVector;
		}
	}

	#endregion
	#region Buttons
	public bool GetButton(string buttonName)
	{
		return Buttons[buttonName];
	}

	public bool GetButtonUp(string buttonName)
	{
		return !Buttons[buttonName] && PreviousButtons[buttonName];
	}

	public bool GetButtonDown(string buttonName)
	{
		return Buttons[buttonName] && !PreviousButtons[buttonName];
	}
	#endregion
	#region Feedback

	public void Vibrate(float left, float right, float stopTime)
	{
		GamePad.SetVibration(Index, left, right);
		VibrateTimer = stopTime + Time.time;
	}

	public void StopVibration()
	{
		GamePad.SetVibration(Index, 0f, 0f);
	}

	#endregion
	#region InputManager

	public bool JumpButtonDown()
	{
		return GetButtonDown("A") || GetButtonDown("LB");
	}

	public bool JumpButtonUp()
	{
		return GetButtonUp("A") || GetButtonUp("LB");
	}

	public bool AttackButton()
	{
		return GetButton("X") || GetButton("RB");
	}

	public bool SpecialAttackButton()
	{
		return GetButton("Y") || State.Triggers.Right > 0.5f;
	}

	#endregion
}
