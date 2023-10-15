using Godot;
using System;
using System.Diagnostics.Tracing;
using System.Text.RegularExpressions;

public partial class player : CharacterBody2D
{

	[Signal] public delegate void OnPlayerDiedEventHandler();

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	AnimatedSprite2D animatedSprite2D = null;
	Timer coyoteTimer = null;


	private enum State { NORMAL, DASHING, INPUT_DISABLED }


	// TODO TRIM DOWN UNUSED VARIABLES
	[Export]private float GroundSpeed = 125.0f;
	[Export]private int airTurnSpeed = 30;
	[Export] private int airTurnFinaleSpeed = 85;
	[Export]private int friction = 23;
	[Export]private int maxHorizontalSpeed = 124;
	[Export]private float JumpVelocity = -250.0f;
	[Export]private int JUMP_RELESE_FORCE = -100;
	[Export]private int addiditionalFallGravity = 1500;
	[Export]private float acceleration = 0.25f;
	[Export]private int wallGravity = 900;
	[Export]private bool hasDoubleJump = false;
	[Export]private bool CanGaineDoubleJump = false;
	[Export]private bool hasDash = false;
	[Export]private bool isDashing = false;
	[Export] private bool wallClimbJump = false;
	private State currentState = State.NORMAL;
	private bool isStateNew = true;
	private bool isDying = false;
	private bool isJumpingOnEnemy = false;
	private bool isJumpingOnJumpPlatform = false;
	private bool hasJumped = false;

	private float lastJumpDirection = 0;

	private float myPosition = 0;
	private float myLastPosition = 0;

	private float Speed = 125.0f;

	private double wallJumpTimer = 0.3f;
	private double wallJumpTimerReset = 0.3f;
	public bool isWallJumping = false;
	public bool hasReachedMaxSpeed { get; set; } = false ;
	

	[Export] private int wallJumpSpeed = 600;
	[Export] private int wallJumpTurnSpeed = 5;
	[Export] private float wallJumpTurnAcceleration = 0.08f;
	[Export] private float wallJumpAcceleration = 0.2f;
	[Export] private float wallJumpVelocity = -250.0f;

	[Export] private float airSpeed = 75.0f;


	//Celeste tutorial
	[Export]private int jumpSpeed = 200;
	[Export]private int dashSpeed = 1500;

