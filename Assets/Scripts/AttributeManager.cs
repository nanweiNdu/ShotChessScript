using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ���Ա仯�¼�������
/// </summary>
public class AttributeManager : MonoBehaviour
{
    public static AttributeManager Instance { get; private set; }
    // ʹ���ֵ����洢��ͬ���Ե��¼�
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
    /// ע�����Ա仯���¼�������
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
    /// ע�����Ա仯���¼�������
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
    /// ֪ͨ���Ա仯
    /// </summary>
    /// <param name="attributeName"></param>
    /// <param name="newValue"></param>
    /// <param name="changeAmount"></param>
    public bool NotifyAttributeChange(string attributeName, float newValue , float changeAmount , int triggerCount,Vector3 vector = new())
    {
        if (attributeEvents.ContainsKey(attributeName) && attributeEvents[attributeName] != null)
        {
            attributeEvents[attributeName].Invoke(this, new AttributeChangeEventArgs(attributeName, newValue, changeAmount,triggerCount,vector));
            return true; // ��ʾ�л�Ӧ
        }

        return false; // ��ʾû�л�Ӧ
    }
}
/// <summary>
/// ���Ա仯�¼�������
/// </summary>
public class AttributeChangeEventArgs : EventArgs
{
    /// <summary>
    /// �¼�����
    /// </summary>
    public string AttributeName { get; private set; }
    /// <summary>
    /// �仯���ֵ
    /// </summary>
    public float NewValue { get; private set; }
    /// <summary>
    /// ֵ�ı仯��
    /// </summary>
    public float ChangeAmount { get; private set; }
    /// <summary>
    /// ������������
    /// </summary>
    public int TriggerCount { get; private set; } // ����������������
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