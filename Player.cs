using Godot;
using System;

public partial class Player : Area2D
{
	public event Action HitEventHandler;

	[Export]
	public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

	// What direction the player is aiming in. Updated each tick by HandleAim().
	private Vector2 aimDirection = new Vector2(0, 0);

	// What direction the player last moved in. Updated each tick in HandleMove().
	private Vector2 lastDirection = new Vector2(0, 0);

	public Vector2 ScreenSize; // Size of the game window.

    private bool bUsingGamepad = false;
							   
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScreenSize = GetViewportRect().Size;
	}

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if(@event is InputEventJoypadButton || @event is InputEventJoypadMotion)
        {
            bUsingGamepad = true;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

        HandleMove(delta);
        HandleAim(delta);
        HandleFire(delta);

        //Hide();
    }

	private void HandleMove(double delta)
	{
        Vector2 velocity = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        if (!velocity.IsZeroApprox())
        {
            lastDirection = velocity.Normalized();
        }

        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            animatedSprite2D.Play();
        }
        else
        {
            animatedSprite2D.Stop();
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            x: Mathf.Clamp(Position.X, 0, ScreenSize.X),
            y: Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );

        animatedSprite2D.Rotation = 0;
        if (velocity.Y != 0)
        {
            animatedSprite2D.Animation = "up";
            //animatedSprite2D.FlipV = velocity.Y > 0;

            float degrees = velocity.Y > 0 ? 180 : 0;
            if (velocity.X != 0)
            {
                degrees += Math.Sign(velocity.X) * Math.Sign(velocity.Y) * -45;
            }

            animatedSprite2D.Rotation = degrees * (float)Math.PI / 180f;
        }
        else if (velocity.X != 0)
        {
            animatedSprite2D.Animation = "walk";
            animatedSprite2D.FlipH = velocity.X < 0;
        }
    }

    private void HandleAim(double delta)
    {
        Vector2 aimVector = Input.GetVector("aim_left", "aim_right", "aim_up", "aim_down");

        if(!aimVector.IsZeroApprox())
        {
            aimDirection = aimVector;
        }
        else
        {
            // if no aim was provided we can derive an aim direction from player movement.
            aimDirection = lastDirection;
        }

    }

    private void HandleFire(double delta)
    {
        if (Input.IsActionJustPressed("fire"))
        {
            var finalAimDir = aimDirection;
            if(!bUsingGamepad)
            {
                // Consider mouse position as well, and calculate the aim direction from where the mouse is relative to the player.
                var mousePos = GetViewport().GetMousePosition();
                finalAimDir = (mousePos - Position).Normalized();
            }
            Fireball fb = (Fireball)ResourceLoader.Load<PackedScene>("res://fireball.tscn").Instantiate();
            fb.Position = Position;
            fb.Velocity = finalAimDir.IsZeroApprox() ? new Vector2(0, 1) : finalAimDir * Speed * 2;
            GetNode<AudioStreamPlayer>("Fireball").Play();
            GetParent().AddChild(fb);
        }
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
