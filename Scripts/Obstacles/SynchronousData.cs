using Godot;
using SynchronousStates;

public partial class SynchronousData : Node
{
	[Export]
	private int rewindLength = 4; // Value represents seconds

	[Export]
	private bool canManipulate = true;

	private int historyLimit = 60; // 60 represent fps

	private TimeStates state = TimeStates.Idle;
	private Area2D rewindArea;
	private bool mouseInArea = false;

	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if ((mouseInArea) && Input.IsActionJustPressed("rewind") && (state != TimeStates.Exited))
		{
			//GD.Print("Active Rewind Phase");
			SetTimeState(TimeStates.Actvated);
        }
	}

	private void MouseInside()
	{
		mouseInArea = true;
    }

	private void MouseOutside()
	{
		mouseInArea = false;
	}


	public void SetHistoryLimit(int time)
	{
		historyLimit = 60 * time;
	}

	public int GetHistoryLimit()
	{
		return historyLimit;
	}

	public void SetRewindLength(int rewindLength)
	{
		this.rewindLength = rewindLength;
	}
	
	public int GetRewindLength()
	{
		return rewindLength;
	}

	public void SetTimeState(TimeStates state)
	{
		this.state = state;
	}

	public TimeStates GetTimeState()
	{
		return state;
	}

	public void SetRewindArea(Area2D area)
	{
		rewindArea = area;

		rewindArea.MouseEntered += () => MouseInside();
		rewindArea.MouseExited += () => MouseOutside();
	}

	public void SetCanManipulate(bool canManipulate)
	{
		this.canManipulate = canManipulate;
	}
}
