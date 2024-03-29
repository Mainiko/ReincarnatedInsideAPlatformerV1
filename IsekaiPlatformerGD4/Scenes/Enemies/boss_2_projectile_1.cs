using Godot;
using System;

public partial class boss_2_projectile_1 : Area2D
{
	[Export] private int Speed = 200;
	private Vector2 direction = new Vector2(-1, 0);
	private float rotation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition += (float)(Speed * delta) * direction;
		this.RotationDegrees = rotation;
	}

	private void _on_visible_on_screen_notifier_2d_screen_exited()
	{
		GD.Print("out of screen");
		this.QueueFree();
	}

	private void _on_body_entered(Node2D body)
	{
		if (body.Name == "player")
		{
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerDie");
		}
	}
	private void SetDirection(Vector2 vector, float degrees)
	{
		GD.Print("ost");
		direction = vector;
		rotation = degrees;
	}

}


