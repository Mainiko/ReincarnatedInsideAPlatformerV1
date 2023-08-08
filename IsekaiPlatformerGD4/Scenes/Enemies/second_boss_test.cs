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



	bool isFalling = false;
	bool firstTime = true;
	float startingPosition = 0;
	// Get the gravity from the project settings to be synced with RigidBody nodes.

	public override async void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		GD.Print("my position " + this.Position);
		var position = new Vector2(0, 60);

		//int speed = 1; //# Change this to increase it to more units/second



		if(this.Position.X <= 100)
		{
			velocity.Y = fallSpeed;
		}

		else
		{
			this.Position = Position.MoveToward(position, (float)(delta * speed));
		}

		//positon = this.Position;
		//positon.Y = 100;
		//this.Position = positon;
		//if (firstTime)
		//{
		//	firstTime = false;
		//	await ToSignal(GetTree().CreateTimer(timeBeforeFirstFall), "timeout");
		//	startingPosition = this.Position.Y;
		//	isFalling = true;
		//}

		//if (isFalling)
		//{
		//	velocity.Y = fallSpeed;
		//}

		//if (IsOnFloor())
		//{
		//	await ToSignal(GetTree().CreateTimer(timeBeforeGoingUpp), "timeout");
		//	velocity.Y = goingUppSpeed;
		//	isFalling = false;
		//}

		//if (this.Position.Y < startingPosition && !isFalling)
		//{
		//	await ToSignal(GetTree().CreateTimer(timeBeforeFall), "timeout");
		//	velocity.Y = 0;
		//	isFalling = true;
		//}

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
			this.QueueFree();
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
}



