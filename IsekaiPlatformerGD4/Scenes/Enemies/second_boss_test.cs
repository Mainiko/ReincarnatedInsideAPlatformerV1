using Godot;
using System;
using System.Threading.Tasks;

public partial class second_boss_test : CharacterBody2D
{

	[Export] private int fallSpeed = 300;
	[Export] private int goingUppSpeed = -50;
	[Export] private float timeBeforeFall = 0.5f;
	[Export] private float timeBeforeGoingUpp = 0.5f;
	[Export] private float timeBeforeFirstFall = 0;
	[Export] private int speed = 100;
	[Export] private float shotSpeed = 0.1f;



	bool shot = false;
	bool firstTime = true;
	bool moveLeft = true;
	bool stomping = false;
	bool goingToStomp = false;
	bool goingUpp = false;
	bool wait = false;
	bool moving = true;
	bool rotate = false;
	float startingPosition = 0;
	float t = 0;
	double movespeed = 0.8;
	int count = 0;
	int countShot = 0;
	int timesHit = 0;
	// Get the gravity from the project settings to be synced with RigidBody nodes.

	public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		//GD.Print("my position " + this.Position);

		//int speed = 1; //# Change this to increase it to more units/second

		//160,88


		if (!stomping)
		{
			if (t >= 1)
			{
				if (moveLeft)
				{
					moveLeft = false;
				}
				else
				{
					moveLeft = true;
				}

				goingToStomp = true;
				t = 0;
			}

		}

		if ((t > 0.4 && t < 0.42 && !moveLeft || t > 0.6 && t < 0.62 && moveLeft) && stomping == false)
		{
			goingToStomp = true;
		}

		if (goingToStomp)
		{
			count += 1;

			if (count == 4)
			{
				moving = false;
				goingToStomp = false;
				await ShotLeft();
				moving = true;
			}

			if (count == 6)
			{
				moving = false;
				goingToStomp = false;
				await ShotRight();
				moving = true;

			}

			//if (count == 15)
			//{
			//	GD.Print("ost");
			//	moving = false;
			//	goingToStomp = false;
			//	await ShotLeftToRight();
			//	moving = true;
			//}

			if (count == 14)
			{
				//else if(count == 21)
				//{
				//	moving = false;
				//	goingToStomp = false;

				//}

					GD.Print("is going to stomp: " + goingToStomp);
					startingPosition = this.Position.Y;
					goingToStomp = false;
					stomping = true;
					moving = false;
					velocity.Y = fallSpeed;
			
			}
			
			else
			{
				goingToStomp = false;


			}
			if (count == 23)
			{
				count = 0;
			}


		}

		if (IsOnFloor() && !wait)
		{
			wait = true;
			await ToSignal(GetTree().CreateTimer(timeBeforeGoingUpp), "timeout");
			velocity.Y = goingUppSpeed;
			goingUpp = true;
			wait = false;


		}

		if (this.Position.Y <= startingPosition && goingUpp)
		{

			velocity.Y = 0;
			stomping = false;
			goingUpp = false;
			moving = true;

		}



		if (!moveLeft && moving)
		{
			var a = new Vector2(70, 70);
			var b = new Vector2(200, 180);
			var c = new Vector2(250, 70);
			if (t < 1)
			{
				t += (float)(movespeed * delta);
				this.Position = QuadraticBezier(a, b, c, t);
			}
		}

		if (moveLeft && moving)
		{
			var a = new Vector2(250, 70);
			var b = new Vector2(200, 180);
			var c = new Vector2(70, 70);
			if (t < 1)
			{
				t += (float)(movespeed * delta);
				this.Position = QuadraticBezier(a, b, c, t);
			}
		}

		//GD.Print("my velociy: " + this.Velocity.Y);
		//GD.Print("t: " + t);
		//GD.Print("count: " + count);
		//GD.Print("RotationDegrees: " + this.RotationDegrees);





		//if (!moveLeft && this.Position.X < 160)
		//{
		//	this.Position = Position.MoveToward(new Vector2(250, 100), (float)(delta * speed));
		//}

		//if (!moveLeft && this.Position.X > 160)
		//{
		//	this.Position = Position.MoveToward(new Vector2(250, 60), (float)(delta * speed));
		//}

		//if (moveLeft && this.Position.X > 160)
		//{
		//	this.Position = Position.MoveToward(new Vector2(70, 100), (float)(delta * speed));
		//}

		//if (moveLeft && this.Position.X <= 160)
		//{
		//	this.Position = Position.MoveToward(new Vector2(70, 60), (float)(delta * speed));
		//}

