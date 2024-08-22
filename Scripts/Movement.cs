using Godot;
using System;

public partial class Movement : CharacterBody2D
{
    [Signal]
    public delegate void HitEventHandler();

    Vector2 velocity = Vector2.Zero;
    float Speed = 50;
    private const float Gravity = 200.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Console.WriteLine("here");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        var velocity = Vector2.Zero; // The player's movement vector.

        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 300;
        }

        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 300;
        }

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
        }

        var collision = MoveAndCollide(velocity * (float)delta);

    }
}
