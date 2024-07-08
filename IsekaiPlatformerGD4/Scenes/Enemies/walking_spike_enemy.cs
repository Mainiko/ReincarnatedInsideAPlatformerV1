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

			var found_ledge = LedgeCheckRight.IsColliding() && ledgeCheckLeft.IsColliding();
			if (IsOnWall() || !found_ledge)
			{
				direction *= -1;
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
		GD.Print("Body: " + body.Name + "has entered" + "ost");
		//Call function from player
		if (body.Name == "player")
		{
			playSound("crunch");
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
		GD.Print("Should be dead");
		if (hasDied)
		{
			GD.Print("Why not dead?");
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
	private void playSound(string sound)
	{

		AudioStreamPlayer audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		string path = "";

		switch (sound)
		{
			case ("baby"):
				path = "res://Assets/Sounds/FX/ESM_War_UI_Source_Old_Match_Strike_Light_1_Fire_Foley.wav";
				break;
			case ("projectile"):
				path = "res://Assets/Sounds/FX/FF_CF_foley_plip_yellow.wav";
				break;
			case ("squish"):
				path = "res://Assets/Sounds/FX/EX_West_Mine_Wet_Gravel_Hit_Clip.wav";
				break;
			case ("crunch"):
				path = "res://Assets/Sounds/FX/OLIVER_fx_foley_lapsung_ice_crunch.wav";
				break;
			default:
				break;
		}

		audioStreamPlayer.Stream = (AudioStream)ResourceLoader.Load(path);
		audioStreamPlayer.Play();
	}

}
