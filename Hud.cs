using Godot;
using System;

public partial class Hud : CanvasLayer
{
    [Signal]
    public delegate void StartGameEventHandler();

	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        //Dont stop processing when the tree is paused
        ProcessMode = ProcessModeEnum.Always;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if (Input.IsActionJustPressed("pause")) {
            GetTree().Paused = !GetTree().Paused;
            if (GetTree().Paused) {
                ShowMessage("Paused", false);
            } else {
                HideMessage();
            }
        }
    }
    public void ShowMessage(string text, bool removeAutomatically=true) {
        var message = GetNode<Label>("Message");
        message.Text = text;
        message.Show();

        if (removeAutomatically) {
            GetNode<Timer>("MessageTimer").Start();
        }
    }

    public void HideMessage() {
        GetNode<Label>("Message").Hide();
    }

    async public void ShowGameOver() {
        ShowMessage("Game Over");
        GetNode<ColorRect>("Curtain").Show();

        var messageTimer = GetNode<Timer>("MessageTimer");
        await ToSignal(messageTimer, Timer.SignalName.Timeout);

        var message = GetNode<Label>("Message");
        message.Text = "Dodge the Creeps!";
        message.Show();

        await ToSignal(GetTree().CreateTimer(1.0), SceneTreeTimer.SignalName.Timeout);
        GetNode<Button>("StartButton").Show();
    }

    public void UpdateScore(int score) {
        GetNode<Label>("ScoreLabel").Text = score.ToString();
    }

    private void OnStartButtonPressed() {
        GetNode<Button>("StartButton").Hide();
        GetNode<ColorRect>("Curtain").Hide();
        EmitSignal(SignalName.StartGame);
    }

    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnMessageTimerTimeout() {
        HideMessage();
    }
}
