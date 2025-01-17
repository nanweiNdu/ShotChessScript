using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 属性变化事件控制器
/// </summary>
public class AttributeManager : MonoBehaviour
{
    public static AttributeManager Instance { get; private set; }
    // 使用字典来存储不同属性的事件
    private Dictionary<string, EventHandler<AttributeChangeEventArgs>> attributeEvents = new Dictionary<string, EventHandler<AttributeChangeEventArgs>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// 注册属性变化的事件处理器
    /// </summary>
    /// <param name="attributeName"></param>
    /// <param name="handler"></param>
    public void RegisterAttributeChange(string attributeName, EventHandler<AttributeChangeEventArgs> handler)
    {
        if (!attributeEvents.ContainsKey(attributeName))
        {
            attributeEvents[attributeName] = null;
        }
        attributeEvents[attributeName] += handler;
    }
    /// <summary>
    /// 注销属性变化的事件处理器
    /// </summary>
    /// <param name="attributeName"></param>
    /// <param name="handler"></param>
    public void UnregisterAttributeChange(string attributeName, EventHandler<AttributeChangeEventArgs> handler)
    {
        if (attributeEvents.ContainsKey(attributeName))
        {
            attributeEvents[attributeName] -= handler;
        }
    }
    /// <summary>
    /// 通知属性变化
    /// </summary>
    /// <param name="attributeName"></param>
    /// <param name="newValue"></param>
    /// <param name="changeAmount"></param>
    public bool NotifyAttributeChange(string attributeName, float newValue , float changeAmount , int triggerCount,Vector3 vector = new())
    {
        if (attributeEvents.ContainsKey(attributeName) && attributeEvents[attributeName] != null)
        {
            attributeEvents[attributeName].Invoke(this, new AttributeChangeEventArgs(attributeName, newValue, changeAmount,triggerCount,vector));
            return true; // 表示有回应
        }

        return false; // 表示没有回应
    }
}
/// <summary>
/// 属性变化事件属性类
/// </summary>
public class AttributeChangeEventArgs : EventArgs
{
    /// <summary>
    /// 事件名称
    /// </summary>
    public string AttributeName { get; private set; }
    /// <summary>
    /// 变化后的值
    /// </summary>
    public float NewValue { get; private set; }
    /// <summary>
    /// 值的变化量
    /// </summary>
    public float ChangeAmount { get; private set; }
    /// <summary>
    /// 触发次数参数
    /// </summary>
    public int TriggerCount { get; private set; } // 新增触发次数参数
    public Vector3  Vector { get; private set; }
    public AttributeChangeEventArgs(string attributeName, float newValue, float changeAmount,int triggerCount,Vector3 vector)
    {
 
       AttributeName = attributeName;
        NewValue = newValue;
        ChangeAmount = changeAmount;
        TriggerCount = triggerCount;
        Vector = vector;
    }
}