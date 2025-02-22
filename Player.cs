using Godot;
using System;

public partial class Player : Area2D
{
	public event Action HitEventHandler;

	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

	private Vector2 lastDirection = new Vector2(0, 1);

	public Vector2 ScreenSize; // Size of the game window.
							   
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var velocity = Vector2.Zero; // The player's movement vector.

		if (Input.IsActionPressed("move_right")) {
			velocity.X += 1;
		}

		if (Input.IsActionPressed("move_left")) {
			velocity.X -= 1;
		}

		if (Input.IsActionPressed("move_down")) {
			velocity.Y += 1;
		}

		if (Input.IsActionPressed("move_up")) {
			velocity.Y -= 1;
		}

		if (!velocity.IsZeroApprox()) {
			lastDirection = velocity.Normalized();
		}

		if (Input.IsActionJustPressed("fire")) {
			Fireball fb = (Fireball) ResourceLoader.Load<PackedScene>("res://fireball.tscn").Instantiate();
			fb.Position = Position;
			fb.Velocity = lastDirection * Speed * 2;
            GetNode<AudioStreamPlayer>("Fireball").Play();
            GetParent().AddChild(fb);
		}

		var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0) {
			velocity = velocity.Normalized() * Speed;
			animatedSprite2D.Play();
		} else {
			animatedSprite2D.Stop();
		}

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
			y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
		);

		animatedSprite2D.Rotation = 0;
		if (velocity.Y != 0) {
			animatedSprite2D.Animation = "up";
			//animatedSprite2D.FlipV = velocity.Y > 0;

			float degrees = velocity.Y > 0 ? 180 : 0;
			if (velocity.X != 0) {
				degrees += Math.Sign(velocity.X) * Math.Sign(velocity.Y) * -45;
			}

			animatedSprite2D.Rotation = degrees * (float)Math.PI / 180f;
		}
		else if (velocity.X != 0) {
			animatedSprite2D.Animation = "walk";
			animatedSprite2D.FlipH = velocity.X < 0;
		}

		//Hide();
	}

	private void OnBodyEntered(Node2D body) {
		Hide(); // Player disappears after being hit.
		HitEventHandler?.Invoke();
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	public void Start(Vector2 position) {
		Position = position;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}
}
