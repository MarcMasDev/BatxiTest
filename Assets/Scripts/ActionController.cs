using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public enum TrainingAction
{
    Resting = 0,
    Curl = 1,
    Run = 2,
    BenchPress = 3,
    LegTrain = 4,
}

public class ActionController : MonoBehaviour
{
    [Serializable]
    private class ActionEvent
    {
        [HideInInspector] public string name;
        [HideInInspector] public TrainingAction action;
        public UnityEvent onSelected = new UnityEvent();
        [Tooltip("Fired once the Animator finishes transitioning into this action. Enable character props.")]
        public UnityEvent onArrived = new UnityEvent();
    }

    [SerializeField] private UnityEvent onBeforeAction = new UnityEvent();

    [Tooltip("Don't repeat an action until every action has been played once (remembered across scene reloads during a play session).")]
    [SerializeField] private bool avoidRepeat = true;

    [Tooltip("Debug: always play Forced Action instead of a random one.")]
    [SerializeField] private bool forceAction;
    [ShowIf("forceAction")] [SerializeField] private TrainingAction forcedAction;

    [Tooltip("Press to switch to another action at runtime. Set to None to disable.")]
    [SerializeField] private Key nextActionKey = Key.Space;

    [SerializeField] private ActionEvent[] actions = Array.Empty<ActionEvent>();

    [SerializeField] private Animator animator;
    private Coroutine arrivalRoutine;

    public TrainingAction Current { get; private set; }

    private static readonly Dictionary<string, TrainingAction> lastActions = new Dictionary<string, TrainingAction>(); //Actions played this round
    private static readonly Dictionary<string, List<TrainingAction>> remainingActions = new Dictionary<string, List<TrainingAction>>(); //Actions not yet played in the current round

    private string HistoryKey => gameObject.scene.name + "/" + name;

    private void Start()
    {
        Play(PickAction());
    }

    private void Update()
    {
        if (nextActionKey != Key.None && Keyboard.current != null && Keyboard.current[nextActionKey].wasPressedThisFrame)
        {
            Play(PickAction());
        }
    }

    public void Play(TrainingAction action)
    {
        Current = action;
        lastActions[HistoryKey] = action;

        if (remainingActions.TryGetValue(HistoryKey, out List<TrainingAction> remaining))
        {
            remaining.Remove(action); //action picked, so remove it from remaining actions.
        }

        onBeforeAction.Invoke();

        ActionEvent entry = Array.Find(actions, a => a.action == action);
        if (entry == null) return;

        entry.onSelected.Invoke();

        if (arrivalRoutine != null) StopCoroutine(arrivalRoutine);
        arrivalRoutine = StartCoroutine(InvokeWhenArrived(entry.onArrived));
    }

    public void Play(int action)
    {
        Play((TrainingAction)action);
    }

    private TrainingAction PickAction()
    {
        if (forceAction) return forcedAction;

        var values = (TrainingAction[])Enum.GetValues(typeof(TrainingAction));

        if (!avoidRepeat) return values[UnityEngine.Random.Range(0, values.Length)];

        if (!remainingActions.TryGetValue(HistoryKey, out List<TrainingAction> remaining) || remaining.Count == 0)
        {
            remaining = new List<TrainingAction>(values);
            remainingActions[HistoryKey] = remaining;
        }

        int index = UnityEngine.Random.Range(0, remaining.Count);

        //At the start of a new round the last action is back in the list, don't let it play twice in a row.
        if (remaining.Count > 1 && lastActions.TryGetValue(HistoryKey, out TrainingAction last) && remaining[index] == last)
        {
            index = (index + 1 + UnityEngine.Random.Range(0, remaining.Count - 1)) % remaining.Count;
        }

        return remaining[index];
    }

    private void OnValidate()
    {
        var values = (TrainingAction[])Enum.GetValues(typeof(TrainingAction));
        var existing = actions ?? Array.Empty<ActionEvent>();
        var synced = new ActionEvent[values.Length];

        for (int i = 0; i < values.Length; i++)
        {
            synced[i] = Array.Find(existing, a => a != null && a.action == values[i]) ?? new ActionEvent();
            synced[i].action = values[i];
            synced[i].name = values[i].ToString();
        }

        actions = synced;
    }

    private IEnumerator InvokeWhenArrived(UnityEvent onArrived)
    {
        yield return null;

        while (animator != null && animator.IsInTransition(0))
            yield return null;

        arrivalRoutine = null;
        onArrived.Invoke();
    }
}
