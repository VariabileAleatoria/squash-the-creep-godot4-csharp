using Godot;

public partial class Mob : CharacterBody3D
{
	// Don't forget to rebuild the project so the editor knows about the new export variable.

	// Minimum speed of the mob in meters per second
	[Export]
	public int MinSpeed = 10;
	// Maximum speed of the mob in meters per second
	[Export]
	public int MaxSpeed = 18;
	// Emitted when the played jumped on the mob.
	[Signal]
	public delegate void SquashedEventHandler();

	// We will call this function from the Main scene
	public void Initialize(Vector3 startPosition, Vector3 playerPosition)
	{
		// We position the mob and turn it so that it looks at the player.
		LookAtFromPosition(startPosition, playerPosition, Vector3.Up);
		// And rotate it randomly so it doesn't move exactly toward the player.
		RotateY((float)GD.RandRange(-Mathf.Pi / 4.0, Mathf.Pi / 4.0));
		float randomSpeed = (float)GD.RandRange(MinSpeed, MaxSpeed);
		// We calculate a forward velocity that represents the speed.
		Velocity = Vector3.Forward * randomSpeed;
		// We then rotate the vector based on the mob's Y rotation to move in the direction it's looking
		Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);
		GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = randomSpeed / MinSpeed;
	}

	public override void _PhysicsProcess(double delta)
	{
		MoveAndSlide();
	}
	
	private void _on_visible_on_screen_notifier_3d_screen_exited()
	{
		// Replace with function body.
		QueueFree();
	}

	public void Squash()
	{
		EmitSignal(nameof(Squashed));
		QueueFree();
	}
}



