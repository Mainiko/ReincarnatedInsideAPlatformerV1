using Godot;
using System;
using System.Threading.Tasks;

public partial class walking_enemy_with_spikes_somtimes : CharacterBody2D
{


	AnimatedSprite2D animatedSprite2D = null;
	[Export] private int timeBeforeDisappear = 100;
	[Export] private int timeBeforeRreturning = 100;
	[Export] private int timeDelay = 100;

	bool spikeIsActive = true;
	int lastTime = 0;
	int timepast = 0;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	Vector2 direction = Vector2.Right;
	public async override void _PhysicsProcess(double delta)
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


		float ost = 25.0f;
		velocity.X = direction.X * ost;
		Velocity = velocity;
		MoveAndSlide();

		Area2D Spikes = GetNode<Area2D>("Spikes");
		if (timepast == timeBeforeDisappear)
		{
			Spikes.Visible = false;
			Spikes.CollisionLayer = 0;
			spikeIsActive = false;
			lastTime = timepast;
		}


		else if (timepast == lastTime + timeBeforeRreturning && lastTime > timeBeforeDisappear)
		{
			Spikes.Visible = false;
			Spikes.CollisionLayer = 0;
			spikeIsActive = false;
			lastTime = timepast;

		}

		else if (!Spikes.Visible)
		{
			await Task.Delay(timeDelay);
			Spikes.Visible = true;
			Spikes.CollisionLayer = 1;
			spikeIsActive = true;
			lastTime = timepast;

		}
		timepast += 1;
		//GD.Print("This is timepast: " + timepast);

		//GD.Print("This is last time: " + lastTime);

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

	private void _on_spikes_body_entered(CharacterBody2D body)
	{
		GD.Print("Body: " + body.Name + "has entered spikes");

		//Call function from player
		if (body.Name == "player" && spikeIsActive)
		{
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerDie");
		}
	}
}
