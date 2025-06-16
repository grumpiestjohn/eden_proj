using Godot;
using System;
using System.Reflection;

public partial class SceneHandle : Node
{
    public string curr_scene = "";
    public static SceneHandle Instance { get; private set; }
    public override void _Ready()
    {
        if (Instance == null) Instance = this;
        Input.MouseMode = Input.MouseModeEnum.Hidden;
        curr_scene = GetTree().CurrentScene.Name;
    }
    public async void ChangeScene(string scenename, bool transition = true, double duration = 1)
    {
        if (GlobalOverlay.Instance.TweenWorking == true)
        {
            GD.Print("Transition is in progress, no scene change");
            return;
        }
        if (transition)
        {
            GlobalOverlay.Instance.FadeInOut(true, duration);
            await ToSignal(GlobalOverlay.Instance, "TweenFinished");
        }
        GetTree().ChangeSceneToFile($"res://tscn/{scenename}.tscn");
        if (GameProcessHandle.night_ongoing) GameProcessHandle.BlockerAttempt();
        curr_scene = scenename;
        if (transition)
        {
            GlobalOverlay.Instance.FadeInOut(false, duration);
            await ToSignal(GlobalOverlay.Instance, "TweenFinished");
        }
    }

    public void UpdateLocalizedSprites()
    {
        foreach (Node node in GetTree().GetNodesInGroup("localized_sprites"))
        {
            string path = $"res://sprite/{TranslationServer.GetLocale()}/{node.Name}.png";

            if (!ResourceLoader.Exists(path))
            {
                GD.PrintErr($"Missing localization sprite at: {path}");
                continue;
            }

            Texture2D texture = ResourceLoader.Load<Texture2D>(path);

            if (node is TextureButton textureButton)
            {
                textureButton.TextureNormal = texture;

            }
            else if (node is Sprite2D sprite)
            {
                sprite.Texture = texture;
            }
            else
            {
                GD.PrintErr($"Invalid node for localization: {node.GetType()}");
            }
        }
    }
}