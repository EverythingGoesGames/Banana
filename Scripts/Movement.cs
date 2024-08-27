using Godot;
using System;

public partial class Movement : CharacterBody2D
{
	[Signal]
	public delegate void HitEventHandler();

	
	float Speed = 5;
	private const float Gravity = 500.0f;
	private const float jumpHeight = 6050.0f;
	public Vector2 velocity = Vector2.Zero;
	public float targetXVelocity = 10f;
	public Vector2 targetVelocity = new Vector2(1000,0);
	public bool onFloor = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Console.WriteLine("here");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
			velocity.Y += Vector2.Down.Y * Gravity * (float)delta;
			//Add timer
			var TimeSinceWalkedOff = Time.GetUnixTimeFromSystem();
			if (IsOnFloor())
			{
				TimeSinceWalkedOff += Time.GetUnixTimeFromSystem();
				onFloor = true;
			}
			else
			{
			if (Time.GetUnixTimeFromSystem() - TimeSinceWalkedOff < .2)
			{
				onFloor = true;
			}
			else
			{
				onFloor = false;
			}

			}

		if (Input.IsActionPressed("move_right"))
		{
			velocity.X += velocity.MoveToward(targetVelocity, 10).X *Speed;
			Speed += 100 * (float) delta;
		}

		if (Input.IsActionPressed("move_left"))
		{
			velocity.X += velocity.MoveToward(-targetVelocity, 10).X * Speed;
			Speed += 100 * (float)delta;
		}
		if (!Input.IsAnythingPressed())
		{ 
			velocity = velocity.MoveToward(new Vector2(0, Gravity), 500) *Speed;
			Speed = 5;
		}

		velocity = velocity.Normalized();


		Velocity = velocity * Speed;
		MoveAndSlide();
		//Causes possiblity of infinte jump
		//Could turn this bug into a feature and have jump timing
		if (Input.IsActionPressed("jump") && (IsOnFloor() || onFloor))
		{
			velocity.Y = -jumpHeight * Speed;
		}

	}
}
