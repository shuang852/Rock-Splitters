using System;
using Cleaning;
using Managers;
using UI.Core;

public class PauseDialogue : Dialogue
{
    public Action Abandoned;

    private CleaningTimerManager timerManager;

    protected override void OnAwake() => Abandoned += OnAbandoned;

    private void Start()
    {
        timerManager = M.GetOrThrow<CleaningTimerManager>();
    }

    protected override void OnClose()
    {
        timerManager.StartTimer();
        // TODO: Enable the cleaning controls...
    }
    protected override void OnPromote()
    {
        timerManager.StopTimer();
        // TODO: Disable the cleaning controls...
    }
    protected override void OnDemote() { }

    private void OnAbandoned() => canvasGroup.interactable = false;
}
