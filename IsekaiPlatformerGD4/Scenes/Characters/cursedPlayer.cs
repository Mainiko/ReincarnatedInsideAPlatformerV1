using Godot;
using System;

public partial class cursedPlayer : CharacterBody2D
{
	public const float Speed = 125.0f;
	public const float JumpVelocity = -250.0f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	AnimatedSprite2D animatedSprite2D = null;


    private enum State { NORMAL }

    //private int gravity = 1000;
    private Vector2 velocity = Vector2.Zero;
    private int maxHorizontalSpeed = 200;
    private int horizontalAcceleration = 2000;
    private int jumpSpeed = 300;
    private int jumpTerminationMultiplier = 4;
    private bool hasDoubleJump = false;
    private State currentState = State.NORMAL;
    private bool isStateNew = true;
    private bool isDying = false;

    private int defaultHazardMask = 0;

    //Vector2 direction = Vector2.Zero;

    public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.y += gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor()) 
			velocity.y = JumpVelocity;

        Vector2 direction = GetDirectionVector();

        velocity.x += direction.x * horizontalAcceleration * (float)delta; // Vad gor denna? // kraschar hela grejen // hj
                                                                           // lpte inte heller att parsea horizontalAccelleration till float



		if (direction != Vector2.Zero)
		{
			velocity.x = direction.x * Speed;

			FlipSprite(direction);
		}
		else
		{
			velocity.x = Mathf.MoveToward(Velocity.x, 0, Speed);
        }

		Velocity = velocity;
		MoveAndSlide();
	}


    private Vector2 GetDirectionVector()
    {
        //Vector2 moveVector = Vector2.Zero;
        //moveVector.x = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left");
        //if (Input.IsActionJustPressed("jump"))
        //{
        //    moveVector.y = -1;
        //}
        //else
        //{
        //    moveVector.y = 0;
        //}

        Vector2 direction = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");

        return direction;
    }//Get player input as vector

    private void FlipSprite(Vector2 direction) //Flip sprit so it faces input direction
    {
        if (direction.x < 0)
        {
            animatedSprite2D.FlipH = true;
        }
        else
        {
            animatedSprite2D.FlipH = false;
        }
    }

}
