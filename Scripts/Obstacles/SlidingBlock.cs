using Godot;
using System;
using System.Collections.Generic;

public partial class SlidingBlock : CharacterBody2D
{
	[Export]
	private float speed = 50.0f;

	[Export]
	private int x_dir = 0;

	[Export]
	private int y_dir = 0;

	[Export]
	private float xDisplacement = 0.00f;

	[Export]
	private float yDisplacement = 0.00f;

	private string state = "Idle";

	LinkedList<Vector2> positionHistory = new LinkedList<Vector2>();

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		switch(state)
		{
			case "Idle":
				Vector2 velocity = Velocity;

				velocity.X = x_dir;
				velocity.Y = y_dir;

				Velocity = velocity.Normalized() * speed;

				KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);

				if (collision != null)
				{
					QueueFree();
				}

				if (Input.IsActionJustPressed("rewind"))
				{
					state = "Activated";
				}

				RecordHistory();
				break;
			case "Activated":
				GlobalPosition = positionHistory.First.Value;
				positionHistory.RemoveFirst();

				if (positionHistory.Count ==0)
				{
					state = "Exited";
				}
				break;
			case "Exited":
				state = "Idle";
				break;
		}
	}

	public void AdjustPosition(Vector2 pos)
	{
		pos.X = pos.X + xDisplacement;

		pos.Y = pos.Y + yDisplacement;

		GlobalPosition = pos;
	}

	private void RecordHistory()
	{
		if (positionHistory.Count == 240)
		{
			positionHistory.RemoveLast();
		}

		positionHistory.AddFirst(GlobalPosition);
	}
}
