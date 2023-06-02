using Godot;
using System;

public partial class FlyingBirdEnemy : Path2D
{
	// Called when the node enters the scene tree for the first time.

	AnimatedSprite2D animatedSprite2D = null;
	PathFollow2D pathFollow2D = null;
	Vector2 direction = Vector2.Right;
	float lastPosition = 0;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("PathFollow2D/Area2D/AnimatedSprite2D");
		pathFollow2D = GetNode<PathFollow2D>("PathFollow2D");
		var newPosistion = pathFollow2D.Progress;
		
		//GD.Print(pathFollow2D.Progress);
		GD.Print(lastPosition);

		if (newPosistion > lastPosition)
            animatedSprite2D.FlipH = true;
        if (newPosistion < lastPosition)
            animatedSprite2D.FlipH = false;

		lastPosition = newPosistion;
	}

	private void _on_hitbox_body_entered(CharacterBody2D body)
	{
		GD.Print("Body: " + body.Name + "has entered");

		//Call function from player
		if (body.Name == "player")
		{
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerDie");
		}
	}

	private void _on_death_hit_box_body_entered(CharacterBody2D body)
	{
		GD.Print("Body: " + body.Name + "has entered");

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
