using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

/// <summary>
/// Various Helper methods that enable quick access to things in Unity
/// </summary>
public static class HelperMethods
{
    public static List<GameObject> GetChildrenList(this GameObject go)
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform tran in go.transform)
        {
            children.Add(tran.gameObject);
        }
        return children;
    }

    public static GameObject[] GetChildrenArray(this GameObject go)
    {
        return GetChildrenList(go).ToArray();
    }

    public static List<T> GetChildrenList<T>(this GameObject go) where T : Component
    {
        List<T> children = new List<T>();
        foreach (Transform tran in go.transform)
        {
            T component = tran.gameObject.GetComponent<T>();
            if (component != null)
            {
                children.Add(component);
            }
        }
        return children;
    }

    public static List<T> GetGrandChildrenList<T>(this GameObject go, int indexOfGrandchild) where T : Component
    {
        List<T> children = new List<T>();
        foreach (Transform tran in go.transform)
        {
            T component = tran.GetChild(indexOfGrandchild).GetComponent<T>();
            if (component != null)
            {
                children.Add(component);
            }
        }
        return children;
    }

    public static string GetTimeSpanString(this float seconds, string format = @"mm\:ss")
    {
        TimeSpan floatAsTimeSpan = TimeSpan.FromSeconds(seconds);
       return $"{floatAsTimeSpan.ToString(format)}";
    }
    public static string GetTimeSpanString(this int seconds, string format = @"mm\:ss")
    {
        TimeSpan floatAsTimeSpan = TimeSpan.FromSeconds(seconds);
        return $"{floatAsTimeSpan.ToString(format)}";
    }
}