	[Export] private int defaultHazardMask = 0;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		coyoteTimer = GetNode<Timer>("CoyoteTimer");
	}

	public override  void _PhysicsProcess(double delta)
	{

		Vector2 velocity = Velocity;
		Vector2 direction = GetDirectionVector();
		bool wasOnFloor = IsOnFloor();

		// Handle the animation
		HandleAnimations(velocity, direction);

		if (IsOnFloor())
		{
			Speed = GroundSpeed;
			lastJumpDirection = 0;
			hasDoubleJump = CanGaineDoubleJump;
		}

		HandleGravity(ref velocity, delta);

		HandleJump(ref velocity);
		
		HandleWallJump(ref velocity,ref direction, delta);

		if (isWallJumping)
		{
			wallJumpTimer -= delta;
			if (wallJumpTimer <= 0)
			{
				isWallJumping = false;
				wallJumpTimer = wallJumpTimerReset;
				GD.Print("walljumptimer = false");
			}
		}

		HandleMovement(ref velocity, ref direction, delta);


		Velocity = velocity;
		
		MoveAndSlide();

		if (wasOnFloor && !IsOnFloor())
		{
			coyoteTimer.Start();
		}

		if (IsOnFloor())
		{
			hasDoubleJump = true;
		}
	}


	public Vector2 GetDirectionVector() //Get player input as vector
	{
		Vector2 direction = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
		return direction;
	}

	private void FlipSprite(Vector2 direction) //Flip sprite so it faces input direction
	{
		animatedSprite2D.FlipH = direction.X < 0;
	}

	private void PlayerDie()
	{
		GetTree().ReloadCurrentScene();
		QueueFree();
		GD.Print("I'm dead");

	}

	private void PlayerJumpOnEnemy()
	{
		GD.Print("Jump");
		isJumpingOnEnemy = true;

	}

	private void PlayerJumpOnJumpPlatform()
	{
		GD.Print("Jump");
		isJumpingOnJumpPlatform = true;
	}


	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Escape)
			{
				GetTree().ChangeSceneToFile("res://Scenes/UI/menu.tscn");
			}
		}
	}

	private void HandleMovement(ref Vector2 velocity, ref Vector2 direction, double delta) //Continue imorgon and check why  movement works forward movement after a walljump
	{
		if (direction != Vector2.Zero)
		{
			FlipSprite(direction);
			if (!IsOnFloor())
			{
				if (isWallJumping && lastJumpDirection != direction.X) //When applying direction to the same direction as the walljump
				{
					GD.Print("Inside 1");

					GD.Print("velocity.X === " + velocity.X);
					if (velocity.X > 199 || velocity.X < -199) //When reaching max speed
					{
						hasReachedMaxSpeed = true;
					}

					if(hasReachedMaxSpeed)
					{
						velocity.X = Mathf.Lerp(velocity.X, 0, acceleration); //Start slowing down to 0, to not be able to move in the air
					}
					else
					{
						velocity.X = Mathf.Lerp(velocity.X, direction.X * jumpSpeed, acceleration); //Continue adding speed until maxspeed is reached
					}

				}
				else if (isWallJumping) //When turning middair
				{
					GD.Print("Inside 2 TURN");

					velocity.X = Mathf.Lerp(velocity.X, direction.X * jumpSpeed, acceleration);
				}
				else
				{
					velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, acceleration);
				}
			}
			else
			{
				velocity.X = Mathf.Lerp(velocity.X, direction.X * Speed, acceleration);
			}

		}
		else
		{
			velocity.X = Mathf.Lerp(0, velocity.X, Mathf.Pow(friction, -10 * (float)delta)); //This is friction
		}

		velocity.X = Mathf.Clamp(velocity.X, -maxHorizontalSpeed, maxHorizontalSpeed);

	}

	private void HandleWallJump(ref Vector2 velocity,ref Vector2 direction, double delta)
	{
		//FIX LOGIC SO THAT lastJumpDirection gives logical value not opposite as it is now
		if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastLeft").IsColliding() && !IsOnFloor()) //Jump from left wall
		{
			velocity.Y = wallJumpVelocity;

			isWallJumping = true;
			hasReachedMaxSpeed = false;
			lastJumpDirection = -1;
		}
		else if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastRight").IsColliding() && !IsOnFloor()) //Jump from right wall
		{
			velocity.Y = wallJumpVelocity;

			isWallJumping = true;
			hasReachedMaxSpeed = false;
			lastJumpDirection = 1;
		}
		//Do we want to do anything special here?
  //      else if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastLeft").IsColliding() && wallClimbJump == true)
		//{
		//}
		//else if (Input.IsActionJustPressed("jump") && GetNode<RayCast2D>("RayCastRight").IsColliding() && wallClimbJump == true)
		//{
  //      }

		if (isWallJumping) //Check if we are walljumping and then continually apply horizontal walljump acceleration
		{
			if (lastJumpDirection < 0)
			{
				velocity.X = Mathf.Lerp(velocity.X, wallJumpSpeed, wallJumpAcceleration);
			}
			if(lastJumpDirection > 0)
			{
				velocity.X = Mathf.Lerp(velocity.X, -wallJumpSpeed, wallJumpAcceleration);
			}
		}

	}

	private void HandleGravity(ref Vector2 velocity,double delta)
	{
		if (!IsOnFloor())
		{
			if (GetNode<RayCast2D>("RayCastLeft").IsColliding() && Input.IsActionPressed("move_left") && velocity.Y > 0)
			{
				velocity.Y += (gravity - wallGravity) * (float)delta;
				animatedSprite2D.Play("Wallclimb");

			}
			else if (GetNode<RayCast2D>("RayCastRight").IsColliding() && Input.IsActionPressed("move_right") && velocity.Y > 0)
			{
				velocity.Y += (gravity - wallGravity) * (float)delta;
				animatedSprite2D.Play("Wallclimb");
			}
			else if (Velocity.Y > 0) //If player is falling 
			{
				velocity.Y += addiditionalFallGravity * (float)delta; //Apply fast fall
			}
			else
			{
				velocity.Y += gravity * (float)delta;
			}
		}
	}

	private void HandleJump(ref Vector2 velocity)
	{
		if (Input.IsActionJustPressed("jump") && (IsOnFloor() || !coyoteTimer.IsStopped() || hasDoubleJump))
		{
			velocity.Y = JumpVelocity;

			if (!IsOnFloor() && coyoteTimer.IsStopped())
			{
				hasDoubleJump = false;
			}
		}

		if (isJumpingOnEnemy)
		{
			velocity.Y = (float)(JumpVelocity * 1.035);

			isJumpingOnEnemy = false;
		}


		if (isJumpingOnJumpPlatform)
		{
			velocity.Y = (float)(JumpVelocity * 1.5);
			isJumpingOnJumpPlatform = false;
		}


		if (Input.IsActionJustReleased("jump") && (velocity.Y < JUMP_RELESE_FORCE))
		{
			velocity.Y = JUMP_RELESE_FORCE;
		}
	}

	private void HandleAnimations(Vector2 velocity, Vector2 direction)
	{
		if (!IsOnFloor())
		{
			if (velocity.Y < 0)
			{
				hasJumped = true;
				animatedSprite2D.Play("JumpUp");
			}
			else if (velocity.Y > 0)
			{
				animatedSprite2D.Play("JumpDown");
			}
		}
		else if (animatedSprite2D.Animation == "JumpDown" && IsOnFloor())
		{
			animatedSprite2D.Play("JumpImpact");
		}
		else if (direction == Vector2.Zero && (animatedSprite2D.IsPlaying() && animatedSprite2D.Animation == "JumpImpact") == false)
		{
			animatedSprite2D.Play("Idle");
		}

		else if (IsOnFloor() && direction != Vector2.Zero)
		{
			animatedSprite2D.Play("Run");
		}
	}

	//Methods for other scripts to reach certain variables. Is there a better way to do this?
	public bool HasReachedMaxSpeed()
	{
		return hasReachedMaxSpeed;
	}

	public bool IsWallJumping()
	{
		return isWallJumping;
	}

	public float LastJumpDirection()
	{
		return lastJumpDirection;
	}

	


}
