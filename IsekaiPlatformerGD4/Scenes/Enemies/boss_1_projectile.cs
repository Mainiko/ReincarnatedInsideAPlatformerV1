using Godot;
using System;

public partial class boss_1_projectile : Area2D
{
	[Export] private int Speed = 200;
	private Vector2 direction = new Vector2(-1, 0);
	private float rotation;
	private bool stopMoving = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		//this one makes its so the wallbox does not trigger the _on_area_entered function ???
		//PlayAnimation("new_animation"); 

		if (!stopMoving) { 
			GlobalPosition += (float)(Speed * delta) * direction;
			this.RotationDegrees = rotation;
		}

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
		direction = vector;
		rotation = degrees;
	}

	private void PlayAnimation(string animation)
	{

		AnimationPlayer animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play(animation);
		GD.Print("PlayAnimation called " + animation + " Animationplayer now playing: " + animationPlayer.CurrentAnimation);

	}

	private void StopMoving()
	{
		stopMoving = true;
		GD.Print("Stopped moving");
	}

}





