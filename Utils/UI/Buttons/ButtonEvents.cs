using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using Replay.Utils;

[RequireComponent(typeof(Button))]
public class ButtonEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [Serializable]
    public class ButtonEventType : UnityEvent { }

    public ButtonEventType onTouchDown = new ();
    public ButtonEventType onTouchUp = new ();
    public ButtonEventType onClick = new ();

    public enum Mode
    {
        EventHandler,
        EventTrigger
    };
    [SerializeField] public Mode mode = Mode.EventHandler;

    EventTrigger eventTrigger;
    Button button;

    private void Awake()
    {
        if (mode == Mode.EventTrigger)
            SetupEventTriggers();    
    }

    private void OnDestroy()
    {
        ClearEventTriggers();
    }

    #region Event Triggers

    void AddEventTrigger(EventTriggerType type, Action<BaseEventData> eventHandler)
    {
        var entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((data) => { if (button.interactable) eventHandler?.Invoke(data); });
        eventTrigger.triggers.Add(entry);
    }
    void ClearEventTriggers()
    {
        if(eventTrigger != null)
            eventTrigger.triggers.Clear();
    }
    void SetupEventTriggers()
    {
        eventTrigger = gameObject.GetOrAddComponent<EventTrigger>();
        button = gameObject.GetOrAddComponent<Button>();
        
        ClearEventTriggers();

        AddEventTrigger(EventTriggerType.PointerDown, (data) => PerformOnTouchDown());
        AddEventTrigger(EventTriggerType.PointerUp, (data) => PerformOnTouchUp());
        AddEventTrigger(EventTriggerType.PointerClick, (data) => PerformOnClick());
    }

    #endregion
    
    #region Event Handlers
    
    public void OnPointerDown (PointerEventData eventData) => PerformOnTouchDown();
    public void OnPointerUp (PointerEventData eventData) => PerformOnTouchUp();
    public void OnPointerClick(PointerEventData eventData) => PerformOnClick();
    
    #endregion
    
    #region Delegate Implementation

    void ForEachButtonEvent(Action<IButtonEvents> action)
    {
        var buttonEvents = GetComponents<IButtonEvents>();
        if (buttonEvents != null && buttonEvents.Length > 0)
        {
            foreach (var buttonEvent in buttonEvents)
                action?.Invoke(buttonEvent);    
        }
    }
    
    public void PerformOnClick()
    {
        ForEachButtonEvent((buttonEvent) => buttonEvent.OnClick());
        onClick.Invoke();
    }
    public void PerformOnTouchDown()
    {
        ForEachButtonEvent((buttonEvent) => buttonEvent.OnTouchDown());
        onTouchDown.Invoke();
    }
    public void PerformOnTouchUp()
    {
        ForEachButtonEvent((buttonEvent) => buttonEvent.OnTouchUp());
        onTouchUp.Invoke();
    }
    #endregion
}