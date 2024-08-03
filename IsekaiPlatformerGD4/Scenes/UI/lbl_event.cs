using Godot;
using System;

public partial class lbl_event : Label
{
	protected string eventName;

	private CustomSignals customSignals;


	public override void _Ready()
	{
		//var customSignals = GetNode<Script>("CustomSignals");

		customSignals = GetNode<CustomSignals>("/root/CustomSignals");


		//https://docs.godotengine.org/en/latest/classes/class_object.html#class-object-method-connect
		// // Option 2: Object.connect() with a constructed Callable using a target object and method name.
		//button.Connect("button_down", new Callable(self, nameof(OnButtonDown)));
		customSignals.Connect("EventChangedEventHandler",new Callable(this,nameof( UpdateDisplay)));

		eventName = "signalthingie fixed";



		//customSignals.EventChangedEventHandler.connect(UpdateDisplay)
		//customSignals.Connect("EventChangedEventHandler",this, "UpdateDisplay()")
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	//Continue with getting this to work
	protected virtual void UpdateDisplay()
	{
		this.Text = eventName;
	}
}
