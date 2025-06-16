using Godot;
using System;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;

public partial class GameProcessHandle : Node
{
    public static int custom_night_just_done = 0;

    public static bool killable = false;
    public static bool night_ongoing = false;
    public static int night_timeout_mode = 0;
    public static string cause_of_death = "Unknown";

    public static bool scene_changing = false;
    public static string dest = "return";

    public static Sprite2D eyes = null;
    public static bool spacebar_pressed = false;
    public static bool can_be_closed = false;
    public static float eyesTimer = 0;

    public static int? hour = null;
    public static int? minute = null;
    public static int? second = null;
    public static float sec_timer = 0;
    public static int night_timer = 0;
    public static int cd_timer = 0;


    public static int eyes_closed_progr = 0;
    public static int sanity = 100;
    public static int candle_pos = 2;

    public static bool door_open = false;
    public static bool clock_on = true;
    public static bool clock_glitch = false;

    public static int curr_night = 1;
    //DIFFICULTY
    public static readonly int[,] nightDifficulties = new int[,]
    {
        //0quick 1senseless 2arch 3climb 4grab 5mono 6fort 7block 8hands
        { 3,  3,  0,  0,  0,  0,  0,  0,  0 }, //NIGHT 1
        { 4,  4,  3,  0,  0,  0,  0,  0,  0 }, //NIGHT 2
        { 1,  0,  1,  0,  0,  0,  0,  0,  0 }, //NIGHT 3
        { 1,  0,  1,  1,  0,  0,  0,  0,  0 }, //NIGHT 4
        { 1,  0,  1,  1,  1,  0,  0,  0,  0 }, //NIGHT 5
        { 1,  1,  1,  1,  1,  0,  0,  0,  0 }, //NIGHT 6
        {15, 15, 15, 15, 15, 15, 15,  15,  15 } //NIGHT 7
    };
    //[CANDLE ENTITIES]

    //QUICKSILVER
    public static int entity_quicksilver_diff;
    public static AnimatedSprite2D entity_quicksilver_sprite;
    public static int entity_quicksilver_pos;
    public static bool entity_quicksilver_active = false;
    public static bool entity_quicksilver_on_cd = false;

    //SENSELESS
    public static int entity_senseless_diff;
    public static Sprite2D entity_senseless_sprite;
    public static int entity_senseless_pos;
    public static bool entity_senseless_active = false;
    public static bool entity_senseless_on_cd = false;

    //[DOOR ENTITIES]

    //ARCHANGEL
    public static int entity_archangel_diff;
    public static int entity_archangel_phase = 0;
    public static bool entity_archangel_active = false;
    public static bool entity_archangel_on_cd = false;

    //[CEILING ENTITIES]

    //CLIMBER
    public static int entity_climber_diff;
    public static int entity_climber_phase = 0;
    public static bool entity_climber_active = false;
    public static bool entity_climber_on_cd = false;
    //GRABBER
    public static int entity_grabber_diff;
    public static bool entity_grabber_active = false;
    public static bool entity_grabber_on_cd = false;

    //[CLOCK ENTITIES]
    //HANDS
    public static int entity_hands_diff;
    public static bool entity_hands_active = false;
    public static bool entity_hands_on_cd = false;
    //[OTHER (MIXED)]

    //BLOCKER
    public static int entity_blocker_diff;
    public static bool entity_blocker_active = false;
    public static bool entity_blocker_on_cd = false;
    //MONOLITH
    public static int entity_monolith_diff = 0;
    public static bool entity_monolith_active = false;
    public static bool entity_monolith_on_cd = false;
    //FORTUNE
    public static int entity_fortune_diff = 0;
    public static bool entity_fortune_active = false;
    public static bool entity_fortune_on_cd = false;

