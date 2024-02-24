using System;
using Godot;

public partial class player : CharacterBody3D
{
    public string slot_text;
    private Label text_control;

    public static int score;
    private Label score_label;

    private CanvasItem win_background;
    private Label score_label_win;

    public static Vector3 player_position;

    private int x = 0;
    private int z = 0;

    private float rotate_value = 0f;
    private bool isPlay = false;
    private bool key_calculate = true;

    public override void _Ready()
    {
        win_background = GetNode<CanvasItem>($"../UI/PanelContainer");
        text_control = GetNode<Label>($"../UI/TC_MarginContainer/TextControl");
        score_label = GetNode<Label>($"../UI/SC_MarginContainer/Score");
        score_label_win = GetNode<Label>($"../UI/PanelContainer/SC_MarginContainer/Score");
    }

    public override void _PhysicsProcess(double delta)
    {
        player_position = GlobalPosition;

        text_control.Text = $"{slot_text}";
        score_label.Text = $"Score: {score}";

        if (IsOnFloor() != true)
        {
            Velocity = Vector3.Down * (float)delta * 1000;
        }

        startSimulation((float)delta);

        if (isPlay && key_calculate)
        {
            calculateControlling();
            isPlay = false;
        }

        /* GD.Print(RotationDegrees); */
        /* GD.Print(rotate_value); */

        MoveAndSlide();
    }

    private void startSimulation(float delta)
    {
        rotate_value %= 360f;

        RotationDegrees = new Vector3(RotationDegrees.X, rotate_value, RotationDegrees.Z);

        GlobalPosition = new Vector3(
            GlobalPosition.X,
            GlobalPosition.Y,
            Mathf.Lerp(GlobalPosition.Z, z, delta * 10)
        );

        GlobalPosition = new Vector3(
            Mathf.Lerp(GlobalPosition.X, x, delta * 10),
            GlobalPosition.Y,
            GlobalPosition.Z
        );
    }

    private async void calculateControlling()
    {
        key_calculate = false;
        string[] words_control = slot_text.Split(' ');
        foreach (string control in words_control)
        {
            switch (control)
            {
                case "L":
                    rotate_value += 90f;
                    break;
                case "R":
                    rotate_value -= 90f;
                    break;
                case "F":
                    if (Mathf.Abs(rotate_value) == 0)
                    {
                        x++;
                    }
                    else if (Mathf.Abs(rotate_value) == 180)
                    {
                        x--;
                    }
                    else if (rotate_value == 90 || rotate_value == -270)
                    {
                        z--;
                    }
                    else if (rotate_value == 270 || rotate_value == -90)
                    {
                        z++;
                    }
                    break;
            }
            await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
        }

        if (
            Mathf.Round(GlobalPosition.X) == goal.goal_position.X
            && Math.Round(GlobalPosition.Z) == goal.goal_position.Z
        )
        {
            win_background.Visible = true;
            score_label_win.Text = $"Score: {score}";
            GD.Print("You win");
        }
        else
        {
            GD.Print("You lost");
        }

        key_calculate = true;
    }

    private void _on_rotate_left_button_pressed()
    {
        slot_text += "L ";
    }

    private void _on_forward_button_pressed()
    {
        slot_text += "F ";
    }

    private void _on_rotate_right_button_pressed()
    {
        slot_text += "R ";
    }

    private void _on_play_pressed()
    {
        isPlay = true;
    }

    private void _on_gear_area_entered(Area3D area)
    {
        GD.Print(area);
    }
}
