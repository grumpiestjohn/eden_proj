using Godot;
using System;

public partial class GameTable : Node2D
{
    private bool isDragging = false;
    private Vector2 dragOffset;
    public static AnimatedSprite2D candle;
    public static AnimatedSprite2D quicksilversprite;
    public static Sprite2D senselesssprite;
    public static Sprite2D left;
    public static Sprite2D right;
    public static Sprite2D center;
    public override void _Ready()
    {
        candle = GetNode<AnimatedSprite2D>("candle");
        quicksilversprite = GetNode<AnimatedSprite2D>("quicksilver");
        senselesssprite = GetNode<Sprite2D>("senseless");
        left = GetNode<Sprite2D>("left");
        right = GetNode<Sprite2D>("right");
        center = GetNode<Sprite2D>("center");
        CandlePosition();
        if (!GameProcessHandle.night_ongoing)
        {
            GameProcessHandle.StartNight();
            SoundHandle.Instance.PlayMusic("res://sfx/amb.ogg");
        }
        if (GameProcessHandle.entity_quicksilver_active)
        {
            switch (GameProcessHandle.entity_quicksilver_pos)
            {
                case 1:
                    quicksilversprite.Position = new Vector2(left.Position.X, quicksilversprite.Position.Y);
                    break;
                case 2:
                    quicksilversprite.Position = new Vector2(center.Position.X, quicksilversprite.Position.Y);
                    break;
                case 3:
                    quicksilversprite.Position = new Vector2(right.Position.X, quicksilversprite.Position.Y);
                    break;
            }
            if (!quicksilversprite.IsPlaying())
            {
                quicksilversprite.Play("open");
                quicksilversprite.Frame = -1;
            }
            quicksilversprite.Visible = true;
        }
        else
        {
            quicksilversprite.Visible = false;
        }

        if (GameProcessHandle.entity_senseless_active)
        {
            switch (GameProcessHandle.entity_senseless_pos)
            {
                case 1:
                    senselesssprite.Position = new Vector2(left.Position.X, senselesssprite.Position.Y);
                    break;
                case 2:
                    senselesssprite.Position = new Vector2(center.Position.X, senselesssprite.Position.Y);
                    break;
                case 3:
                    senselesssprite.Position = new Vector2(right.Position.X, senselesssprite.Position.Y);
                    break;
            }
            senselesssprite.Visible = true;
        }
        else
        {
            quicksilversprite.Visible = false;
        }
    }

    public void CandlePosition()
    {
        switch (GameProcessHandle.candle_pos)
        {
            case 1:
                candle.Position = GetNode<Sprite2D>("left").Position;
                break;
            case 2:
                candle.Position = GetNode<Sprite2D>("center").Position;
                break;
            case 3:
                candle.Position = GetNode<Sprite2D>("right").Position;
                break;
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton)
        {
            if (GameProcessHandle.eyes_closed_progr != 0) return;
            if (mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                Vector2 mousePos = GetGlobalMousePosition();
                var frameTexture = candle.SpriteFrames.GetFrameTexture(candle.Animation, candle.Frame);
                Vector2 candleSize = frameTexture.GetSize() * candle.Scale;
                Rect2 candleRect = new Rect2(candle.GlobalPosition - candleSize / 2, candleSize);
                if (candleRect.HasPoint(mousePos))
                {
                    isDragging = true;
                    dragOffset = candle.GlobalPosition - mousePos;
                }
            }
            else if (!mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
            {
                isDragging = false;
                if (CandleOverlapping("left")) GameProcessHandle.candle_pos = 1;
                else if (CandleOverlapping("center")) GameProcessHandle.candle_pos = 2;
                else if (CandleOverlapping("right")) GameProcessHandle.candle_pos = 3;
                CandlePosition();
            }
        }
        else if (@event is InputEventMouseMotion && isDragging) { 
            Vector2 mousePos = GetGlobalMousePosition();
            candle.GlobalPosition = mousePos + dragOffset;
        }
    }

    private bool CandleOverlapping(string spriteName)
    {
        var frameTexture = candle.SpriteFrames.GetFrameTexture(candle.Animation, candle.Frame);
        Vector2 candleSize = frameTexture.GetSize() * candle.Scale;
        Rect2 candleRect = new Rect2(candle.GlobalPosition - candleSize / 2, candleSize);

        Sprite2D target = GetNode<Sprite2D>(spriteName);
        Vector2 targetSize = target.Texture.GetSize() * target.Scale;
        Rect2 targetRect = new Rect2(target.GlobalPosition - targetSize / 2, targetSize);

        return candleRect.Intersects(targetRect);
    }
}