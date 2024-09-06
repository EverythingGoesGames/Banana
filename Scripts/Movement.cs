using Godot;
using System;

public partial class Movement : CharacterBody2D
{
	[Export]
	float Speed = 50;
	[Export]
	private const float Gravity = 980.0f;
	[Export]
	private const float jumpHeight = 500.0f;
	[Export]
	private const float fallSpeedModifier = 100.0f;
	[Export]
	private const float shortHopModifier = 0.8f;
	

	public Vector2 velocity = Vector2.Zero;
	public Vector2 targetVelocity = new Vector2(500,0);
	public bool onFloor = false;
	private double TimeSinceWalkedOff = 0;
	
	private float timeHeld = 0.0f;
	private float shortHopTime = 0.15f;
	private float maxHoldTime = 0.3f;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var TimeSinceWalkedOff = Time.GetUnixTimeFromSystem();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//velocity.Y += Vector2.Down.Y * Gravity * (float)delta;
			
		if (IsOnFloor())
		{
			TimeSinceWalkedOff = Time.GetUnixTimeFromSystem();
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
			if (velocity.X < 0)
			{
				velocity.X = -velocity.X/2;
			}
			else
			{
				velocity.X += velocity.MoveToward(targetVelocity, 10).X * Speed;
				Speed += 100 * (float)delta;
			}
			
		}

		if (Input.IsActionPressed("move_left"))
		{
			if (velocity.X > 0)
			{
				velocity.X = -velocity.X/2;
			}
			else
			{
				velocity.X += velocity.MoveToward(-targetVelocity, 10).X * Speed;
				Speed += 100 * (float)delta;
			}
			
		}
		if (!Input.IsAnythingPressed())
		{ 
			velocity = velocity.MoveToward(new Vector2(0, Gravity), 50) *Speed;
			Speed = 5;
		}
		
		velocity = velocity.Normalized();

		//if (Input.IsActionJustPressed("jump") && (IsOnFloor() || onFloor) && timeHeld < maxHoldTime)
		////  if (Input.IsActionJustPressed("jump")  && timeHeld < maxHoldTime)
		//{
			//timeHeld += (float)delta;
			//if (timeHeld < shortHopTime)
			//{
				//velocity.Y = -jumpHeight * shortHopModifier;
				//GD.Print("shorthop", timeHeld);
			//}
			//else
			//{
				//velocity.Y = -jumpHeight * 100;
				//GD.Print("longhop", timeHeld);
			//}
//
		//}
		//else if (!Input.IsActionPressed("jump") && (!IsOnFloor() && !onFloor))
		//{
			//velocity.X /= 2;
			//velocity.Y = Gravity * (float)delta;
			//timeHeld = 0.0f;
		//}
		//else 
		
		//  START BRIAN JUMP/FALL STUFF
		//  as we fall, we should fall faster 9.8m/s^2 and all that
		float fallingMultiplier = (float)((Time.GetUnixTimeFromSystem() - TimeSinceWalkedOff) * (Time.GetUnixTimeFromSystem() - TimeSinceWalkedOff));
		fallingMultiplier = fallingMultiplier * fallSpeedModifier;
		if (fallingMultiplier > 100) fallingMultiplier = 100;
		
		if (Input.IsActionPressed("jump") && onFloor)
		{
			timeHeld += (float)delta;
			if (timeHeld < shortHopTime)
			{
				velocity.Y = -jumpHeight * shortHopModifier;
			}
			else if (timeHeld > shortHopTime && timeHeld < maxHoldTime)
			{
				//  velocity should decrease as we approach the apex of the jump
				float taper = (1 - timeHeld * 2);
				velocity.Y = -jumpHeight * shortHopModifier * taper;
			}
			else
			{
				velocity.Y = Gravity * (float)delta * fallingMultiplier;
			}
		}
		else
		{
			velocity.Y = Gravity * (float)delta * fallingMultiplier;
			timeHeld = 0.0f;
		}
		
		//  END BRIAN JUMP/FALL STUFF

		velocity.X *= Speed;
		Velocity = velocity;
		MoveAndSlide();
		//Causes possiblity of infinte jump
		//Could turn this bug into a feature and have jump timing
		


	}
}
