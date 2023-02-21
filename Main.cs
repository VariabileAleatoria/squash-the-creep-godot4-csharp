using Godot;

public partial class Main : Node
{
	// Don't forget to rebuild the project so the editor knows about the new export variable.

	#pragma warning disable 649
	[Export]
	public PackedScene MobScene;
	#pragma warning restore 649

	public override void _Ready()
	{
		GD.Randomize();
		GetNode<Control>("UserInterface/Retry").Hide();
	}
	private void OnMobTimerTimeout()
	{
		// Create a new instance of the Mob scene.
		Mob mob = (Mob)MobScene.Instantiate();

		// Choose a random location on the SpawnPath.
		// We store the reference to the SpawnLocation node.
		var mobSpawnLocation = GetNode<PathFollow3D>("SpawnPath/SpawnLocation");
		// And give it a random offset.
		mobSpawnLocation.ProgressRatio = GD.Randf();

		Vector3 playerPosition = GetNode<Player>("Player").Position;
		mob.Initialize(mobSpawnLocation.Position, playerPosition);
		mob.Squashed += GetNode<ScoreLabel>("UserInterface/ScoreLabel").OnMobSquashed;
		AddChild(mob);
	}
	private void OnPlayerHit()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Control>("UserInterface/Retry").Show();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_accept") && GetNode<Control>("UserInterface/Retry").Visible)
		{
			// This restarts the current scene.
			GetTree().ReloadCurrentScene();
		}
	}

}




