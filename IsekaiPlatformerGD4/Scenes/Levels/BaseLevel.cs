using Godot;
using System;

public partial class BaseLevel : Node
{
    // Called when the node enters the scene tree for the first time.
    PackedScene playerScene;

    Node2D PlayerRoot = null;
    Node2D BallEnemyRoot = null;

    public override void _Ready()
	{
        //playerScene = GD.Load<PackedScene>("res://Scenes/Characters/Player.tscn");
        //Node instance = playerScene.Instantiate();
        //PlayerRoot = GetNode<Node2D>("PlayerRoot");
        //PlayerRoot.AddChild(instance);


        InstanceScene("res://Scenes/Characters/Player.tscn", "PlayerRoot");
        InstanceScene( "res://Scenes/Enemies/ball_enemy.tscn", "BallEnemyRoot");


    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{

	}

    //Instansiates and adds new scene as child into our scene
    public void InstanceScene( string path, string rootname) 
    {
		//GD.Print(sceneRoot.Name.ToString());

        PackedScene packedScene = GD.Load<PackedScene>(path);

        Node2D sceneRoot;

		Node instance = packedScene.Instantiate();
		sceneRoot = GetNode<Node2D>(rootname);
		sceneRoot.AddChild(instance);

    }

    public void SpawnPlayer()
    {
        InstanceScene( "res://Scenes/Characters/player.tscn", "PlayerRoot");
    }

	public void OnPlayerDiedEvent()
	{
        InstanceScene("res://Scenes/Characters/player.tscn", "PlayerRoot");

    }


    public void ConnectFunc()
    {
        Callable callable = new Callable(this, nameof(OnPlayerDiedEvent));
        this.Connect("OnPlayerDiedEventHandler", callable);
        //this.Connect("OnPlayerDiedEventHandler", this, "OnPlayerDied");
    }



}
