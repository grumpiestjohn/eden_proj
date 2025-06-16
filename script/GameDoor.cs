using Godot;
using System;
using static Godot.HttpClient;
using static System.Formats.Asn1.AsnWriter;
using System.Diagnostics;

public partial class GameDoor : Node2D
{
    private AnimatedSprite2D door;
    private CollisionObject2D hitbox;
    public override void _Ready()
    {
        door = GetNode<AnimatedSprite2D>("door");
        if (GameProcessHandle.door_open)
        {
            door.Play("open");
            door.Frame = door.SpriteFrames.GetFrameCount(door.Animation) - 1;
        }
        else
        {
            door.Play("close");
            door.Frame = door.SpriteFrames.GetFrameCount(door.Animation) - 1;
        }
    }
	public override void _Process(double delta)
	{
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left && GameProcessHandle.eyes_closed_progr == 0)
            {
                Vector2 mousePos = GetGlobalMousePosition();
                var frameTexture = door.SpriteFrames.GetFrameTexture(door.Animation, door.Frame);
                Vector2 doorSize = frameTexture.GetSize() * door.Scale;
                Rect2 doorRect = new Rect2(door.GlobalPosition - doorSize / 2, doorSize);
                if (doorRect.HasPoint(mousePos))
                {
                    if (door.IsPlaying()) return;
                    if (door.Animation.Equals("open"))
                    {
                        GameProcessHandle.door_open = false;
                        door.Play("close");
                    }
                    else
                    {
                        GameProcessHandle.door_open = true;
                        door.Play("open");
                    }
                }
            }
        }
    }
}