using System;
using System.Collections.Generic;
using System.Threading;
using Replay.Utils;
using UnityEngine;
using UnityEngine.Scripting;

public class ThreadUtils : ComponentSingleton<ThreadUtils>, IDebugLoggable
{
    [Preserve]
    public static bool ShouldCreateOwnGameObject() => true;
    [Preserve]
    public static bool ShouldEnableDontDestroyOnLoad() => true;
    //[Preserve]
    //public static bool ShouldNotRenameGameObject() => true;
    
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize() =>
        Load();

    volatile bool _queued = false;
    List<Action> _backlog = new (8);
    List<Action> _actions = new (8);
    
    public void ExecuteOnMainThread(Action action)
    {
        var mainThreadContext = System.Threading.SynchronizationContext.Current;
        mainThreadContext.Post(_ => action?.Invoke(), null);
    }
    public void RunAsync(Action action) {
        ThreadPool.QueueUserWorkItem(o => action());
    }
  
    public void RunAsync(Action<object> action, object state) {
        ThreadPool.QueueUserWorkItem(o => action(o), state);
    }
  
    public void ExecuteOnMainThreadUpdate(Action action)
    {
        lock(_backlog) {
            _backlog.Add(action);
            _queued = true;
        }
    }
    
    private void Update()
    {
        if(_queued)
        {
            lock(_backlog) {
                (_actions, _backlog) = (_backlog, _actions);
                _queued = false;
            }
  
            foreach(var action in _actions)
                action();
  
            _actions.Clear();
        }
    }
  
    public string ToDebugString()
    {
        var count = _actions.Count;
        return $"Jobs Count: {count}";
    }
    
}
