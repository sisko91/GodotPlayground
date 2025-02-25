using Godot;
using System;

public partial class Mob : CharacterBody2D {
    public static int MOB_LIMIT = 15;

    public float Speed { get; set; }
    private Vector2 velocity;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames();
        animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta) {
        HandleMove(delta);
    }

    public void UpdateVelocity(Vector2 playerPos) {
        // Move the mob towards the player
        Vector2 normalToPlayer = (playerPos - Position).Normalized();
        float direction = normalToPlayer.Angle();

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        Rotation = direction;

        velocity = new Vector2(Speed, 0).Rotated(direction);
    }

    private void HandleMove(double delta) {
        Position += velocity * (float)delta;
    }
}