    public static GameProcessHandle Instance { get; private set; }
    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        if (night_ongoing)
        {
            SpawnAttempt();
            if (!spacebar_pressed)
            {
                eyesTimer += (float)delta;
                if (eyesTimer >= 0.05f)
                {
                    EyesAdjust(-5);
                    eyesTimer = 0f;
                }
            }
            else
            {
                eyesTimer += (float)delta;
                if (eyesTimer >= 0.05f)
                {
                    EyesAdjust(5);
                    eyesTimer = 0f;
                }
            }
            sec_timer += (float)delta;
            if (sec_timer >= 1 / 60f)
            {
                second++;
                if (second >= 60)
                {
                    second = 0;
                    minute++;
                    night_timer++;
                    if (minute == 60)
                    {
                        minute = 0;
                        hour++;
                        if (hour == 24)
                        {
                            hour = 0;
                        }
                    }
                }
                sec_timer = 0f;
            }
            if (night_timer >= 360 && night_timeout_mode == 0) night_timeout_mode = 1;
        }
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButton && mouseButton.Pressed && mouseButton.ButtonIndex == MouseButton.Left)
        {
            if (!night_ongoing || GlobalOverlay.Instance.TweenWorking) return;
            Vector2 mousePos = mouseButton.Position;
            float windowWidth = GetViewport().GetVisibleRect().Size.X;
            if (mousePos.X <= 40)
            {
                if (SceneHandle.Instance.curr_scene == "game_table")
                {
                    dest = "game_door";
                }
                else if (SceneHandle.Instance.curr_scene == "game_clock")
                {
                    dest = "game_table";
                }
                else return;
            }
            else if (mousePos.X >= windowWidth - 40)
            {
                if (SceneHandle.Instance.curr_scene == "game_door")
                {
                    dest = "game_table";
                }
                else if (SceneHandle.Instance.curr_scene == "game_table")
                {
                    dest = "game_clock";
                }
                else return;
            }
            else return;
            switch (dest)
            {
                case "game_door":
                    SoundHandle.Instance.TweakPanBusses(1);
                    break;
                case "game_table":
                    SoundHandle.Instance.TweakPanBusses(2);
                    break;
                case "game_clock":
                    SoundHandle.Instance.TweakPanBusses(3);
                    break;
            }
            SceneHandle.Instance.ChangeScene(dest, true, 1f);
        }
        if (@event is InputEventKey keyEvent2 && keyEvent2.PhysicalKeycode == Key.Space)
        {
            if (keyEvent2.Pressed != spacebar_pressed) can_be_closed = eyes_closed_progr == 0;
            spacebar_pressed = keyEvent2.Pressed;
        }
    }
    public static void EyesAdjust(int value)
    {
        if (eyes == null)
        {
            eyes = GlobalOverlay.Instance.FindChild("eyes", true, false) as Sprite2D;
        }
        if (!can_be_closed && value > 0) value = -value;
        eyes_closed_progr = Mathf.Clamp(eyes_closed_progr + value, 0, 100);
        if (night_timeout_mode == 1 && eyes_closed_progr == 0)
        {
            night_timeout_mode = 2;
        }
        else if (night_timeout_mode == 2 && eyes_closed_progr == 100)
        {
            night_ongoing = false;
            SoundHandle.Instance.musicinstance.Stop();
            SceneHandle.Instance.ChangeScene("menu", false);
            SoundHandle.NewBgMusic();
        }
        Godot.Color color = eyes.Modulate;
        color.A = eyes_closed_progr / 100f;
        eyes.Modulate = color;
    }
    public static void GetKilled(string name)
    {
        if (!killable) return;
        killable = false;
        night_ongoing = false;
        cause_of_death = name;
        SoundHandle.Instance.musicinstance.Stop();
        SceneHandle.Instance.ChangeScene("menu", false);
        SoundHandle.NewBgMusic();
    }
    public static async void StartNight()
    {
        if (night_ongoing) return;
        hour = DateTime.Now.Hour;
        minute = DateTime.Now.Minute;
        second = DateTime.Now.Second;
        sec_timer = 0f;
        night_timer = 0;
        eyes_closed_progr = 0;
        sanity = 100;
        candle_pos = 2;
        door_open = false;
        clock_on = false;

        entity_quicksilver_active = false;
        entity_quicksilver_pos = 0;
        entity_quicksilver_on_cd = false;

        entity_senseless_active = false;
        entity_senseless_pos = 0;
        entity_senseless_on_cd = false;


        if (curr_night != 8)
        {
            entity_quicksilver_diff = nightDifficulties[curr_night - 1, 0];
            entity_senseless_diff = nightDifficulties[curr_night - 1, 1];
            entity_fortune_diff = nightDifficulties[curr_night - 1, 2];
            entity_monolith_diff = nightDifficulties[curr_night - 1, 3];
            entity_archangel_diff = nightDifficulties[curr_night - 1, 4];
            entity_climber_diff = nightDifficulties[curr_night - 1, 5];
            entity_grabber_diff = nightDifficulties[curr_night - 1, 6];
        }
        else
        {
            //code for inputting custom night settings
        }
        night_timeout_mode = 0;
        await SceneHandle.Instance.ToSignal(SceneHandle.Instance.GetTree().CreateTimer(3f), "timeout");
        
        killable = true;
        night_ongoing = true;
    }
    //ATTEMPTS + SPAWNS
    public static void SpawnAttempt()
    {
        for (int i = 0; i < nightDifficulties.Length / 7; i++)
        {
            float entityDiff = (float)nightDifficulties[curr_night - 1, i];
            if (entityDiff == 0) continue;
            float roll = (float)GD.Randf();
            double chance = -1;
            switch (i)
            {
                case 0: //Quicksilver
                case 1: //Senseless
                    chance = ((2.5d + (entityDiff - 1d) / 19d) * 16d) / 36000d;
                    break;
                case 2: //Archangel
                case 3: //Climber
                    chance = ((0.5d + (entityDiff - 1d) / 19d) * 8.5d) / 36000d;
                    break;
                case 4: //Grabber
                    chance = ((2.5d + (entityDiff - 1d) / 19d) * 16d) / 36000d;
                    break;
                case 5: //Monolith
                    chance = ((2.5d + (entityDiff - 1d) / 19d) * 16d) / 36000d;
                    break;
                case 6: //Fortune
                    chance = ((0.5d + (entityDiff - 1d) / 19d) * 8.5d) / 36000d;
                    break;
                case 8: //Hands
                    chance = ((2.5d + (entityDiff - 1d) / 19d) * 16d) / 36000d;
                    break;
                default:
                    chance = -1;
                    break;
            }
            if (roll < chance)
            {
                GD.Print($"Entity {i} spawned with difficulty {entityDiff}!");
                switch (i)
                {
                    case 0: //Quicksilver
                        SpawnQuicksilver();
                        break;
                    case 1: //Senseless
                        SpawnSenseless();
                        break;
                    case 2: //Archangel
                        SpawnArchangel();
                        break;
                    case 3: //Climber
                        SpawnClimber();
                        break;
                    case 4: //Grabber
                        SpawnGrabber();
                        break;
                    case 5: //Monolith
                        SpawnMonolith();
                        break;
                    case 6: //Fortune
                        SpawnFortune();
                        break;
                    case 8: //Hands
                        SpawnHands();
                        break;
                }
            }
        }
    }
    public static void BlockerAttempt()
    {
        if (!entity_blocker_active && !entity_blocker_on_cd && entity_blocker_diff != 0)
        {
            float roll = (float)GD.Randf();
            double chance = 0.05f + (0.15f / 19f * (entity_blocker_diff - 1));
            if (roll < chance)
            {
                GD.Print("Blocker entity spawned!");
                entity_blocker_active = true;
            }
        }
    }
    public static async void SpawnQuicksilver()
    {
        if (entity_quicksilver_active) return;
        if (entity_quicksilver_on_cd) return;
        entity_quicksilver_active = true;
        entity_quicksilver_on_cd = false;
        entity_quicksilver_pos = (int)Math.Round((double)GD.RandRange(1, 3));
        while (entity_quicksilver_pos == entity_senseless_pos)
        {
            entity_quicksilver_pos = (int)Math.Round((double)GD.RandRange(1, 3));
        }
        GD.Print($"Quicksilver spawned at position {entity_quicksilver_pos} with difficulty {entity_quicksilver_diff}!");
        if (SceneHandle.Instance.curr_scene == "game_table")
        {
            entity_quicksilver_sprite = GameTable.quicksilversprite;
            if (entity_quicksilver_sprite != null)
            {
                switch (entity_quicksilver_pos)
                {
                    case 1:
                        if (GameTable.left != null)
                        {
                            Vector2 pos = entity_quicksilver_sprite.Position;
                            pos.X = GameTable.left.Position.X;
                            entity_quicksilver_sprite.Position = pos;
                        }
                        break;
                    case 2:
                        if (GameTable.center != null)
                        {
                            Vector2 pos = entity_quicksilver_sprite.Position;
                            pos.X = GameTable.center.Position.X;
                            entity_quicksilver_sprite.Position = pos;
                        }
                        else GD.PrintErr("Center position not found for Quicksilver!");
                        break;
                    case 3:
                        if (GameTable.right != null)
                        {
                            Vector2 pos = entity_quicksilver_sprite.Position;
                            pos.X = GameTable.right.Position.X;
                            entity_quicksilver_sprite.Position = pos;
                        }
                        else GD.PrintErr("Right position not found for Quicksilver!");
                        break;
                }
                entity_quicksilver_sprite.Visible = true;
                entity_quicksilver_sprite.Play("spawn");
            }
        }
        SoundHandle.Instance.PlaySFXPanned($"res://sfx/shuffle{Math.Round((double)GD.RandRange(1, 4))}.ogg", 2);
        await SceneHandle.Instance.ToSignal(SceneHandle.Instance.GetTree().CreateTimer(5f), "timeout");
        if (SceneHandle.Instance.curr_scene == "game_table")
        {
            entity_quicksilver_sprite = GameTable.quicksilversprite;
            entity_quicksilver_sprite.Play("open");
        }
        float awaittime = 10f - (entity_quicksilver_diff * 5f / 19f);
        await SceneHandle.Instance.ToSignal(SceneHandle.Instance.GetTree().CreateTimer(awaittime - 5f), "timeout");
        if (candle_pos == entity_quicksilver_pos)
        {
            for (int opasity = 100; opasity >= 0; opasity -= 5)
            {
                entity_quicksilver_sprite = null;
                if (SceneHandle.Instance.curr_scene == "game_table")
                {
                    entity_quicksilver_sprite = GameTable.quicksilversprite;
                    Godot.Color color = entity_quicksilver_sprite.Modulate;
                    color.A = opasity / 100f;
                    entity_quicksilver_sprite.Modulate = color;
                }
                await SceneHandle.Instance.ToSignal(SceneHandle.Instance.GetTree().CreateTimer(0.05f), "timeout");
            }
            entity_quicksilver_sprite = null;
            if (SceneHandle.Instance.curr_scene == "game_table")
            {
                entity_quicksilver_sprite = GameTable.quicksilversprite;
                entity_quicksilver_sprite.Visible = false;
                Godot.Color color = entity_quicksilver_sprite.Modulate;
                color.A = 1f;
                entity_quicksilver_sprite.Modulate = color;
            }
            entity_quicksilver_pos = 0;
            OnCooldown("quicksilver");
            entity_quicksilver_active = false;
        }
        else GetKilled("Quicksilver");
    }
    public static async void SpawnSenseless()
    {
        if (entity_senseless_active) return;
        if (entity_senseless_on_cd) return;
        entity_senseless_active = true;
        entity_senseless_on_cd = false;
        entity_senseless_pos = candle_pos;
        while (entity_senseless_pos == entity_quicksilver_pos)
        {
            entity_senseless_pos = (int)Math.Round((double)GD.RandRange(1, 3));
        }
        GD.Print($"Senseless spawned at position {entity_senseless_pos} with difficulty {entity_senseless_diff}!");
        if (SceneHandle.Instance.curr_scene == "game_table")
        {
            entity_senseless_sprite = GameTable.senselesssprite;
            if (entity_senseless_sprite != null)
            {
                switch (entity_senseless_pos)
                {
                    case 1:
                        if (GameTable.left != null)
                        {
                            Vector2 pos = entity_senseless_sprite.Position;
                            pos.X = GameTable.left.Position.X;
                            entity_senseless_sprite.Position = pos;
                        }
                        break;
                    case 2:
                        if (GameTable.center != null)
                        {
                            Vector2 pos = entity_senseless_sprite.Position;
                            pos.X = GameTable.center.Position.X;
                            entity_senseless_sprite.Position = pos;
                        }
                        break;
                    case 3:
                        if (GameTable.right != null)
                        {
                            Vector2 pos = entity_senseless_sprite.Position;
                            pos.X = GameTable.right.Position.X;
                            entity_senseless_sprite.Position = pos;
                        }
                        break;
                }
                entity_senseless_sprite.Visible = true;
            }
        }
        SoundHandle.Instance.PlaySFXPanned($"res://sfx/shuffle{Math.Round((double)GD.RandRange(1, 4))}.ogg", 2);
        for (int opasity = 0; opasity <= 100; opasity += 5)
        {
            entity_senseless_sprite = null;
            if (SceneHandle.Instance.curr_scene == "game_table")
            {
                entity_senseless_sprite = GameTable.senselesssprite;
                Godot.Color color = entity_senseless_sprite.Modulate;
                color.A = opasity / 100f;
                entity_senseless_sprite.Modulate = color;
            }
            await SceneHandle.Instance.ToSignal(SceneHandle.Instance.GetTree().CreateTimer(0.05f), "timeout");
        }
        float awaittime = 10f - (entity_senseless_diff * 5f / 19f);
        await SceneHandle.Instance.ToSignal(SceneHandle.Instance.GetTree().CreateTimer(awaittime - 5f), "timeout");
        if (candle_pos != entity_senseless_pos)
        {
            for (int opasity = 100; opasity <= 100; opasity -= 5)
            {
                entity_senseless_sprite = null;
                if (SceneHandle.Instance.curr_scene == "game_table")
                {
                    entity_senseless_sprite = GameTable.senselesssprite;
                    Godot.Color color = entity_senseless_sprite.Modulate;
                    color.A = opasity / 100f;
                    entity_senseless_sprite.Modulate = color;
                }
                await SceneHandle.Instance.ToSignal(SceneHandle.Instance.GetTree().CreateTimer(0.05f), "timeout");
            }
            entity_senseless_sprite = null;
            if (SceneHandle.Instance.curr_scene == "game_table")
            {
                entity_senseless_sprite = GameTable.senselesssprite;
                entity_senseless_sprite.Visible = false;
                Godot.Color color = entity_senseless_sprite.Modulate;
                color.A = 1f;
                entity_senseless_sprite.Modulate = color;
            }
            entity_senseless_pos = 0;
            OnCooldown("senseless");
            entity_senseless_active = false;
        }
        else GetKilled("Senseless");
    }
    public static void SpawnArchangel()
    {
        if (entity_archangel_active) return;
        if (entity_archangel_on_cd) return;
        entity_archangel_phase++;
        OnCooldown("archangel");
        switch (entity_archangel_phase)
        {
            case 1:
                entity_archangel_active = true;
                break;
            case 2:
                break;
            case 3:
                if (GameProcessHandle.door_open == true) {
                    entity_archangel_active = false;
                    entity_archangel_phase = 0;
                }
                else
                {
                    GetKilled("Archangel");
                }
                break;
        }
    }
    public static void SpawnClimber()
    {

    }
    public static void SpawnGrabber()
    {

    }
    public static void SpawnMonolith()
    {

    }
    public static void SpawnFortune()
    {

    }
    public static void SpawnBlocker()
    {

    }
    public static void SpawnHands()
    {
        
    }
    public static async void OnCooldown(string entitytype)
    {
        float timer = 10f;
        switch (entitytype)
        {
            case "quicksilver":
                entity_quicksilver_on_cd = true;
                timer = 15f - (entity_quicksilver_diff - 1) * 8f / 19f;
                break;
            case "senseless":
                entity_senseless_on_cd = true;
                timer = 13f - (entity_senseless_diff - 1) * 8f / 19f;
                break;
            case "archangel":
                entity_archangel_on_cd = true;
                timer = 20f - (entity_archangel_diff - 1) * 10f / 19f;
                break;
            case "climber":
                entity_climber_on_cd = true;
                timer = 20f - (entity_climber_diff - 1) * 10f / 19f;
                break;
            case "grabber":
                entity_grabber_on_cd = true;
                timer = 30f - (entity_grabber_diff - 1) * 15f / 19f;
                break;
            case "monolith":
                entity_monolith_on_cd = true;
                timer = 20f - (entity_monolith_diff - 1) * 10f / 19f;
                break;
            case "fortune":
                entity_fortune_on_cd = true;
                timer = 35f - (entity_fortune_diff - 1) * 15f / 19f;
                break;
            case "blocker":
                entity_blocker_on_cd = true;
                timer = 10f - (entity_blocker_diff - 1) * 5f / 19f;
                break;
            case "hands":
                entity_hands_on_cd = true;
                timer = 25f - (entity_hands_diff - 1) * 10f / 19f;
                break;
        }
        await SceneHandle.Instance.ToSignal(SceneHandle.Instance.GetTree().CreateTimer(timer), "timeout");
        switch (entitytype)
        {
            case "quicksilver":
                entity_quicksilver_on_cd = false;
                break;
            case "senseless":
                entity_senseless_on_cd = false;
                break;
            case "archangel":
                entity_archangel_on_cd = false;
                break;
            case "climber":
                entity_climber_on_cd = false;
                break;
            case "grabber":
                entity_grabber_on_cd = false;
                break;
            case "monolith":
                entity_monolith_on_cd = false;
                break;
            case "fortune":
                entity_fortune_on_cd = false;
                break;
            case "blocker":
                entity_blocker_on_cd = false;
                break;
            case "hands":
                entity_hands_on_cd = false;
                break;
        }
    }
}

