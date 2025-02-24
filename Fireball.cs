using First2DGame;
using Godot;
using System;

public partial class Fireball : Area2D, IMunition
{
    public Vector2 Velocity { get; set; }
    public Vector2 ScreenSize; // Size of the game window

    [Export]
    public NodePath AudioTrack { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        ScreenSize = GetViewportRect().Size;

        if(AudioTrack != null)
        {
            var track = GetNode<AudioStreamPlayer>(AudioTrack);
            if (track != null) {
                track.Play();
            }
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        Position += Velocity * (float)delta;
        
        if (Position.X > ScreenSize.X || Position.X < 0 || Position.Y > ScreenSize.Y || Position.Y < 0) {
            QueueFree();
        }
    }

    private void OnBodyEntered(Node2D body) {
        body.QueueFree();
        QueueFree();
    }
}
