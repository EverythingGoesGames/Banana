using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class TriggerObstacle : StaticBody2D
{
	[Export]
	private string path = "";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("test"))
		{
			var scene = GD.Load<PackedScene>(path);

			var shoot = scene.Instantiate();

			if (shoot.HasMethod("AdjustPosition"))
			{
				shoot.Call("AdjustPosition", Position);
			}

			GetParent().AddChild(shoot);
		}
	}
}
