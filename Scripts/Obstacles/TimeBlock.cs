using Godot;
using System;

public partial class TimeBlock : CharacterBody2D
{
	[Export]
	private int countdown = 3;

	private Timer timer;
	private AnimationPlayer player;
	private Area2D area;
	private Label label;
	private Area2D area2;

	// Used to prevent repeatedly triggeting the rotating animation mechanic and alternate between two animation
	private bool timerRunning = false;
	private bool upright = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.AnimationFinished += (body) => RotateFinished();

		timer = GetNode<Timer>("Timer");
		timer.Timeout += () => RotatePlatform();

		area = GetNode<Area2D>("Area2D");
		area.BodyEntered += (body) => InitiateRotation((CharacterBody2D)body);

		area2 = GetNode<Area2D>("Area2D2");
		area2.BodyEntered += (body) => InitiateRotation((CharacterBody2D)body);

		label = GetNode<Label>("Label");
		label.Text = GD.VarToStr(countdown);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!timer.IsStopped())
		{
			label.Text = GD.VarToStr((int)Mathf.Round(timer.TimeLeft));
		}

		// Testing possible time rewind mechanic
		//if (Input.IsActionJustPressed("jump"))
		//{
		//	if (player.IsPlaying() == false)
		//	{
		//		player.Call("play", "Rotate_Upright");
		//	}
		//	else
		//	{
		//		player.Call("play_backwards", "Rotate_Upright");
		//	}
		//}
	}

	private void RotatePlatform()
	{
		if (!upright)
		{
			player.Play("Rotate_Upright");
		}
		else
		{
			player.PlayBackwards("Rotate_Upright");
		}
	}

	private void RotateFinished()
	{
		timerRunning = false;
		upright = !upright;
		label.Text = GD.VarToStr(countdown);
	}

	private void InitiateRotation(CharacterBody2D body)
	{
		if ((timer.IsStopped() == true) && (!timerRunning))
		{
			timerRunning = true;
			timer.Start(countdown);
		}
	}
}
