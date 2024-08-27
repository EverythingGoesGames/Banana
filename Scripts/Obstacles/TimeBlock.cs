using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

public partial class TimeBlock : CharacterBody2D
{
	[Export]
	private float countdown = 1.0f;

	private float time = 0.0f;

	private AnimationPlayer player;
	private Area2D area;
	private Label label;
	private CollisionShape2D collision;

	private Label label2; // Testing purpose

	// Initiate the process of destroying the time block
	private bool timerRunning = false;
	private bool done = false;
	private string forwardBackward = "do_nothing";


	// Rewind feature
	private float timeRewind = 0.0f;
	private float tmpRewind = 0.0f;
	private string state = "Idle";

	private bool animationDelay = false;
	private const int HISTORYLIMIT = 240;
	private const float RECORDMARKER = 0.1f;

	LinkedList<Rewind> rewindHistory = new LinkedList<Rewind>();
	

	public struct Rewind
	{
		public float time;
		public bool timerRunning;
		public bool done;
		public bool collsionEnabled;
		public bool areaEnabled;
		public string forwardBackward;  // This is for the animation player
	};



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.AnimationFinished += (body) => AnimationFinished();

		area = GetNode<Area2D>("Area2D");
		area.BodyEntered += (body) => InitiateDestruction((CharacterBody2D)body);

		collision = GetNode<CollisionShape2D>("CollisionShape2D");

		label = GetNode<Label>("Label");
		label.Text = GD.VarToStr(countdown);

		label2 = GetNode<Label>("Label2");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(state)
		{
			case "Idle":
				if (timerRunning == true)
				{
					time += (float)delta;
				}

				// Display the time on the block with 1 decimal place
				if (!done)
				{
					float num = Mathf.Clamp(countdown - time, 0.0f, countdown);
					label.Text = num.ToString("N1");
				}

				if (time > countdown)
				{
					done = true;
					timerRunning = false;
					forwardBackward = "play";
					player.Call(forwardBackward, "Destroyed");
					collision.Disabled = true;
					area.Monitoring = false;
				}

				label2.Text = "Not Rewinding";

				// Time rewind mechanic
				RecordHistory();
				break;
			case "Activated":

				Rewind record = rewindHistory.First();

				// Assign essential properties for rewind effect
				time = record.time;
				done = record.done;
				collision.Disabled = record.collsionEnabled;
				area.Monitoring = record.areaEnabled;
				timerRunning = record.timerRunning;
				forwardBackward = record.forwardBackward;

				rewindHistory.RemoveFirst();


				if (record.forwardBackward.Equals("play_backwards"))
				{
					player.Call(forwardBackward, "Destroyed");
				}
				else if ((rewindHistory.Count == 0) && (record.forwardBackward.Equals("play")))
				{
					player.Call(forwardBackward, "Destroyed");
				}
				else if ((rewindHistory.Count == 0) && (record.forwardBackward.Equals("do_nothing")))
				{
					player.Stop();
				}

				if (!player.IsPlaying() && (timerRunning))
				{
					float num = Mathf.Clamp(countdown - time, 0.0f, countdown);
					label.Text = num.ToString("N1");
					//GD.Print(time);
				}

				//GD.Print(rewindHistory.Count, ": ", record.time, " ", record.forwardBackward);

				label2.Text = "Rewinding";

				if (rewindHistory.Count == 0)
				{
					state = "Exited";
				}
				break;
			case "Exited":

				state = "Idle";

				break;
		}



		// Testing the animation
		if (Input.IsActionJustPressed("test"))
		{
			timerRunning = true;
		}

		// Testing the rewind feature
		if (Input.IsActionJustPressed("rewind"))
		{
			state = "Activated";
		}
	}

	private void AnimationFinished()
	{
		time = 0.0f;
		forwardBackward = "do_nothing";
	}

	private void InitiateDestruction(CharacterBody2D body)
	{
		if (body.CollisionLayer == 0)
		{
			timerRunning = true;
		}
	}

	private void RecordHistory()
	{
		if (rewindHistory.Count >= HISTORYLIMIT)
		{
			rewindHistory.RemoveLast();
		}

		Rewind timeline;
		timeline.time = time;
		timeline.done = done;
		timeline.collsionEnabled = collision.Disabled;
		timeline.areaEnabled = area.Monitoring;
		timeline.timerRunning = timerRunning;

		if (forwardBackward.Equals("do_nothing"))
		{
			timeline.forwardBackward = forwardBackward;
		}
		else if (forwardBackward.Equals("play") && (rewindHistory.First().forwardBackward.Equals("do_nothing")))
		{
			timeline.forwardBackward = "play";
		}
		else
		{
			timeline.forwardBackward = "play_backwards";
		}

		rewindHistory.AddFirst(timeline);

		string display = timeline.time + " " + timeline.done + " " + timeline.timerRunning + " " + timeline.forwardBackward;

		//GD.Print(rewindHistory.Count, ": " , display);
		//GD.Print(display);
	}
}
