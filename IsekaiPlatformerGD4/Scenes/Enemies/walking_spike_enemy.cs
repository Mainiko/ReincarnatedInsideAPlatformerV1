using Godot;
using System;

public partial class walking_spike_enemy : CharacterBody2D
{
	AnimatedSprite2D animatedSprite2D = null;
	[Export] private float walkingSpeed = 50.0f;


	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	Vector2 direction = Vector2.Right;

	bool hasDied = false;
	bool firsttime = true;


	public async override  void _PhysicsProcess(double delta)
	{	 
		Vector2 velocity = Velocity;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		var LedgeCheckRight = GetNode<RayCast2D>("LedgeCheckRight");
		var ledgeCheckLeft = GetNode<RayCast2D>("LedgeCheckLeft");



		if (firsttime)
		{
			firsttime = false;
		}

		else
		{
			if (!IsOnFloor())
				velocity.Y += gravity * (float)delta;

			bool found_wall = IsOnWall();
			var found_ledge = LedgeCheckRight.IsColliding() && ledgeCheckLeft.IsColliding();
			if (found_wall || !found_ledge)
			{
				if(direction.X > 0 && LedgeCheckRight.IsColliding())
				{
					//do nothing

				}
				else if(direction.X < 0 && ledgeCheckLeft.IsColliding())
				{
					//do nothing

				}
				else
				{
					//GD.Print(direction.X);
					direction *= -1;
					//GD.Print(direction.X);
					//GD.Print(found_ledge);
					//GD.Print(animatedSprite2D);
				}

			}


			if (direction.X > 0)
				animatedSprite2D.FlipH = true;
			if (direction.X < 0)
				animatedSprite2D.FlipH = false;


			float Speed = walkingSpeed;
			velocity.X = direction.X * Speed;
			Velocity = velocity;
		}
	
		MoveAndSlide();
		CheckIfDead();
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
			hasDied = true;
			animatedSprite2D.Play("Explode");
			RemoveHitBoxes();
		}
	}

	private void CheckIfDead()
	{
		if (hasDied)
		{
			if (animatedSprite2D.Animation == "Explode" && animatedSprite2D.IsPlaying() == false)
			{
				this.QueueFree();
			}
		}
	}

	private void RemoveHitBoxes()
	{
		var deathHitbox = GetNode<Area2D>("Death_HitBox");
		var hitbox = GetNode<Area2D>("Hitbox");

		deathHitbox.QueueFree();
		hitbox.QueueFree();
	}

	private void SetDirection(Vector2 vector, float degrees)
	{
		GD.Print("ost");
		direction = vector;
	}

}
