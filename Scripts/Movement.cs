using Godot;
using System;

public partial class Movement : CharacterBody2D
{
	[Signal]
	public delegate void HitEventHandler();

<<<<<<< Updated upstream
	
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
=======
    
    float Speed = 5;
    private const float Gravity = 100.0f;
    private const float jumpHeight = 5800.0f;
    public Vector2 velocity = Vector2.Zero;
    public Vector2 targetVelocity = new Vector2(30,500);
    public bool onFloor = false;
    private double TimeSinceWalkedOff = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //var TimeSinceWalkedOff = Time.GetUnixTimeFromSystem();
        //Console.WriteLine("here");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!IsOnFloor())
        {
            velocity.Y += Gravity;
        }

        /*if (IsOnFloor())
        {
            TimeSinceWalkedOff += delta;
            onFloor = true;
        }
        else
        {
            if (Time.GetUnixTimeFromSystem() - TimeSinceWalkedOff < 100000)
            {
                onFloor = true;
                TimeSinceWalkedOff = Time.GetUnixTimeFromSystem();
                GD.Print("reset time");
            }
            else
            {
                GD.Print("Too big");
                onFloor = false;
            }

        }*/

        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += velocity.MoveToward(targetVelocity, 80).X *Speed;

            Speed += 100 * (float) delta;
        }
        else if (Input.IsActionPressed("move_left"))
        {
            velocity.X += velocity.MoveToward(-targetVelocity, 30).X * Speed;
            Speed += 100 * (float)delta;
        }
        if (!Input.IsAnythingPressed())
        { 
            velocity = velocity.MoveToward(new Vector2(velocity.X, Gravity), 500);
            Speed = 5;
        }

        if (Input.IsActionJustPressed("jump") && (IsOnFloor() || onFloor))
        {
            GD.Print("Jumped");
            velocity.Y += -jumpHeight;
        }

        Velocity = velocity;
        velocity = velocity.Normalized();
        
        MoveAndSlide();
        //Causes possiblity of infinte jump
        //Could turn this bug into a feature and have jump timing
       
>>>>>>> Stashed changes

	}
}
