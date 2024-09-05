using Godot;
using System;

public partial class Movement : CharacterBody2D
{
	[Export]
	float Speed = 50;
	[Export]
	private const float Gravity = 950.0f;
	[Export]
	private const float jumpHeight = 800.0f;
	public Vector2 velocity = Vector2.Zero;
	public Vector2 targetVelocity = new Vector2(1000,0);
	public bool onFloor = false;
	private double TimeSinceWalkedOff = 0;
	private float timeHeld = 0.0f;
	private float maxHoldTime = 0.1f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        var TimeSinceWalkedOff = Time.GetUnixTimeFromSystem();
        //Console.WriteLine("here");
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

        if (Input.IsActionJustPressed("jump") && (IsOnFloor() || onFloor) && timeHeld < maxHoldTime)
        {
			timeHeld += (float)delta;
            velocity.Y = -jumpHeight;
        }
        else if (!Input.IsActionPressed("jump") && (!IsOnFloor() && !onFloor))
        {
			velocity.X /= 2;
            velocity.Y = Gravity * (float)delta;
			timeHeld = 0.0f;
        }
		else if (Input.IsActionPressed("jump"))
		{
			timeHeld += (float)delta;
			GD.Print(timeHeld);
			if (timeHeld >= maxHoldTime)
			{
				velocity.Y = Gravity * (float)delta;
            }
		}
		else
		{
			velocity.Y = Gravity * (float)delta;
			timeHeld = 0.0f;
		}

		velocity.X *= Speed;
        Velocity = velocity;
		MoveAndSlide();
		//Causes possiblity of infinte jump
		//Could turn this bug into a feature and have jump timing
		


	}
}
