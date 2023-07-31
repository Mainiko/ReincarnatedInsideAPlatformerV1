using Godot;
using System;
using System.Threading.Tasks;

public partial class jumping_enemy : CharacterBody2D
{
	[Export] private float JumpVelocity = -100.0f;
	[Export] private int TimeDelayBeforeJump = 1000;
	[Export] private int TimeDelayBeforeFall = 1000;
	[Export] private int addiditionalFallGravity = 100;
	private bool isJumping = false;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public async override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
			isJumping = false;
		}
		else
		{
			if(isJumping)
			{
				velocity.Y = JumpVelocity ;
			}
		}

		Velocity = velocity;
		MoveAndSlide();

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
	
	private void _on_timer_timeout()
	{
		isJumping = true;
	}
}


