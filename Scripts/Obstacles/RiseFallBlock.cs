using Godot;
using System;

public partial class RiseFallBlock : Path2D
{
	[Export]
	public float speed_scale = 1.0f;

	// private PathFollow2D followPath; 
	private Area2D area;
	private AnimationPlayer player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//followPath = GetNode<PathFollow2D>("PathFollow2D");

		area = GetNode("AnimatableBody2D").GetNode<Area2D>("Area2D");
		area.BodyEntered += (body) => OnPlatform();
		area.BodyExited += (body) => NotOnPlatform();

		player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.SpeedScale = speed_scale;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// Make the platform move forward (depending on the path created) when player gets on top of the platform
	public void OnPlatform()
	{
		player.Play("Move");
	}

	// Make the platform move backwards (depending on the path created) when player gets off the platform
	public void NotOnPlatform()
	{
		player.PlayBackwards("Move");
	}
}
