using Godot;
using System;
using System.Linq;

public partial class YoYoLauncher : Path2D
{
	[Export]
	private Area2D area;

	private int powerLevel = 0;
	private float time = 0.0f;
	private bool launched = false;
	private float travelDistance = 0.0f;

	private Sprite2D powerLevel1;
	private Sprite2D powerLevel2;
	private Sprite2D powerLevel3;
	private Sprite2D powerLevel4;
	private Sprite2D powerLevel5;

	private PathFollow2D followPath;

	private enum YoYoState { Charging, Idle, Receiving };
	private YoYoState state = YoYoState.Charging;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		powerLevel1 = GetNode<Node2D>("PowerLevel").GetNode<Sprite2D>("Power1");
		powerLevel2 = GetNode<Node2D>("PowerLevel").GetNode<Sprite2D>("Power2");
		powerLevel3 = GetNode<Node2D>("PowerLevel").GetNode<Sprite2D>("Power3");
		powerLevel4 = GetNode<Node2D>("PowerLevel").GetNode<Sprite2D>("Power4");
		powerLevel5 = GetNode<Node2D>("PowerLevel").GetNode<Sprite2D>("Power5");

		followPath = GetNode<PathFollow2D>("PathFollow2D");

		area.BodyEntered += (body) => ActivateLauncher(body);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(state)
		{
			case YoYoState.Charging:
				IncDecPower((float)delta);

				PowerDisplay();

				if (launched)
				{
					state = YoYoState.Idle;
					travelDistance = 0.2f * powerLevel;
					powerLevel = 0;
					time = 0.0f;
				}

				break;
			case YoYoState.Idle:

				launched = false;

				PowerDisplay();

				followPath.ProgressRatio = Mathf.Clamp(followPath.ProgressRatio + 0.005f, 0.0f, travelDistance);

				if (followPath.ProgressRatio >= travelDistance)
				{
					state = YoYoState.Receiving;
				}

				break;
			case YoYoState.Receiving:

				followPath.ProgressRatio = Mathf.Clamp(followPath.ProgressRatio - 0.005f, 0.0f, travelDistance);

				if (followPath.ProgressRatio <= 0.0f)
				{
					state = YoYoState.Charging;
				}

				break;
		}


	}

	private void IncDecPower(float delta)
	{
		time = (float) Mathf.Clamp(time + delta, 0.0, 10.0f);


		if (time > 9.99)
		{
			powerLevel = 5;
		}
		else if (time > 7.99)
		{
			powerLevel = 4;
		}
		else if (time > 5.99)
		{
			powerLevel = 3;
		}
		else if (time > 3.99)
		{
			powerLevel = 2;
		}
		else if (time > 1.99)
		{
			powerLevel = 1;
		}
		else
		{
			powerLevel = 0;
		}

		//GD.Print("Power Level: ", powerLevel);
    }

	private void PowerDisplay()
	{
		HideReveal(powerLevel5, powerLevel == 5 ? 1.0f : 0.0f);
		HideReveal(powerLevel4, powerLevel >= 4 ? 1.0f : 0.0f);
		HideReveal(powerLevel3, powerLevel >= 3 ? 1.0f : 0.0f);
		HideReveal(powerLevel2, powerLevel >= 2 ? 1.0f : 0.0f);
		HideReveal(powerLevel1, powerLevel >= 1 ? 1.0f : 0.0f);
	}

	private void HideReveal(Sprite2D img, float alpha)
	{
		Color tmp = img.Modulate;
		tmp.A = alpha;
		img.Modulate = tmp;
	}

	private void ActivateLauncher(Node2D body)
	{
        if (body is CharacterBody2D)
        {
            CharacterBody2D tmp = (CharacterBody2D) body;

			if (tmp.CollisionLayer == 1)
			{
				launched = true;
			}
        }
	}
}
