using Godot;
using System;
using System.Collections.Generic;
using SynchronousStates;

public partial class RiseFallBlock : Path2D
{
	[Export]
	private SynchronousData syncData;

	private Label debugText;

	private PathFollow2D followPath; 
	private Area2D area;
	private AnimationPlayer player;
	private bool isOnPlatform = false;

	private LinkedList<object> progressRatio = new LinkedList<object>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		followPath = GetNode<PathFollow2D>("PathFollow2D");

		area = GetNode("AnimatableBody2D").GetNode<Area2D>("Area2D");
		area.BodyEntered += (body) => OnPlatform(body);
		area.BodyExited += (body) => NotOnPlatform();

		syncData.SetHistoryLimit(syncData.GetRewindLength());
		syncData.SetRewindArea(GetNode<AnimatableBody2D>("AnimatableBody2D").GetNode<Area2D>("RewindArea"));

		debugText = GetNode<Label>("DebugText");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(syncData.GetTimeState())
		{
			case TimeStates.Idle:

				if ((isOnPlatform))
				{
					followPath.ProgressRatio += (float) delta / 2.0f;
				}
				else if ((followPath.ProgressRatio > 0.0) && (!isOnPlatform))
				{
					followPath.ProgressRatio -= (float) delta / 2.0f;
				}

				RecordHistory();
				break;
			case TimeStates.Actvated:
				followPath.ProgressRatio = (float) progressRatio.First.Value;

				progressRatio.RemoveFirst();

				if (progressRatio.Count == 0)
				{
					syncData.SetTimeState(TimeStates.Idle);
				}
				break;
			case TimeStates.Exited:
				break;
		}

		debugText.Text = "Rewind: " + syncData.GetRewindLength().ToString() + "s";
	}

	// Make the platform move forward (depending on the path created) when player gets on top of the platform
	public void OnPlatform(Node2D body)
	{
		if (body is CharacterBody2D)
		{
			CharacterBody2D tmp = (CharacterBody2D) body;

			if (tmp.CollisionLayer == 1)
			{
				isOnPlatform = true;
			}
		}
	}

	// Make the platform move backwards (depending on the path created) when player gets off the platform
	public void NotOnPlatform()
	{
		isOnPlatform = false;
	}

	private void RecordHistory()
	{
		if (progressRatio.Count >= syncData.GetHistoryLimit())
		{
			progressRatio.RemoveLast();
		}

		object tmp = (object) followPath.ProgressRatio;

		progressRatio.AddFirst(tmp);
	}
}
