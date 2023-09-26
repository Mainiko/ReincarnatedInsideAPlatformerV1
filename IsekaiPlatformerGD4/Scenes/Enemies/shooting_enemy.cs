using Godot;
using System;

public partial class shooting_enemy : StaticBody2D
{
	bool shot = true;
	[Export] private double shotSpeed = 1.0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override async void _Process(double delta)
	{
		if (shot)
		{
			Vector2 direction = new Vector2(-1, 0);
			var projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/boss_1_projectile.tscn").Instantiate();
			projectile.Call("SetDirection", direction, 180);
			shot = false;
			await ToSignal(GetTree().CreateTimer(shotSpeed), "timeout");
			AddChild(projectile);
			shot = true;
		}

	}

		
	private void _on_hitbox_body_entered(Node2D body)
	{
		//GD.Print("Body: " + body.Name + "has entered");

		//Call function from player
		if (body.Name == "player")
		{
			GD.Print("Kill me please!");
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerJumpOnEnemy");
			this.QueueFree();
		}
	}
}


