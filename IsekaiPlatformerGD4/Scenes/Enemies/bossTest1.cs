using Godot;
using System;
using System.Threading.Tasks;

public partial class bossTest1 : CharacterBody2D
{
	[Export] private int Speed = 100;
	[Export] private int timebeforeCharging = 1000;
	[Export] private int timebeforeMoving = 100;
	[Export] private int timeNeddedtoHit = 3;

	

	int timesPlayerHaveHit = 0;
	int timeSinceStoped = 0;
	bool spikeIsActive = false;
	bool chargeMode = false;
	bool confused = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	Vector2 direction = Vector2.Left;
	




	public async override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Area2D Spikes = GetNode<Area2D>("HitboxDeath");
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");



		if (IsOnWall())
			{
				if (chargeMode)
				{
					if (confused)
					{
					   Speed = 0;
					   spikeIsActive = true;
				 	}
					else
					{
					confused = true;
					await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
					spikeIsActive = false;
					chargeMode = false;
					confused = false;
					direction *= -1;
					Speed = 100;
					GD.Print("My speed" + Speed);
					GD.Print("On wall?" + IsOnWall());
					animatedSprite2D.Play("Walking");
				}

			}
				else
				{
					animatedSprite2D.Play("Walking");
					GD.Print("outside of chargemode ");
					direction *= -1;
					Speed = 100;
				}
			}

			if (spikeIsActive)
			{
				Spikes.Visible = true;
				Spikes.CollisionLayer = 1;
			}
			else if (!spikeIsActive)
			{
				Spikes.Visible = false;
				Spikes.CollisionLayer = 0;
			}
		
		velocity.X = direction.X * Speed;
		Velocity = velocity;
		MoveAndSlide();
	}

	private async void _on_hitbox_body_entered(CharacterBody2D body)
	{
		GD.Print("Body: " + body.Name + "has entered");
		GD.Print("_on_hitbox_body_entered");


		Area2D Spikes = GetNode<Area2D>("HitboxDeath");
   
		var player = GetNode<CharacterBody2D>(body.GetPath());


		if (body.Name == "player" && !spikeIsActive)
		{
			AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

			animatedSprite2D.Play("Squish");
			GD.Print("Kill me please!");
			chargeMode = true;
			player.Call("PlayerJumpOnEnemy");
			spikeIsActive = true;
			Spikes.Visible = true;
			Spikes.CollisionLayer = 1;
			Speed = 0;
			timesPlayerHaveHit++;

			if (timesPlayerHaveHit == timeNeddedtoHit)
			{
				this.QueueFree();
			}

			await ToSignal(GetTree().CreateTimer(1.5f), "timeout");
			GD.Print(player.Position);
			GD.Print(this.Position);
			GD.Print(direction);

			if (player.Position.X > this.Position.X)
			{
				GD.Print("right");
				if(direction.X > 0)
				{
					//do nothing
				}

				else if(direction.X < 0)
				{
					direction.X *= -1;
				}
			}

			else if(player.Position.X < this.Position.X)
			{
				GD.Print("left");

				if (direction.X > 0)
				{
					direction.X *= -1;
				}

				else if (direction.X < 0)
				{
					//do nothing
				}
			}

			//animatedSprite2D.Play("Walking");

			Speed = 200;

		}
	}

		private void _on_hitbox_death_body_entered(CharacterBody2D body)
	{
		GD.Print("Body: " + body.Name + "has entered");
		GD.Print("_on_hitbox_death_body_entered");


		//Call function from player
		if (body.Name == "player" && spikeIsActive)
		{

			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerDie");
		}
	}
	
	private void _on_hitboxfrontandback_body_entered(CharacterBody2D body)
	{
		GD.Print("Body: " + body.Name + "has entered");

		GD.Print("_on_hitboxfrontandback_body_entered");

		//Call function from player
		if (body.Name == "player")
		{
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerDie");
		}
	}

	private void _on_animated_sprite_2d_animation_finished()
	{
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (animatedSprite2D.Animation == "Squish")
		{
			animatedSprite2D.Play("AngryWalking");
		}
		else if (chargeMode == false)
		{
			animatedSprite2D.Play("Walking");
		}
		// Replace with function body.
	}

}





