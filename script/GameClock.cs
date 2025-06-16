using Godot;
using System;

public partial class GameClock : Node2D
{
	static RichTextLabel clock_display;
    static Sprite2D clock;
    public override void _Ready()
	{
        clock_display = GetNode<RichTextLabel>("time");
        clock = GetNode<Sprite2D>("clock");
    }

	public override void _Process(double delta)
	{
        if (!GameProcessHandle.clock_on) clock_display.Text = "";
        else if (!GameProcessHandle.clock_glitch) clock_display.Text = $"[center]{GameProcessHandle.hour:00}:{GameProcessHandle.minute:00}:{GameProcessHandle.second:00}[/center]";
        else clock_display.Text = $"{GD.RandRange(1, 99).ToString("D2")}:{GD.RandRange(1, 99).ToString("D2")}:{GD.RandRange(1, 99).ToString("D2")}";
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (GameProcessHandle.eyes_closed_progr != 0) return;
            if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                Vector2 mousePos = GetGlobalMousePosition();
                var frameTexture = clock.Texture;
                Vector2 clockSize = frameTexture.GetSize() * clock.Scale;
                Rect2 clockRect = new Rect2(clock.GlobalPosition - clockSize / 2, clockSize);
                if (clockRect.HasPoint(mousePos))
                {
                    SoundHandle.Instance.PlaySFXPanned("res://sfx/button.ogg", 3);
                    GameProcessHandle.clock_on = !GameProcessHandle.clock_on;
                    ToggleWeirdClock(GameProcessHandle.clock_on);
                }
            }
        }
    }
    public static void ToggleWeirdClock(bool on)
    {
        
    }

}
