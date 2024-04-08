using Godot;
using System;

public partial class wallBox : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_area_entered(Area2D area)
	{
		// Replace with function body.

		GD.Print("area: " + area.Name + " entered");

		string areaName = area.Name;

		bool containsProjectileName = areaName.Contains("Boss1Projectile");

		if(containsProjectileName)
		{
			GD.Print("wall box entered");
			var projectile = GetNode<Area2D>(area.GetPath());
			projectile.Call("PlayAnimation", "splat");
			projectile.Call("StopMoving");
		}
	}
}



