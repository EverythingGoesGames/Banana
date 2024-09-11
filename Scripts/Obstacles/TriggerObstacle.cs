using Godot;

public partial class TriggerObstacle : StaticBody2D
{
	[Export]
	private string path = "";

	[Export]
	private Area2D triggerArea;

	private bool activateObject = false;

	private CharacterBody2D body;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Get the node address for its global position when instanting adding a scene to the tree
		body = GetNode<CharacterBody2D>("CharacterBody2D");

		triggerArea.BodyEntered += (body) => ShootObject(body);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (activateObject)
		{
			// Assign activateObject false to prevent multiple instantiation so that player to move into the area again to trigger obstacle
			activateObject = false;
			var scene = GD.Load<PackedScene>(path);

			var shoot = scene.Instantiate();

			if (shoot.HasMethod("AdjustPosition"))
			{
				shoot.Call("AdjustPosition", body.GlobalPosition, Mathf.Abs(Mathf.RadToDeg(Rotation)));
			}

			GetParent().AddChild(shoot);
		}
	}

	public void ShootObject(Node2D body)
	{
		if (body is CharacterBody2D)
		{
			CharacterBody2D tmp = (CharacterBody2D) body;

			if (tmp.CollisionLayer == 1)
			{
				activateObject = true;
			}
		}
	}
}
