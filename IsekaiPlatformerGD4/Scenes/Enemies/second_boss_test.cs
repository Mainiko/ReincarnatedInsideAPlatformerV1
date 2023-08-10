using Godot;
using System;

public partial class second_boss_test : CharacterBody2D
{

	[Export] private int fallSpeed = 300;
	[Export] private int goingUppSpeed = -50;
	[Export] private float timeBeforeFall = 0.5f;
	[Export] private float timeBeforeGoingUpp = 0.5f;
	[Export] private float timeBeforeFirstFall = 0;
	[Export] private int speed = 100;



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
	int timesHit = 0;
	// Get the gravity from the project settings to be synced with RigidBody nodes.

	public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		GD.Print("my position " + this.Position);

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

		if((t > 0.4 && t < 0.42 && !moveLeft || t > 0.6 && t < 0.62 && moveLeft) && stomping == false )
		{
			goingToStomp = true;
		}

		if (goingToStomp)
		{
			count += 1;

			if (count == 4 || count == 6 || count == 14 || count == 15 || count == 21 )
			{
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
			if (count == 22)
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
			var b = new Vector2(200, 150);
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
			var b = new Vector2(200, 150);
			var c = new Vector2(70, 70);
			if (t < 1)
			{
				t += (float)(movespeed * delta);
				this.Position = QuadraticBezier(a, b, c, t);
			}
		}

		GD.Print("my velociy: " + this.Velocity.Y);
		GD.Print("t: " + t);
		GD.Print("count: " + count);
		GD.Print("RotationDegrees: " + this.RotationDegrees);





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
			moving = false;
			this.Rotate((float)0.1);
			if (this.RotationDegrees > 0 && this.RotationDegrees < 1)
			{
				this.RotationDegrees = 0;
				rotate = false;
				moving = true;
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
		if (body.Name == "player")
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
}


