using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

public partial class savehandle : Node
{
    public static Dictionary<string, int> saveddata = new Dictionary<string, int>();
    private const string path = "res://data.dat";
    public override void _Ready()
    {
        FileAccess file = null;
        //CREATING A FILE IF IT'S MISSING
        if (!FileAccess.FileExists(path))
        {
            file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
            file.Close();
        }
        file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        string data = file.GetAsText();
        //FILLING THE FILE WITH DEFAULT VALUES IF EMPTY
        if (data == "")
        {
            file.Close();
            file = FileAccess.Open(path, FileAccess.ModeFlags.WriteRead);
            file.StoreLine("nightcompleted=0");
            file.StoreLine("extrasentity=0");
            file.StoreLine("extrasdocs=0");
            file.StoreLine("musicvolume=50");
            file.StoreLine("sfxvolume=50");
            file.StoreLine("visualeffects=1");
            data = file.GetAsText();
            file.Close();
        }
        //STORING EXISTING DATA IN THE DICTIONARY
        string[] entries = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (string entry in entries)
        {
            string[] keyValue = entry.Split('=');
            if (int.TryParse(keyValue[1], out int value))
            {
                saveddata[keyValue[0]] = value;
            }
            else
            {
                GD.Print("hey wait " + entry + " doesn't fit the criteria. skipping");
            }
        }
        //APPLYING SAVED SETTINGS
        soundhandle.ChangeMusVolume((double)saveddata["musicvolume"] / 100);
        soundhandle.ChangeSFXVolume((double)saveddata["sfxvolume"] / 100);
        file.Close();
    }
    public static void SaveNewData(string valuename, int value)
    {
        saveddata[valuename] = value;
        FileAccess file = FileAccess.Open(path, FileAccess.ModeFlags.Write);
        foreach (KeyValuePair<string, int> line in saveddata)
        {
            file.StoreLine(line.Key + "=" + line.Value);
        }
        file.Close();
    }
}