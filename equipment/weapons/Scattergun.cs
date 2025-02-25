using First2DGame;
using Godot;
using System;

public partial class Scattergun : Weapon
{
    public override IMunition TryFire() {
        if (IsPreMatureFire()) { return null; }
        var munitionInstances = new IMunition[3] { getAndValidateMunitions(), getAndValidateMunitions(), getAndValidateMunitions() };
        if (munitionInstances[0] == null) { return null; }

        munitionInstances[0].Position = GlobalPosition;
        munitionInstances[0].Velocity = Vector2.FromAngle(GlobalRotation + 10 * (float) Math.PI / 180f) * FirePower;
        ((Node2D)munitionInstances[0]).Scale = new Vector2(0.2f, 0.2f);

        munitionInstances[1].Position = GlobalPosition;
        munitionInstances[1].Velocity = Vector2.FromAngle(GlobalRotation - 10 * (float)Math.PI / 180f) * FirePower;
        ((Node2D)munitionInstances[1]).Scale = new Vector2(0.2f, 0.2f);

        munitionInstances[2].Position = GlobalPosition;
        munitionInstances[2].Velocity = Vector2.FromAngle(GlobalRotation) * FirePower;
        ((Node2D)munitionInstances[2]).Scale = new Vector2(0.2f, 0.2f);

        //TODO: Fix to be consistent
        GetParent().GetParent().AddChild((Node) munitionInstances[0]);
        GetParent().GetParent().AddChild((Node)munitionInstances[1]);
        GetParent().GetParent().AddChild((Node)munitionInstances[2]);
        return null;
    }
}
