using System;
using Cleaning;
using Managers;
using ToolSystem;
using UI.Core;

public class PauseDialogue : Dialogue
{
    public Action Abandoned;

    private CleaningTimerManager timerManager;
    private ToolManager toolManager;

    private Tool previousTool;

    protected override void OnAwake() => Abandoned += OnAbandoned;

    protected override void OnClose()
    {
        timerManager.StartTimer();
        toolManager.SelectTool(previousTool);
    }
    protected override void OnPromote()
    {
        timerManager = M.GetOrThrow<CleaningTimerManager>();
        toolManager = M.GetOrThrow<ToolManager>();
        
        timerManager.StopTimer();
        previousTool = toolManager.CurrentTool;
        toolManager.SelectTool(null);
    }
    protected override void OnDemote() { }

    private void OnAbandoned() => canvasGroup.interactable = false;
}
