using Godot;
using System.Collections.Generic;
using SynchronousStates;

public partial class SlidingBlock : SyncCharObject
{
	private LinkedList<Vector2> rewindHistory = new LinkedList<Vector2>();

	public override void _Ready()
	{
		syncData.SetHistoryLimit(syncData.GetHistoryLimit());
		syncData.SetRewindArea(GetNode<Area2D>("RewindArea"));

		debugText = GetNode<Label>("DebugText");
	}

    public override void _PhysicsProcess(double delta)
	{
		switch(syncData.GetTimeState())
		{
			case TimeStates.Idle:
				PlatformMovement(speed);

				KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);

				if (collision != null)
				{
					QueueFree();
				}

				RecordHistory();
				break;
			case TimeStates.Actvated:
				GlobalPosition = rewindHistory.First.Value;
				rewindHistory.RemoveFirst();

				if (rewindHistory.Count == 0)
				{
					syncData.SetTimeState(TimeStates.Idle);
				}
				break;
			case TimeStates.Exited:
				break;
		}

		debugText.Text = "Rewind: " + syncData.GetRewindLength().ToString() + "s";
	}

	public void AdjustPosition(Vector2 pos, float rotation)
	{
		GlobalPosition = pos;

		if (rotation > 179.0f)
		{
			xDir *= -1;
			yDir *= -1;
		}
	}

	private void RecordHistory()
	{
		if (rewindHistory.Count >= syncData.GetHistoryLimit())
		{
			rewindHistory.RemoveLast();
		}

		rewindHistory.AddFirst(GlobalPosition);
	}
}
