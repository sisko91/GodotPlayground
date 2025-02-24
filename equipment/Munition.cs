using Godot;
using System;
using System.ComponentModel.Design;

namespace First2DGame
{
    // The IMunition interface must be implemented by any type that is fired from a weapon.
    public interface IMunition
    {
        // The velocity of the projectile/burst/etc.
        Vector2 Velocity { get; set; }

        // The position of the projectile/burst/etc.
        Vector2 Position { get; set; }
    }
}