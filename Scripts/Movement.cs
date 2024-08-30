using Godot;
using System;

public partial class Movement : CharacterBody2D
{
	[Signal]
	public delegate void HitEventHandler();

	
	float Speed = 50;
	private const float Gravity = 950.0f;
	private const float jumpHeight = 800.0f;
	public Vector2 velocity = Vector2.Zero;
	public Vector2 targetVelocity = new Vector2(1000,0);
	public bool onFloor = false;
	private double TimeSinceWalkedOff = 0;

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
			velocity.X += velocity.MoveToward(targetVelocity, 10).X *Speed;
			Speed += 100 * (float) delta;
            GD.Print(velocity.X);
        }

		if (Input.IsActionPressed("move_left"))
		{
            GD.Print("left");
            velocity.X += velocity.MoveToward(-targetVelocity, 10).X * Speed;
			Speed += 100 * (float)delta;
		}
		if (!Input.IsAnythingPressed())
		{ 
			velocity = velocity.MoveToward(new Vector2(0, Gravity), 50) *Speed;
			Speed = 5;
		}

     

        velocity = velocity.Normalized();

        if (Input.IsActionPressed("jump") && (IsOnFloor() || onFloor))
        {
            velocity.Y = -jumpHeight;
        }
        else if (!Input.IsActionPressed("jump") && (!IsOnFloor() || !onFloor))
        {
			velocity.X /= 3;
            velocity.Y = Gravity * (float)delta;
			GD.Print("not on ground");
        }
		velocity.X *= Speed;
        Velocity = velocity;
		MoveAndSlide();
		//Causes possiblity of infinte jump
		//Could turn this bug into a feature and have jump timing
		


	}
}
