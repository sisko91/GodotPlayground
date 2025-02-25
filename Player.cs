using First2DGame;
using Godot;
using System;

public partial class Player : Area2D
{
    private static double DASH_COOLDOWN_SECONDS = 2;

    public event Action HitEventHandler;

    [Export]
    public int Speed { get; set; } = 400; // How fast the player will move (pixels/sec).

    [Export]
    public PackedScene PrimaryWeaponTemplate { get; set; }

    private Weapon primaryWeapon = null;

    // What direction the player last moved in. Updated each tick in HandleMove().
    private Vector2 lastDirection = new Vector2(0, 0);

    public Vector2 ScreenSize; // Size of the game window.

    private bool bUsingGamepad = false;

    private double lastDashTime = Time.GetTicksMsec() / 1000.0 - DASH_COOLDOWN_SECONDS;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
        if(PrimaryWeaponTemplate != null)
        {
            primaryWeapon = PrimaryWeaponTemplate.Instantiate<Weapon>();
            // Add the weapon as a child of the player so that it has relative coordinates, orientation, etc.
            AddChild(primaryWeapon);
        }
        else
        {
            GD.PushError("No weapon assigned. Can't play.");
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if(@event is InputEventJoypadButton)
        {
            bUsingGamepad = true;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        HandleZoom();
        HandleMove(delta);
        HandleAim(delta);
        HandleFire(delta);

        //Hide();
    }

    private void HandleZoom() {
        var camera = GetNode<Camera2D>("Camera2D");
        if (Input.IsActionJustPressed("zoom_in")) {
            camera.Zoom = camera.Zoom * new Vector2(1.05f, 1.05f);
        } else if (Input.IsActionJustPressed("zoom_out")) {
            camera.Zoom = camera.Zoom * new Vector2(0.95f, 0.95f);
        }
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

        double currentTimeSeconds = Time.GetTicksMsec() / 1000.0;
        float moveMultiplier = 1;
        if (Input.IsActionJustPressed("dash") && currentTimeSeconds - lastDashTime > DASH_COOLDOWN_SECONDS) {
            moveMultiplier = 30;
            lastDashTime = currentTimeSeconds;
        }

        Position += velocity * (float)delta * new Vector2(moveMultiplier, moveMultiplier);

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
            primaryWeapon.Rotation = aimVector.Angle();
        }
        else
        {
            // if no aim was provided we can derive an aim direction from player movement.
            primaryWeapon.Rotation = lastDirection.Angle();
        }

    }

    private void HandleFire(double delta)
    {
        if(primaryWeapon == null)
        {
            GD.PushError("No weapon equipped; cannot fire.");
            return;
        }
        if (Input.IsActionPressed("fire"))
        {
            if (!bUsingGamepad)
            {
                // Consider mouse position as well, and calculate the aim direction from where the mouse is relative to the player.
                var mousePos = GetGlobalMousePosition();
                // Set the weapon's global rotation so that it accounts for rotations of the player as well.
                // TODO: Maybe don't rotate the weapon?
                primaryWeapon.GlobalRotation = (mousePos - Position).Angle();
            }
            var munition = primaryWeapon.TryFire();
            if(munition != null)
            {
                GetParent().AddChild((Node)munition);
            }
        }
    }

    private void OnBodyEntered(Node2D body) {
        HitEventHandler?.Invoke();
        QueueFree();
    }

    public void Start(Vector2 position) {
        Position = position;
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }
}
