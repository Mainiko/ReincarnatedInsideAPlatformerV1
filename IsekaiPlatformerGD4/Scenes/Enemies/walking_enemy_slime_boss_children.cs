using Godot;
using System;
using System.Threading.Tasks;

public partial class walking_enemy_slime_boss_children : CharacterBody2D
{

	AnimatedSprite2D animatedSprite2D = null;
	[Export] private int timeBeforeDisappear = 100;
	[Export] private int timeBeforeRreturning = 100;
	[Export] private int timeDelay = 100;
	[Export] private int timesNeedToKill = 3;
	[Export] private double shotSpeed = 0.5;


	bool isSpawned = false;
	bool hasDied = false;
	int lastTime = 0;
	int timepast = 0;
	int timesPlayerHit = 0;
	bool shot = true;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	Vector2 direction = Vector2.Right;

	public async override void _PhysicsProcess(double delta)
	{

		if (!isSpawned)
		{
			this.Visible = false;
			this.CollisionLayer = 0;
		}

		if (isSpawned)
		{

			Vector2 velocity = Velocity;
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		var LedgeCheckRight = GetNode<RayCast2D>("LedgeCheckRight");
		var ledgeCheckLeft = GetNode<RayCast2D>("LedgeCheckLeft");

		if (!IsOnFloor())
			velocity.Y += gravity * (float)delta;

		bool found_wall = IsOnWall();
		var found_ledge = LedgeCheckRight.IsColliding() && ledgeCheckLeft.IsColliding();
		if (found_wall || !found_ledge)
		{
			GD.Print(direction.X);
			direction *= -1;
			GD.Print(direction.X);
			GD.Print(found_ledge);
			GD.Print(animatedSprite2D);
		}


		if (direction.X > 0)
			animatedSprite2D.FlipH = true;
		if (direction.X < 0)
			animatedSprite2D.FlipH = false;




			if (shot)
			{
				Vector2 direction = new Vector2(-1, 0);
				var player = GetNode<CharacterBody2D>("/root/SlimeBoss/player");
				if (player.Position.X < this.Position.X)
				{
					direction = new Vector2(-1, 0);

				}
				else
				{
					direction = new Vector2(1, 0);

				}


				var projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/boss_1_projectile.tscn").Instantiate();
				projectile.Call("SetDirection", direction, 180);
				shot = false;
				await ToSignal(GetTree().CreateTimer(shotSpeed), "timeout");
				AddChild(projectile);
				shot = true;
			}


			float speed = 25.0f;
		velocity.X = direction.X * speed;
		Velocity = velocity;
		MoveAndSlide();
		CheckIfDead();

		}



	}

	private void _on_hitbox_body_entered(CharacterBody2D body)
	{
		if(isSpawned)
		{
			
				GD.Print("Body: " + body.Name + "has entered");

				//Call function from player
				if (body.Name == "player")
				{
					var player = GetNode<CharacterBody2D>(body.GetPath());
					player.Call("PlayerDie");
				}
			
		}

	}

	private void _on_death_hit_box_body_entered(CharacterBody2D body)
	{
		if (isSpawned)
		{
			
				GD.Print("Body: " + body.Name + "has entered");

			if (body.Name == "player")
			{   //Call function from player

				GD.Print("Kill me please!");
					var player = GetNode<CharacterBody2D>(body.GetPath());
					hasDied = true;
					player.Call("PlayerJumpOnEnemy");
				if (timesNeedToKill == timesPlayerHit)
				{
					animatedSprite2D.Play("Explode");
					//this.QueueFree();
				}
				timesPlayerHit++;
			}
		}
	}

	private void CheckIfDead()
	{
		if (isSpawned)
		{
			if (animatedSprite2D.Animation == "Explode" && animatedSprite2D.IsPlaying() == false)
			{
				this.QueueFree();
			}
		}
	}

	private void Spawn()
	{
		this.Visible = true;
		this.CollisionLayer = 1;
		GD.Print("Spawn");
		isSpawned = true;
	}
}
