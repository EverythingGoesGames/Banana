using Godot;

public partial class SyncCharObject : CharacterBody2D
{
	[Export]
	protected SynchronousData syncData;

	[Export]
	protected float speed = 50.0f;

	[Export]
	protected int xDir = 0;

	[Export]
	protected int yDir = 0;

	protected Vector2 velocity;

	protected Label debugText; // Testing purpose

	protected void PlatformMovement(float speed)
	{
		velocity = Velocity;

		velocity.Normalized();

		velocity.X = speed * xDir;
		velocity.Y = speed * yDir;

		Velocity = velocity;
	}
}