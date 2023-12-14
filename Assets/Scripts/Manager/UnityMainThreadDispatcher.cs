using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher instance;

    private Queue<Action> actionQueue = new Queue<Action>();

    private static object lockObject = new object();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    void Update()
    {
        lock (lockObject)
        {
            while (actionQueue.Count > 0)
            {
                actionQueue.Dequeue().Invoke();
            }
        }
    }

    public void Enqueue(Action action)
    {
        lock (lockObject)
        {
            actionQueue.Enqueue(action);
        }
    }

    public static UnityMainThreadDispatcher Instance()
    {
        if (instance == null)
        {
            GameObject singleton = new GameObject("UnityMainThreadDispatcher");
            instance = singleton.AddComponent<UnityMainThreadDispatcher>();
        }

        return instance;
    }
}
