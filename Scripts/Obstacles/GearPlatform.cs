using Godot;
using System;
using System.Collections.Generic;

public partial class GearPlatform : CharacterBody2D
{
	[Export]
	public float platformSpeed = 50.0f;

	[Export]
	public int dir = 1;

	private RayCast2D railChecker;
	private float time = 0.00f;
	private string state = "Idle";

	// Time rewound effect
	LinkedList<Rewind> rewindHistory = new LinkedList<Rewind>();
	private const int HISTORYLIMIT = 240;

	private Label label; // For testing rewind effect

	public struct Rewind
	{
		public Vector2 pos;
		public int dir;
		public float time;
		public Vector2 rayCastPos;
	}

	public override void _Ready()
	{
		// The railChecker is intended to detect both ends of the rail.
		railChecker = GetNode<RayCast2D>("RailChecker");
		Vector2 tmp = railChecker.Position;
		tmp.X = tmp.X * dir;
		railChecker.Position = tmp;

		label = GetNode<Label>("Label");
	}

	public override void _PhysicsProcess(double delta)
	{
		switch(state)
		{
			case "Idle":
				if (!railChecker.IsColliding())
				{
					// This will pause the platform to give the player some time to get on it.
					time += (float)delta;

					PlatformMovement(0.0f);

					// After a certain amount of time has passed, platform will continue moving in the opposite direction and reset time.
					if (time > 1.00)
					{
						time = 0.00f;
						dir *= -1;

						Vector2 tmp = railChecker.Position;
						tmp.X = tmp.X * -1;
						railChecker.Position = tmp;

						PlatformMovement(platformSpeed);
					}
				}
				else
				{
					PlatformMovement(platformSpeed);
				}

				label.Text = "Not Rewinding";

				RecordHistory();


				if (Input.IsActionJustPressed("rewind"))
				{
					state = "Activated";
				}


				break;
			case "Activated":
				Rewind record = rewindHistory.First.Value;

				label.Text = "Rewinding";

				Position = record.pos;
				dir = record.dir;
				time = record.time;
				railChecker.Position = record.rayCastPos;

				rewindHistory.RemoveFirst();

				if (rewindHistory.Count == 0)
				{
					state = "Exited";
				}
				break;
			case "Exited":
				state = "Idle";
				break;
		}

		
	}

	private void PlatformMovement(float speed)
	{
		Vector2 velocity = Velocity;

		velocity.X = speed * dir;

		Velocity = velocity;
		MoveAndSlide();
	}

	private void RecordHistory()
	{
		if (rewindHistory.Count == HISTORYLIMIT)
		{
			rewindHistory.RemoveLast();
		}

		Rewind timeline;

		timeline.pos = Position;
		timeline.dir = dir;
		timeline.time = time;
		timeline.rayCastPos = railChecker.Position;

		rewindHistory.AddFirst(timeline);
		//GD.Print(rewindHistory.Count);
	}
}
