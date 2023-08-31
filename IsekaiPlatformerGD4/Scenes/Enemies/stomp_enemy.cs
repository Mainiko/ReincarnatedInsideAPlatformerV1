using Godot;
using System;

public partial class stomp_enemy : CharacterBody2D
{
	[Export] private int fallSpeed = 300;
	[Export] private int goingUppSpeed = -50;
	[Export] private float timeBeforeFall = 0.5f;
	[Export] private float timeBeforeGoingUpp = 0.5f;
	[Export] private float timeBeforeFirstFall = 0;

	bool isFalling = false;
	bool firstTime = true;
	float startingPosition = 0;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public async override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		GD.Print("my position " + this.Position.Y);

		if (firstTime)
		{
			firstTime = false;
			await ToSignal(GetTree().CreateTimer(timeBeforeFirstFall), "timeout");
			startingPosition = this.Position.Y;
			isFalling = true;
		}

		if(isFalling)
		{
			velocity.Y = fallSpeed;
		}

		if(IsOnFloor())
		{
			await ToSignal(GetTree().CreateTimer(timeBeforeGoingUpp), "timeout");
			velocity.Y = goingUppSpeed;
			isFalling = false;
		}

		if (this.Position.Y < startingPosition && !isFalling)
		{
			await ToSignal(GetTree().CreateTimer(timeBeforeFall), "timeout");
			velocity.Y = 0;
			isFalling = true;
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void _on_hitbox_body_entered(Node2D body)
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



