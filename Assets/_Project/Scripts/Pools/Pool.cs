// Script adapted from my group's project "MultiplayerMages" for the Networks & Multiplayer Systems subject
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class Pool<T> where T : UnityEngine.Object
{
    private readonly List<T> m_Items = new();
    private readonly Stack<T> m_AvailableItems = new();
    private readonly Func<T> m_CreateFunc;

    public Pool(Func<T> createFunc)
    {
        m_CreateFunc = createFunc ?? throw new ArgumentNullException(nameof(createFunc));
    }

    public T Get()
    {
        while (m_AvailableItems.Count > 0)
        {
            T pooledObject = m_AvailableItems.Pop();
            if (pooledObject != null)
            {
                SetActive(pooledObject, true);
                return pooledObject;
            }
        }

        T newObject = m_CreateFunc();
        if (newObject != null)
        {
            m_Items.Add(newObject);
            SetActive(newObject, true);
        }

        return newObject;
    }

    public void Release(T element)
    {
        if (element == null)
        {
            return;
        }

        if (IsInactive(element))
        {
            return;
        }

        SetActive(element, false);
        m_AvailableItems.Push(element);
    }

    public void Clear()
    {
        for (int i = 0; i < m_Items.Count; i++)
        {
            T pooledObject = m_Items[i];
            if (pooledObject == null)
            {
                continue;
            }

            DestroyPooledObject(pooledObject);
        }

        m_Items.Clear();
        m_AvailableItems.Clear();
    }

    public void Prewarm(int count)
    {
        if (count <= 0)
        {
            return;
        }

        List<T> tempObjects = new List<T>(count);
        for (int i = 0; i < count; i++)
        {
            T pooledObject = Get();
            if (pooledObject != null)
            {
                tempObjects.Add(pooledObject);
            }
        }

        for (int i = 0; i < tempObjects.Count; i++)
        {
            Release(tempObjects[i]);
        }
    }

    private static bool IsInactive(T obj)
    {
        if (obj is GameObject gameObject)
        {
            return !gameObject.activeSelf;
        }

        if (obj is Component component)
        {
            return !component.gameObject.activeSelf;
        }

        return true;
    }

    private static void SetActive(T obj, bool isActive)
    {
        if (obj is GameObject gameObject)
        {
            gameObject.SetActive(isActive);
            return;
        }

        if (obj is Component component)
        {
            component.gameObject.SetActive(isActive);
        }
    }

    private static void DestroyPooledObject(T obj)
    {
        if (obj is GameObject gameObject)
        {
            UnityEngine.Object.Destroy(gameObject);
            return;
        }

        if (obj is Component component)
        {
            UnityEngine.Object.Destroy(component.gameObject);
            return;
        }

        UnityEngine.Object.Destroy(obj);
    }
}
