using Godot;
using System;

public partial class SlidingBlock : CharacterBody2D
{
	[Export]
	private float speed = 50.0f;

	[Export]
	private int x_dir = 0;

	[Export]
	private int y_dir = 0;

	private bool activated = false;

	public override void _Ready()
	{
		base._Ready();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("test"))
		{
			activated = true;
		}

		if (activated)
		{
			Vector2 velocity = Velocity;

			velocity.X = x_dir;
			velocity.Y = y_dir;

			Velocity = velocity.Normalized() * speed;
		}

		KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);

		if (collision != null)
		{
			QueueFree();
		}
	}
}
