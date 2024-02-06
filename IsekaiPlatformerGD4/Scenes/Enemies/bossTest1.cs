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
	bool shot = true;
	bool firstime = true;
	bool firsttimeSplit = true;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	Vector2 direction = Vector2.Left;
	




	public async override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		Area2D Spikes = GetNode<Area2D>("HitboxDeath");
		AnimatedSprite2D animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if(firstime)
		{
			Speed = 0;
			firstime = false;
			await ToSignal(GetTree().CreateTimer(1.0), "timeout");
			Speed = 100;
		}

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

					if(GetNode<RayCast2D>("RayCastLeft").IsColliding())
					{
						await shotRight();

					}
					else if (GetNode<RayCast2D>("RayCastRight").IsColliding())
					{
						await shotLeft();

					}

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
















			//if(firsttimeSplit)
			//{
			//	if (timesPlayerHaveHit == 0)
			//	{
				
			//		Node2D instance = (Node2D)ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/boss_state_2.tscn").Instantiate();
			//		Node2D instance2 = (Node2D)ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/boss_state_2.tscn").Instantiate();
			//		instance.Call("turnOfSplit");
			//		instance2.Call("turnOfSplit");
			//		GetTree().Root.AddChild(instance);
			//		GetTree().Root.AddChild(instance2);
			//		var Slimeposition = this.Position;
			//		instance.GlobalPosition = new Vector2(Slimeposition.X + 50, 166);
			//		instance2.GlobalPosition = new Vector2(Slimeposition.X - 50, 166);
			//		firsttimeSplit = false;


			//		this.QueueFree();
			//	}
			//}

			//else
			//{
			//	if (timesPlayerHaveHit == 2)
			//	{

			//		await ToSignal(GetTree().CreateTimer(1.0), "timeout");
			//		Node2D instance = (Node2D)ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/walking_enemy.tscn").Instantiate();
			//		Node2D instance2 = (Node2D)ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/walking_enemy.tscn").Instantiate();
			//		GetTree().Root.AddChild(instance);
			//		GetTree().Root.AddChild(instance2);
			//		var Slimeposition = this.Position;
			//		instance.GlobalPosition = new Vector2(Slimeposition.X + 30, 166);
			//		instance2.GlobalPosition = new Vector2(Slimeposition.X - 30, 166);



			//		this.QueueFree();
			//	}
			//}








			player.Call("PlayerJumpOnEnemy");

			Speed = 0;

			await ToSignal(GetTree().CreateTimer(1.0), "timeout");

			chargeMode = true;
			spikeIsActive = true;
			Spikes.Visible = true;
			Spikes.CollisionLayer = 1;
			Speed = 0;
			timesPlayerHaveHit++;

			if (timesPlayerHaveHit == timeNeddedtoHit)
			{
				GD.Print(body.GetPath());

				var slimeChild1  = GetNode<CharacterBody2D>("/root/SlimeBoss/walking_enemy_slimeBoss_Children2");
				var slimeChild2 = GetNode<CharacterBody2D>("/root/SlimeBoss/walking_enemy_slimeBoss_Children");
				slimeChild1.Call("Spawn");
				slimeChild2.Call("Spawn");
				GD.Print("I should be dead");
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
			this.Free();
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

	private async Task<bool> shotLeft()
	{

	

		if (shot)
		{
			shot = false;

			for (int i = 0; i < 3; i++)
		{
				await ToSignal(GetTree().CreateTimer(0.5), "timeout");
				Vector2 direction = new Vector2(-1, 0);
				var projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/boss_1_projectile.tscn").Instantiate();
				projectile.Call("SetDirection", direction, 180);
				AddChild(projectile);

		}
			shot = true;

		}


		return true;

	}


	private async Task<bool> shotRight()
	{
		if (shot)
		{
			shot = false;

			for (int i = 0; i < 3; i++)
			{
				await ToSignal(GetTree().CreateTimer(0.5), "timeout");
				Vector2 direction = new Vector2(1, 0);
				var projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/boss_1_projectile.tscn").Instantiate();
				projectile.Call("SetDirection", direction, 180);
				AddChild(projectile);

			}
			shot = true;

		}

		return true;

	}


	private void turnOfSplit()
	{
		firsttimeSplit = false;
	}

}





