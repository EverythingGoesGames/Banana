using Godot;
using System.Collections.Generic;
using System.Linq;
using SynchronousStates;

public partial class TimeBlock : StaticBody2D
{
	[Export]
	private float countdown = 1.0f;

	[Export]
	SynchronousData syncData;

	private float time = 0.0f;

	private AnimationPlayer player;
	private Area2D area;
	private Label label;
	private CollisionShape2D collision;

	private Label debugText; // Testing purpose

	// Initiate the process of destroying the time block
	private bool timerRunning = false;
	private bool done = false;
	private bool disppeared = false;
	private string forwardBackward = "do_nothing";


	// Rewind feature
	LinkedList<Rewind> rewindHistory = new LinkedList<Rewind>();
	

	public struct Rewind
	{
		public float time;
		public bool timerRunning;
		public bool done;
		public bool disappeared;
		public bool collsionEnabled;
		public bool areaEnabled;
		public string forwardBackward;
	};



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		syncData.SetHistoryLimit(syncData.GetRewindLength());
		syncData.SetRewindArea(GetNode<Area2D>("RewindArea"));

		player = GetNode<AnimationPlayer>("AnimationPlayer");
		player.AnimationFinished += (body) => AnimationFinished();

		area = GetNode<Area2D>("Area2D");
		area.BodyEntered += (body) => InitiateDestruction((CharacterBody2D)body);

		collision = GetNode<CollisionShape2D>("CollisionShape2D");

		label = GetNode<Label>("Label");
		label.Text = GD.VarToStr(countdown);

		debugText = GetNode<Label>("DebugText");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(syncData.GetTimeState())
		{
			case TimeStates.Idle:
				if (timerRunning == true)
				{
					time += (float)delta;
				}

				// Display the time on the block with 1 decimal place
				if ((!done) && (!disppeared))
				{
					float num = Mathf.Clamp(countdown - time, 0.0f, countdown);
					label.Text = num.ToString("N1");
				}
				else
				{
					label.Text = "";
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

				// Testing the animation
				if (Input.IsActionJustPressed("test"))
				{
					timerRunning = true;
				}

				// Time rewind mechanic
				RecordHistory();
				break;
			case TimeStates.Actvated:

				Rewind record = rewindHistory.First();

				// Assign essential properties for rewind effect
				time = record.time;
				done = record.done;
				disppeared = record.disappeared;
				collision.Disabled = record.collsionEnabled;
				area.Monitoring = record.areaEnabled;
				timerRunning = record.timerRunning;
				forwardBackward = record.forwardBackward;

				rewindHistory.RemoveFirst();


				if (record.forwardBackward.Equals("play_backwards"))
				{
					player.Call(forwardBackward, "Destroyed");
				}
				else if ((rewindHistory.Count == 0) && (record.forwardBackward.Equals("play_backwards")))
				{
					forwardBackward ="play";
					player.Call(forwardBackward, "Destroyed");
				}
				else if ((rewindHistory.Count == 0) && (record.forwardBackward.Equals("play")))
				{
					player.Call(forwardBackward, "Destroyed");
				}

				if (!player.IsPlaying() && (timerRunning))
				{
					float num = Mathf.Clamp(countdown - time, 0.0f, countdown);
					label.Text = num.ToString("N1");
				}

				if (rewindHistory.Count == 0)
				{
					syncData.SetTimeState(TimeStates.Idle);
				}
				break;
			case TimeStates.Exited:
				break;
		}

		debugText.Text = "Rewind: " + syncData.GetRewindLength().ToString() + "s";
	}

	private void AnimationFinished()
	{
		time = 0.0f;
		forwardBackward = "do_nothing";
		disppeared = true;
	}

	private void InitiateDestruction(CharacterBody2D body)
	{
		if (body.CollisionLayer == 1)
		{
			timerRunning = true;
		}
	}

	private void RecordHistory()
	{
		if (rewindHistory.Count >= syncData.GetHistoryLimit())
		{
			if (rewindHistory.Last.Value.disappeared)
			{
				rewindHistory.Clear();
				syncData.SetTimeState(TimeStates.Exited);
				return;
			}
			else
			{
				rewindHistory.RemoveLast();
			}
		}

		Rewind timeline;
		timeline.time = time;
		timeline.done = done;
		timeline.disappeared = disppeared;
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
	}
}