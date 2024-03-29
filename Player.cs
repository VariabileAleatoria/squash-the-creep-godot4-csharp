using Godot;

public partial class Player : CharacterBody3D
{
	// How fast the player moves in meters per second.
	[Export]
	public int Speed = 14;
	// The downward acceleration when in the air, in meters per second squared.
	[Export]
	public int FallAcceleration = 75;
	// Vertical impulse applied to the character upon jumping in meters per second.
	[Export]
	public int JumpImpulse = 20;
	// Vertical impulse applied to the character upon bouncing over a mob in meters per second.
	[Export]
	public int BounceImpulse = 16;
	// Emitted when the player was hit by a mob.
	[Signal]
	public delegate void HitEventHandler();

	private Vector3 _velocity = Vector3.Zero;

	public override void _PhysicsProcess(double delta)
	{
		// We create a local variable to store the input direction.
		_velocity = Velocity;
		var direction = Vector3.Zero;

		// We check for each move input and update the direction accordingly
		if (Input.IsActionPressed("move_right"))
		{
			direction.X += 1f;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction.X -= 1f;
		}
		if (Input.IsActionPressed("move_back"))
		{
			// Notice how we are working with the vector's x and z axes.
			// In 3D, the XZ plane is the ground plane.
			direction.Z += 1f;
		}
		if (Input.IsActionPressed("move_forward"))
		{
			direction.Z -= 1f;
		}

		if (direction != Vector3.Zero)
		{
			direction = direction.Normalized();
			GetNode<Node3D>("Pivot").LookAt(Position + direction, Vector3.Up);
			GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 4;
		}
		else
		{
			GetNode<AnimationPlayer>("AnimationPlayer").SpeedScale = 1;
		}

		// Ground velocity
		_velocity.X = direction.X * Speed;
		_velocity.Z = direction.Z * Speed;
		// Vertical velocity
		_velocity.Y -= FallAcceleration * (float) delta;
		// Moving the character
			// Jumping.
		if (IsOnFloor() && Input.IsActionJustPressed("jump"))
		{
			_velocity.Y = JumpImpulse;
		}
		for (int index = 0; index < GetSlideCollisionCount(); index++)
		{
			// We check every collision that occurred this frame.
			KinematicCollision3D collision = GetSlideCollision(index);
			// If we collide with a monster...
			if (collision.GetCollider() is Mob mob)
			{
				// ...we check that we are hitting it from above.
				if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
				{
					// If so, we squash it and bounce.
					mob.Squash();
					_velocity.Y = BounceImpulse;
				}
			}
		}
		Velocity = _velocity;
		MoveAndSlide();
		var pivot = GetNode<Node3D>("Pivot");
		pivot.Rotation = new Vector3(Mathf.Pi / 6f * _velocity.Y / JumpImpulse, pivot.Rotation.Y, pivot.Rotation.Z);
	}

	private void Die()
	{
		EmitSignal(nameof(Hit));
		QueueFree();
	}

	private void OnMobDetectorBodyEntered(Node3D body)
	{
		Die();
	}
}




