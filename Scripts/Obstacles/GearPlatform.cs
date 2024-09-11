using Godot;
using System.Collections.Generic;
using SynchronousStates;

public partial class GearPlatform : SyncCharObject
{
	private RayCast2D railChecker;
	private float time = 0.00f;
	

	// Time rewound effect
	private LinkedList<RewindData> rewindHistory = new LinkedList<RewindData>();

	struct RewindData
	{
		public Vector2 pos;
		public int xDir;
		public float time;
		public Vector2 rayCastPos;
	}

	public override void _Ready()
	{
		// The railChecker is intended to detect both ends of the rail.
		railChecker = GetNode<RayCast2D>("RailChecker");
		Vector2 tmp = railChecker.Position;
		tmp.X = tmp.X * xDir;
		railChecker.Position = tmp;

		syncData.SetHistoryLimit(syncData.GetRewindLength());
		syncData.SetRewindArea(GetNode<Area2D>("RewindArea"));

		debugText = GetNode<Label>("DebugText");
	}

	public override void _PhysicsProcess(double delta)
	{
		switch(syncData.GetTimeState())
		{
			case TimeStates.Idle:
				if (!railChecker.IsColliding())
				{
					// This will pause the platform to give the player some time to get on it.
					time += (float)delta;

					PlatformMovement(0.0f);

					MoveAndSlide();

					// After a certain amount of time has passed, platform will continue moving in the opposite direction and reset time.
					if (time > 1.00)
					{
						time = 0.00f;
						xDir *= -1;

						Vector2 tmp = railChecker.Position;
						tmp.X = tmp.X * -1;
						railChecker.Position = tmp;

						PlatformMovement(speed);
						
						MoveAndSlide();
					}
				}
				else
				{
					PlatformMovement(speed);
					MoveAndSlide();
				}

				RecordHistory();

				break;
			case TimeStates.Actvated:
				RewindData record = rewindHistory.First.Value;

				Position = record.pos;
				xDir = record.xDir;
				time = record.time;
				railChecker.Position = record.rayCastPos;

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

	private void RecordHistory()
	{
		if (rewindHistory.Count >= syncData.GetHistoryLimit())
		{
			rewindHistory.RemoveLast();
		}

		RewindData timeline;

		timeline.pos = Position;
		timeline.xDir = xDir;
		timeline.time = time;
		timeline.rayCastPos = railChecker.Position;

		rewindHistory.AddFirst(timeline);
		//GD.Print(rewindHistory.Count);
	}
}
