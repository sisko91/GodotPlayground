        using First2DGame;
using Godot;
using System;

public partial class Fireball : Area2D, IMunition
{
    public Vector2 Velocity { get; set; }
    public Vector2 ScreenSize; // Size of the game window

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        ScreenSize = GetViewportRect().Size;
        var track = GetNode<AudioStreamPlayer>("Fireball_Shoot");
        track.Play();
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
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        animatedSprite2D.Play();
        animatedSprite2D.AnimationFinished += Hide;

        var track = GetNode<AudioStreamPlayer>("Fireball_Explode");
        //Doesn't work, framework bug maybe
        //track.Finished += QueueFree;
        //track.Finished += body.QueueFree;
        track.Play();

        var timer = GetTree().CreateTimer(0.15);
        timer.Timeout += body.QueueFree;
        timer.Timeout += QueueFree;
    }
}
