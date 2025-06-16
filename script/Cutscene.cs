using Godot;
using System;
using System.Text.Json.Serialization;

public partial class Cutscene : Control
{
    private RichTextLabel text;
    private float speed = 20f;
    private bool istyping = false;
    private bool active = false;
    private bool skipped = false;
    private bool finished = false;
    private bool input_just_used = false;
    private int line_num;
    private int curr_line = 1;
    public override async void _Ready()
    {
        text = GetNode<RichTextLabel>("text");
        await ToSignal(GetTree().CreateTimer(2f), "timeout");
        active = true;
        StartTyping();
    }
    public async void StartTyping()
    {
        if (istyping && active) return;
        istyping = true;
        text.VisibleCharacters = 0;
        string line = FetchLine();
        line = Tr(line);
        text.Text = line;
        for (int i = 1; i <= line.Length; i++)
        {
            if (line[i-1] != ' ' && !skipped) SoundHandle.Instance.PlaySFX("res://sfx/scroll.ogg");
            text.VisibleCharacters = i;
            float delay = 1f / speed;
            float elapsed = 0f;
            while (elapsed < delay)
            {
                if (skipped)
                {
                    break;
                }
                await ToSignal(GetTree(), "process_frame");
                elapsed += (float)GetProcessDeltaTime();
            }
        }
        skipped = false;
        istyping = false;
    }

    public string FetchLine()
    {
        line_num = 0;
        string line = "Don't look where you're not allowed.";
        switch (GameProcessHandle.curr_night)
        {
            case 1:
                line_num = 8;
                break;
            case 2:
                line_num = 10;
                break;
            case 3:
                line_num = 6;
                break;
            case 4:
                line_num = 8;
                break;
            case 5:
                line_num = 10;
                break;
            case 6:
                line_num = 5;
                break;
            case 7:
                line_num = 7;
                break;
        }
        if (SaveHandle.saveddata["customcompleted"] > 2)
        {
            switch (SaveHandle.saveddata["customcompleted"])
            {
                case 3:
                    line_num = 2;
                    line = $"NIGHT8_{curr_line}";
                    break;
                case 4:
                    line_num = 13;
                    line = $"NIGHT9_{curr_line}";
                    break;
            }
        }
        else
        {
            line = $"NIGHT{GameProcessHandle.curr_night}_{curr_line}";
        }
        if (Tr(line) == "...")
        {
            speed = 1.5f;
        }
        else
        {
            speed = 20f;
        }
        curr_line++;
        if (curr_line > line_num) 
        {
            GameProcessHandle.custom_night_just_done = 0;
            finished = true;
        }
        return line;
    }
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            if (active && !istyping && finished)
            {
                active = false;
                SceneHandle.Instance.ChangeScene("game_table");
                SoundHandle.FadeMusicOut();
            }
            else if (active && istyping && !skipped)
            {
                skipped = true;
                text.VisibleCharacters = -1;
            }
            else if (active && !istyping && !skipped)
            {
                SoundHandle.Instance.PlaySFX("res://sfx/accept.ogg");
                StartTyping();
            }
        }
    }
}