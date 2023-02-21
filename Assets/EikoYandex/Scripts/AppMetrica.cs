using System.Runtime.InteropServices;
using UnityEngine;

public static class AppMetricaWeb
{
    public const string Default = "reachGoal";
    [DllImport("__Internal")]
    private static extern void SendEvent(string key, string EventName);

    public static void Event(string eventName)
    {
        Debug.Log("AppMetrica Event :"+eventName);
#if UNITY_WEBGL && !UNITY_EDITOR
        SendEvent(Default, eventName);
#endif
    }

    public static void Event(string key, string eventName)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SendEvent(key, eventName);
#endif
    }
}
