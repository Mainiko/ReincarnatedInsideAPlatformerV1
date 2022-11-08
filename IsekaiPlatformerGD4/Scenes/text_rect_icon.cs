using Godot;
using System;

public partial class text_rect_icon : TextureRect
{

	private Label label;

	private CustomSignals customSignals;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		label = GetNode<Label>("Label");
	}



    public void _on_gui_input(InputEventMouseButton e)
    {

		customSignals = GetNode<CustomSignals>("/root/CustomSignals");

		GD.Print(e);
		string eventValue;
		eventValue = e.ToString();
		label.Text = eventValue;

		//customSignals.EmitSignal(nameof(CustomSignals.EventChangedEventHandler), eventValue);
		customSignals.EmitSignal("EventChangedEventHandler",eventValue);

        if (e.Pressed)
		{
			GD.Print("Clicked");

            if (this.FlipV == true)
            {
                this.FlipV = false;
            }
            else
            {
                this.FlipV = true;

            }

        }

		
    }

 //   public void _on_gui_input(InputEventMouseButton e)
	//{
	//	if(e.Pressed)
	//	{
	//		GD.Print("Clicked");

	//	}
	//}

	//public void _on_gui_input(object e)
	//{
		
	//}

    

}
