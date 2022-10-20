using Godot;
using System;
using System.Collections.Generic;

public class Credits : Control
{
    [Signal]
    public delegate void EndReached();
    
    [Export]
    public string AttributionFilePath {get; set;} = "res://ATTRIBUTION.md";

    [Export]
    public DynamicFont h1Font;
    [Export]
    public DynamicFont h2Font;    
    [Export]
    public DynamicFont h3Font;    
    [Export]
    public DynamicFont h4Font;
    [Export]
    public float CurrentSpeed = 1.0f;

    private bool _isScrolling = true;

    private ScrollContainer _scrollContainer;
    private RichTextLabel _richTextLabel;
    private Timer _scrollResetTimer;

    public override void _Ready()
    {
        _scrollContainer = GetNode<ScrollContainer>("ScrollContainer");
        _richTextLabel = GetNode<RichTextLabel>("ScrollContainer/VBoxContainer/RichTextLabel");
        _scrollResetTimer = GetNode<Timer>("ScrollResetTimer");

        SetFilePath();
        SetHeaderAndFooter();
        SetProcess(false);
    }

    public override void _Process(float delta)
    {
        ScrollTheScrollContainer();
    }

    public void Reset()
    {
        GetNode<ScrollContainer>("ScrollContainer").ScrollVertical = 0;
        SetHeaderAndFooter();
    }

    private string LoadFile(string filePath)
    {
        var file = new File();
        Error err = file.Open(filePath, File.ModeFlags.Read);

        if (err != Error.Ok)
            GD.PrintErr($"Load file failed with error{err}");

        string text = file.GetAsText();
        file.Close();
        return text;
    }

    private void SetFilePath()
    {
        string text = LoadFile(AttributionFilePath);
        
        if (string.IsNullOrEmpty(text))
            return;
        
        text = text.Right(text.Find("\n")); // Trims first line "ATTRIBUTION"
        text = RegexReplaceUrls(text);
        text = RegexReplaceTitles(text);
        GetNode<RichTextLabel>("ScrollContainer/VBoxContainer/RichTextLabel").BbcodeText = $"[center]{text}[/center]";
    }

    private void SetHeaderAndFooter()
    {
    	GetNode<Control>("ScrollContainer/VBoxContainer/HeaderSpace").RectMinSize = new Vector2(RectMinSize.x, RectSize.y);
        GetNode<Control>("ScrollContainer/VBoxContainer/FooterSpace").RectMinSize = new Vector2(RectMinSize.x, RectSize.y);
        GetNode<RichTextLabel>("ScrollContainer/VBoxContainer/RichTextLabel").RectMinSize = new Vector2(RectSize.x, RectMinSize.y);    
    }
    
    private void CheckEndReached(int previousScroll)
    {
        if (previousScroll != GetNode<ScrollContainer>("ScrollContainer").ScrollVertical)
            return;

        SetProcess(false);
        EmitSignal(nameof(EndReached));
    }

    private void ScrollTheScrollContainer()
    {
        if (!_isScrolling || Math.Round(CurrentSpeed) == 0)
            return;
        
        var previousScroll = GetNode<ScrollContainer>("ScrollContainer").ScrollVertical;
        GetNode<ScrollContainer>("ScrollContainer").ScrollVertical += (int)Math.Round(CurrentSpeed);

        CheckEndReached(previousScroll);
    }

    private string RegexReplaceUrls(string credits)
    {
        var regex = new RegEx();
        var matchString = "\\[([^\\]]*)\\]\\(([^\\)]*)\\)";
        var replaceString = "[url=$2]$1[/url]";
        regex.Compile(matchString);
        return regex.Sub(credits, replaceString, true);
    }

    private string RegexReplaceTitles(string credits)
    {
        var iterator = 0;
        var headingFonts = new List<DynamicFont>(){h1Font, h2Font, h3Font, h4Font};
        foreach (var headingFont in headingFonts)
        {
            if (headingFont is DynamicFont)
            {
                iterator += 1;
                var regex = new RegEx();
                var matchString = $"([^#])#{{{iterator}}}\\s([^\n]*)";
                var replaceString = $"$1[font={headingFont.ResourcePath}]$2[/font]";
                regex.Compile(matchString);
                credits = regex.Sub(credits, replaceString, true);
            }
        }

        return credits;
    }

    public void OnScrollContainerScrollEnded()
    {
        GD.Print("Scroll Ended!");
    }

    public void OnRichTextLabelGuiInput(InputEventMouseButton @event)
    {
        if (@event is InputEventMouseButton)
        {
            _isScrolling = false;
            _scrollResetTimer.Start();
        }
    }

    public void OnScrollResetTimerTimeout()
    {
        SetHeaderAndFooter();
        _isScrolling = true;
    }

    public void OnRichTextLabelMetaClicked(string meta)
    {
        if (meta.BeginsWith("https://"))
            OS.ShellOpen(meta);
    }

}
