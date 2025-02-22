using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    private int _score;

    private Player playerInstance;
    
	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //NewGame();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public void GameOver() {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();

        GetNode<Hud>("HUD").ShowGameOver();
        GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
    }

    public void NewGame() {
        _score = 0;

        var playerScene = GD.Load<PackedScene>("res://Player.tscn"); // Will load when the script is instanced.
        playerInstance = playerScene.Instantiate<Player>();
        playerInstance.Name = "Player";
        AddChild(playerInstance);

        if(playerInstance != null)
        {
            var startPosition = GetNode<Marker2D>("StartPosition");
            playerInstance.HitEventHandler += GameOver;
            playerInstance.Start(startPosition.Position);
        }

        GetNode<Timer>("StartTimer").Start();

        var hud = GetNode<Hud>("HUD");
        hud.UpdateScore(_score);
        hud.ShowMessage("Get Ready!");
        GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
        GetNode<AudioStreamPlayer>("Music").Play();
    }

    private void OnScoreTimerTimeout() {
        _score++;
        GetNode<Hud>("HUD").UpdateScore(_score);
    }
    
    private void OnStartTimerTimeout() {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }

    private void OnMobTimerTimeout() {
        // Create a new instance of the Mob scene.
        Mob mob = MobScene.Instantiate<Mob>();

        // Choose a random location on Path2D.
        //var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        //mobSpawnLocation.ProgressRatio = GD.Randf();
        var viewportRect = GetNode<Hud>("HUD").GetViewport().GetVisibleRect();
        uint side = GD.Randi() % 4;

        float x = 0; float y = 0;

        switch(side)
        {
            case 0: // Top
                x = viewportRect.Position.X + viewportRect.Size.X * GD.Randf();
                y = viewportRect.Position.Y;
                break;
            case 1: // Right
                x = viewportRect.End.X;
                y = viewportRect.Position.Y + viewportRect.Size.Y * GD.Randf();
                break;
            case 2: // Bottom
                x = viewportRect.Position.X + viewportRect.Size.X * GD.Randf();
                y = viewportRect.End.Y;
                break;
            case 3: // Left
                x = viewportRect.End.X;
                y = viewportRect.Position.Y + viewportRect.Size.Y * GD.Randf();
                break;
        }
        mob.Position = new Vector2(x, y);

        // Set the mob's direction perpendicular to the path direction.
        //float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;\
        var playerPos = playerInstance.Position;
        Vector2 normalToPlayer = (playerPos - mob.Position).Normalized();
        float direction = normalToPlayer.Angle();

        // Set the mob's position to a random location.
        //mob.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        // Choose the velocity.
        var velocity = new Vector2((float)GD.RandRange(150.0, 250.0), 0);
        mob.LinearVelocity = velocity.Rotated(direction);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
    }
}
