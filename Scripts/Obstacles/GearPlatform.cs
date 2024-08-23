using Godot;
using System;

public partial class GearPlatform : CharacterBody2D
{
	private RayCast2D railChecker;
	private int dir = -1;
	private float time = 0.00f;

	[Export]
	public float platformSpeed = 50.0f;

	public override void _Ready()
	{
		// The railChecker is intended to detect both ends of the rail.
		railChecker = GetNode<RayCast2D>("RailChecker");
	}

	public override void _PhysicsProcess(double delta)
	{

		if (!railChecker.IsColliding())
		{
			// This will pause the platform to give the player some time to get on it.
			time += (float)delta;

			PlatformMovement(0.0f);

			// After a certain amount of time has passed, platform will continue moving in the opposite direction and reset time.
			if (time >= 1.00)
			{
				time = 0.00f;
				dir *= -1;

				PlatformMovement(platformSpeed);
			}
		}
		else
		{
			PlatformMovement(platformSpeed);
		}
	}

	private void PlatformMovement(float speed)
	{
		Vector2 velocity = Velocity;

		velocity.X = speed * dir;

		Velocity = velocity;
		MoveAndSlide();
	}
}
