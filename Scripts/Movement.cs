using Godot;
using System;

public partial class Movement : CharacterBody2D
{
    [Signal]
    public delegate void HitEventHandler();

    
    float Speed = 1000;
    private const float Gravity = 500.0f;
    private const float jumpHeight = 6050.0f;
    public Vector2 velocity = Vector2.Zero;
    public float targetXVelocity = 1000f;
    public bool onFloor = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Console.WriteLine("here");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        
        if (!IsOnFloor())
        {
            velocity.Y += Vector2.Down.Y * Gravity * (float)delta;
            //Add timer
            var TimeSinceWalkedOff = Time.GetUnixTimeFromSystem();
            if (Time.GetUnixTimeFromSystem() - TimeSinceWalkedOff < 2)
            {
                onFloor = true;
            }
            else
            {
                onFloor = false;
            }
        }
        
         
        // The player's movement vector.

        if (Input.IsActionPressed("move_right"))
        {
           if (velocity.X < targetXVelocity)
            {
                velocity.X += 300;
                Speed += 100;
            }
            else
            {
                velocity.X = 1000;
            }
            
        }

        if (Input.IsActionPressed("move_left"))
        {
            if (velocity.X > targetXVelocity)
            {
                velocity.X -= 300;
                Speed += 100;
            }
            else
            {
                velocity.X = -1000;
            }
        }
        if (!Input.IsAnythingPressed())
        {
            velocity = velocity.MoveToward(new Vector2(0, 0), -62);
        }

        velocity = velocity.Normalized();


        Velocity = velocity * (float)delta * Speed;
        MoveAndSlide();
        //Causes possiblity of infinte jump
        //Could turn this bug into a feature and have jump timing
        if (Input.IsActionPressed("jump") && (IsOnFloor() || onFloor))
        {
            velocity.Y = -jumpHeight *Speed;
        }

    }
}
