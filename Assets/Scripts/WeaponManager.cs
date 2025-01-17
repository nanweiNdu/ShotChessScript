using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ����������
/// </summary>
public class WeaponManager : MonoBehaviour
{
    /// <summary>
    /// �����ӵ�ͼƬ�б�
    /// </summary>
    [SerializeField]
    private List<Image> WeaponBullets;
    /// <summary>
    /// ���������ӵ�ͼƬ�б�
    /// </summary>
    [SerializeField]
    private List<Image> WeaponBackBullets;

    /// <summary>
    /// ��ǰ��ϻ�е�ҩ����
    /// </summary>
    public int WeaponBulletNum;
    /// <summary>
    /// ��ǰ�����е�ҩ����
    /// </summary>
    private int WeaponBackBulletNum;
    /// <summary>
    /// ���ο������ĵ�ҩ����
    /// </summary>
    private int WeaponBulletUseNum =1;
    /// <summary>
    /// �������β��䵯ҩ����
    /// </summary>
    private int WeaponBackBulletGetNum;
    /// <summary>
    /// �����˺�
    /// </summary>
    public int WeaponDamage;
    /// <summary>
    /// ������͸��ֵ
    /// </summary>
    public int WeaponPenetrationQuantity;
    /// <summary>
    /// �������
    /// </summary>
    public int WeaponRange;

    private void Start()
    {
        WeaponBulletNum = 2;
        WeaponBackBulletNum = 6;
        WeaponDamage = 4;
        WeaponPenetrationQuantity= 0;
        WeaponRange = 3;
        AttributeManager.Instance.RegisterAttributeChange("CommonBout", OnCommonBout);
        AttributeManager.Instance.RegisterAttributeChange("DamageBout",OndamageBout);
    }
    /// <summary>
    /// �������ǹ����غ�ʱ
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnCommonBout(object sender, AttributeChangeEventArgs e)
    {
        //����ӵ�����
        if (WeaponBulletNum < 2)
        {
            //�������ӵ��㹻ʱ�����ӵ�
            if(WeaponBackBulletNum >= (2 - WeaponBulletNum))
            {
              
                WeaponBackBulletNum -= (2 - WeaponBulletNum);
                WeaponBulletNum = 2;
            }
            //�������ӵ�����ʱֻ����ʣ�౸���ӵ�
            else if(WeaponBackBulletNum >0)
            {
                WeaponBulletNum += WeaponBackBulletNum;
                WeaponBackBulletNum = 0;
            }
            //������û���ӵ�ʱΪ��������һ���ӵ�
            else
            {
                WeaponBackBulletNum += 1;
            }
        }
        else if(WeaponBackBulletNum <6)
        {
            WeaponBackBulletNum += 1;
        }
        UpdateBulletImages();
    }

    private void OndamageBout(object sender, AttributeChangeEventArgs e)
    {
        WeaponBulletNum -= WeaponBulletUseNum;
        UpdateBulletImages();
    }

    private void OnDestroy()
    {
        AttributeManager.Instance.UnregisterAttributeChange("CommonBout", OnCommonBout);
        AttributeManager.Instance.UnregisterAttributeChange("DamageBout", OndamageBout);
    }

    /// <summary>
    /// ���������ӵ��ͱ����ӵ���ͼƬ͸����
    /// </summary>
    private void UpdateBulletImages()
    {
        // ���������ӵ�ͼƬ
        for (int i = 0; i < WeaponBullets.Count; i++)
        {
            if (i < WeaponBulletNum)
            {
                // ����Ϊ��ȫ��͸��
                SetImageAlpha(WeaponBullets[i], 1f);
            }
            else
            {
                // ����Ϊ��ȫ͸��
                SetImageAlpha(WeaponBullets[i], 0f);
            }
        }

        // ���±����ӵ�ͼƬ
        for (int i = 0; i < WeaponBackBullets.Count; i++)
        {
            if (i < WeaponBackBulletNum)
            {
                // ����Ϊ��ȫ��͸��
                SetImageAlpha(WeaponBackBullets[i], 1f);
            }
            else
            {
                // ����Ϊ��ȫ͸��
                SetImageAlpha(WeaponBackBullets[i], 0f);
            }
        }
    }

    /// <summary>
    /// ���� Image ��͸����
    /// </summary>
    /// <param name="image">Ŀ��ͼƬ</param>
    /// <param name="alpha">͸����ֵ��0 ��ʾ��ȫ͸����1 ��ʾ��ȫ��͸����</param>
    private void SetImageAlpha(Image image, float alpha)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}
