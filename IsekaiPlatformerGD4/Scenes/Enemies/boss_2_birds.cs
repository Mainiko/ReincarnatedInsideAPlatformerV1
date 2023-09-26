using Godot;
public partial class boss_2_birds : Node2D
{
	AnimatedSprite2D animatedSprite2D = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _on_area_hitbox_death_body_entered(Node2D body)
	{
		GD.Print("Body: " + body.Name + "has entered");

		//Call function from player
		if (body.Name == "player")
		{
			GD.Print("Kill me please!");
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerJumpOnEnemy");
			//this.QueueFree();

			Node2D Bird = GetNode<Node2D>("Bird");
			Bird.Visible = false;
			var Hitbox_playerHurt = GetNode<Area2D>("AreaDeathPlayer");
			var Hitbox_Hurt = GetNode<Area2D>("AreaHitboxDeath");
			Hitbox_playerHurt.CollisionLayer = 0;
			Hitbox_playerHurt.CollisionMask = 0;
			Hitbox_Hurt.CollisionLayer = 0;
			Hitbox_Hurt.CollisionMask = 0;

			GD.Print(Bird.Visible);
			GD.Print("ost");

		}
	}

	private void _on_area_death_player_body_entered(Node2D body)
	{
		GD.Print("Body: " + body.Name + "has entered");

		//Call function from player
		if (body.Name == "player")
		{
			var player = GetNode<CharacterBody2D>(body.GetPath());
			player.Call("PlayerDie");
		}
	}


	private void EnterLevel()
	{

		Node2D Bird = GetNode<Node2D>("Bird");
		Bird.Visible = true;
		var Hitbox_playerHurt = GetNode<Area2D>("AreaDeathPlayer");
		var Hitbox_Hurt = GetNode<Area2D>("AreaHitboxDeath");
		Hitbox_playerHurt.CollisionLayer = 1;
		Hitbox_playerHurt.CollisionMask = 1;
		Hitbox_Hurt.CollisionLayer = 1;
		Hitbox_Hurt.CollisionMask = 1;
	}
}