		if (rotate)
		{

			var Hitbox_BossHurt = GetNode<Area2D>("Hitbox_BossHurt");
			var Hitbox_playerHurt = GetNode<Area2D>("Hitbox_playerHurt");
			this.CollisionLayer = 0;
			Hitbox_BossHurt.CollisionLayer = 0;
			Hitbox_playerHurt.CollisionLayer = 0;
			this.CollisionMask = 0;
			Hitbox_BossHurt.CollisionMask = 0;
			Hitbox_playerHurt.CollisionMask = 0;

			moving = false;
			this.Rotate((float)0.1);
			if (this.RotationDegrees > 0 && this.RotationDegrees < 1)
			{
				this.RotationDegrees = 0;
				rotate = false;
				moving = true;
				this.CollisionLayer = 1;
				Hitbox_BossHurt.CollisionLayer = 1;
				Hitbox_playerHurt.CollisionLayer = 1;
				this.CollisionMask = 1;
				Hitbox_BossHurt.CollisionMask = 1;
				Hitbox_playerHurt.CollisionMask = 1;
				CollisionLayer = 0;
				CollisionMask = 1;
			}
		}







		Velocity = velocity;
		MoveAndSlide();
	}

	private void _on_hitbox_boss_hurt_body_entered(Node2D body)
	{
		// Replace with function body.
		GD.Print("Body: " + body.Name + "has entered");

		//Call function from player
		if (body.Name == "player")
		{
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerJumpOnEnemy");
			rotate = true;

			if (timesHit == 5)
			{
				this.QueueFree();
			}
			else
			{
				timesHit += 1;
			}
		}
	}

	private void _on_hitbox_player_hurt_body_entered(Node2D body)
	{
		GD.Print("Body: " + body.Name + "has entered");

		//Call function from player
		if (body.Name == "player" && !rotate)
		{
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerDie");
		}
	}

	private Vector2 QuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
	{
		Vector2 q0 = p0.Lerp(p1, t);
		Vector2 q1 = p1.Lerp(p2, t);

		Vector2 r = q0.Lerp(q1, t);
		return r;
	}


	private async Task<bool> ShotRight()
	{

		while (countShot < 16)
		{ 
			Vector2 direction = new Vector2(-0.5f, 1);
		int degrees = 0;
		switch (countShot)
		{
				case 0:
					direction = new Vector2(-0.4f, 1);
					degrees = 110;
					break;
				case 1:
					direction = new Vector2(-0.3f, 1);
					degrees = 105;
					break;
				case 2:
					direction = new Vector2(-0.2f, 1);
					degrees = 100;
					break;
				case 3:
					direction = new Vector2(-0.1f, 1);
					degrees = 95;
					break;

				case 4:
					direction = new Vector2(0f, 1);
					degrees = 90;
					break;

				case 5:
					direction = new Vector2(0.1f, 1);
					degrees = 85;
					break;
				case 6:
					direction = new Vector2(0.2f, 1);
					degrees = 80;
					break;
				case 7:
					direction = new Vector2(0.3f, 1);
					degrees = 75;
					break;
				case 8:
					direction = new Vector2(0.4f, 1);
					degrees = 70;
					break;
				case 9:
					direction = new Vector2(0.5f, 1);
					degrees = 65;
					break;
				case 10:
					direction = new Vector2(0.6f, 1);
					degrees = 60;
					break;
				case 11:
					direction = new Vector2(0.7f, 1);
					degrees = 55;
					break;
				case 12:
					direction = new Vector2(0.8f, 1);
					degrees = 50;
					break;
				case 13:
					direction = new Vector2(0.9f, 1);
					degrees = 45;
					break;
				case 14:
					direction = new Vector2(1f, 0.9f);
					degrees = 40;
					break;
				case 15:
					direction = new Vector2(1f, 0.8f);
					degrees = 35;
					break;
				case 16:
					direction = new Vector2(1f, 0.7f);
					degrees = 30;
					break;



			}

			var projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/projectile_test.tscn").Instantiate();
		projectile.Call("SetDirection", direction, degrees);
		shot = false;
		await ToSignal(GetTree().CreateTimer(shotSpeed), "timeout");
		AddChild(projectile);
		shot = true;
		countShot++;
		}
		countShot = 0;
		return true;
	}


	private async Task<bool> ShotLeft()
	{

		while (countShot < 16)
		{
			Vector2 direction = new Vector2(0.5f, 1);
			int degrees = 0;
			switch (countShot)
			{
				case 0:
					direction = new Vector2(0.4f, 1);
					degrees = 70 ;
					break;
				case 1:
					direction = new Vector2(0.3f, 1);
					degrees = 75 ;
					break;
				case 2:
					direction = new Vector2(0.2f, 1);
					degrees = 80;
					break;
				case 3:
					direction = new Vector2(0.1f, 1);
					degrees = 85 ;
					break;

				case 4:
					direction = new Vector2(0f, 1);
					degrees = 90 ;
					break;

				case 5:
					direction = new Vector2(-0.1f, 1);
					degrees = 95;
					break;
				case 6:
					direction = new Vector2(-0.2f, 1);
					degrees = 100;
					break;
				case 7:
					direction = new Vector2(-0.3f, 1);
					degrees = 105;
					break;
				case 8:
					direction = new Vector2(-0.4f, 1);
					degrees = 110;
					break;
				case 9:
					direction = new Vector2(-0.5f, 1);
					degrees = 115;
					break;
				case 10:
					direction = new Vector2(-0.6f, 1);
					degrees = 120;
					break;
				case 11:
					direction = new Vector2(-0.7f, 1);
					degrees = 125;
					break;
				case 12:
					direction = new Vector2(-0.8f, 1);
					degrees = 130;
					break;
				case 13:
					direction = new Vector2(-0.9f, 1);
					degrees = 135;
					break;
				case 14:
					direction = new Vector2(-1f, 0.9f);
					degrees = 140;
					break;
				case 15:
					direction = new Vector2(-1f, 0.8f);
					degrees = 145;
					break;
				case 16:
					direction = new Vector2(-1f, 0.7f);
					degrees = 150;
					break;


			}

			var projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/projectile_test.tscn").Instantiate();
			projectile.Call("SetDirection", direction, degrees);
			shot = false;
			await ToSignal(GetTree().CreateTimer(shotSpeed), "timeout");
			AddChild(projectile);
			shot = true;
			countShot++;
		}
		countShot = 0;

		return true;
	}


	private async Task<bool> ShotLeftToRight()
	{

		while (countShot < 19)
		{
			Vector2 direction = new Vector2(0.5f, 1);
			int degrees = 0;
			switch (countShot)
			{
				case 0:
					direction = new Vector2(-1, 1);
					degrees = 70;
					break;
				case 1:
					direction = new Vector2(-0.9f, 1);
					degrees = 75;
					break;
				case 2:
					direction = new Vector2(-0.8f, 1);
					degrees = 80;
					break;
				case 3:
					direction = new Vector2(-0.7f, 1);
					degrees = 85;
					break;

				case 4:
					direction = new Vector2(-0.5f, 1);
					degrees = 90;
					break;

				case 5:
					direction = new Vector2(-0.4f, 1);
					degrees = 95;
					break;
				case 6:
					direction = new Vector2(-0.3f, 1);
					degrees = 100;
					break;
				case 7:
					direction = new Vector2(-0.2f, 1);
					degrees = 105;
					break;
				case 8:
					direction = new Vector2(-0.1f, 1);
					degrees = 110;
					break;
				case 9:
					direction = new Vector2(0, 1);
					degrees = 115;
					break;
				case 10:
					direction = new Vector2(0.1f, 1);
					degrees = 120;
					break;
				case 11:
					direction = new Vector2(0.2f, 1);
					degrees = 125;
					break;
				case 12:
					direction = new Vector2(0.3f, 1);
					degrees = 130;
					break;
				case 13:
					direction = new Vector2(0.4f, 1);
					degrees = 135;
					break;
				case 14:
					direction = new Vector2(0.5f, 1);
					degrees = 140;
					break;
				case 15:
					direction = new Vector2(0.6f, 1);
					degrees = 145;
					break;
				case 16:
					direction = new Vector2(0.7f, 1);
					degrees = 150;
					break;
				case 17:
					direction = new Vector2(0.8f, 1);
					degrees = 150;
					break;
				case 18:
					direction = new Vector2(0.9f, 1);
					degrees = 150;
					break;
				case 19:
					direction = new Vector2(1, 1);
					degrees = 150;
					break;


			}

			var projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/projectile_test.tscn").Instantiate();
			projectile.Call("SetDirection", direction, degrees);
			shot = false;
			await ToSignal(GetTree().CreateTimer(shotSpeed), "timeout");
			AddChild(projectile);
			shot = true;
			countShot++;
		}
		countShot = 0;

		return true;
	}


	private async Task<bool> ShotBig(Vector2 direction, int degrees)
	{
		
		var projectile = ResourceLoader.Load<PackedScene>("res://Scenes/Enemies/boss_2_projectile_1.tscn").Instantiate();
			projectile.Call("SetDirection", direction, degrees);
			shot = false;
			await ToSignal(GetTree().CreateTimer(shotSpeed), "timeout");
			AddChild(projectile);
			shot = true;
		return true;
	}

	private async Task<bool> ShotLeftBig()
	{

		await ShotBig(new Vector2(-1f, 0.3f), -120);
		await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
		await ShotBig(new Vector2(-1f, 1f), -150);
		await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
		await ShotBig(new Vector2(-0f, 1), 180);
		await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
		await ShotBig(new Vector2(-1f, 0.4f), -120);
		await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
		await ShotBig(new Vector2(-0, 1f), 180);
		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		return true;
	}

	private async Task<bool> ShotRightBig()
	{

		await ShotBig(new Vector2(1, 0.3f), 120);
		await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
		await ShotBig(new Vector2(1f, 1f),150);
		await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
		await ShotBig(new Vector2(0f, 1), 180);
		await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
		await ShotBig(new Vector2(1f, 0.4f), 120);
		await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
		await ShotBig(new Vector2(0, 1f), 180);
		await ToSignal(GetTree().CreateTimer(1.0f), "timeout");
		return true;
	}

}


