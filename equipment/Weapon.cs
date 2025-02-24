using Godot;
using System;

// Note: We have to be in our configured namespace or Godot won't allow us to select this node type in dropdowns.
namespace First2DGame
{

	// Weapon is the base type that all equippable weapon types extend.
	public partial class Weapon : Node2D
	{

		// How fast this weapon can fire.
		[Export]
		public float FireRate { get; set; }

		// How powerful the weapon fired. This influences the velocity of projectiles for example.
		[Export]
		public float FirePower { get; set; }

		// What type of project / ammo / gameplay entity is spawned (fired) from this weapon.
		[Export]
		public PackedScene MunitionsTemplate { get; set; }

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		// Fires the weapon if possible at the current time. Returns the new munition instance fired if successful.
		public IMunition TryFire()
		{
			if (MunitionsTemplate != null)
			{
				var rawInstance = MunitionsTemplate.Instantiate();
				if(rawInstance == null)
				{
					GD.PushError("Munitions Instance was null!");
					return null;
				}

				if (rawInstance is IMunition munitionInstance)
				{
					// We initialize munitions using *Global* coordinates and orientation so that the projectile is not tied to the weapon or the player itself.
					munitionInstance.Position = GlobalPosition;
					munitionInstance.Velocity = Vector2.FromAngle(GlobalRotation) * FirePower;
					return munitionInstance;
				}
				else
				{
					// Delete the instance because it's not valid.
					// TODO: Figure out how to prevent assigning a type that doesn't implement IMunition.
					rawInstance.Free();
					GD.PushWarning("MunitionsTemplate does not implement IMunition: " + MunitionsTemplate.ToString());
					return null;
				}
			}
            GD.PushWarning("MunitionsTemplate is null!");
            return null;
		}
	}

}