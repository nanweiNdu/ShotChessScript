using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 武器管理器
/// </summary>
public class WeaponManager : MonoBehaviour
{
    /// <summary>
    /// 武器子弹图片列表
    /// </summary>
    [SerializeField]
    private List<Image> WeaponBullets;
    /// <summary>
    /// 武器备用子弹图片列表
    /// </summary>
    [SerializeField]
    private List<Image> WeaponBackBullets;

    /// <summary>
    /// 当前箭匣中弹药数量
    /// </summary>
    public int WeaponBulletNum;
    /// <summary>
    /// 当前箭袋中弹药数量
    /// </summary>
    private int WeaponBackBulletNum;
    /// <summary>
    /// 单次开火消耗弹药数量
    /// </summary>
    private int WeaponBulletUseNum =1;
    /// <summary>
    /// 箭袋单次补充弹药数量
    /// </summary>
    private int WeaponBackBulletGetNum;
    /// <summary>
    /// 武器伤害
    /// </summary>
    public int WeaponDamage;
    /// <summary>
    /// 武器穿透数值
    /// </summary>
    public int WeaponPenetrationQuantity;
    /// <summary>
    /// 武器射程
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
    /// 当经过非攻击回合时
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnCommonBout(object sender, AttributeChangeEventArgs e)
    {
        //如果子弹不满
        if (WeaponBulletNum < 2)
        {
            //当背包子弹足够时补满子弹
            if(WeaponBackBulletNum >= (2 - WeaponBulletNum))
            {
              
                WeaponBackBulletNum -= (2 - WeaponBulletNum);
                WeaponBulletNum = 2;
            }
            //当背包子弹不够时只补充剩余备用子弹
            else if(WeaponBackBulletNum >0)
            {
                WeaponBulletNum += WeaponBackBulletNum;
                WeaponBackBulletNum = 0;
            }
            //当背包没有子弹时为背包补充一发子弹
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
    /// 更新武器子弹和备用子弹的图片透明度
    /// </summary>
    private void UpdateBulletImages()
    {
        // 更新武器子弹图片
        for (int i = 0; i < WeaponBullets.Count; i++)
        {
            if (i < WeaponBulletNum)
            {
                // 设置为完全不透明
                SetImageAlpha(WeaponBullets[i], 1f);
            }
            else
            {
                // 设置为完全透明
                SetImageAlpha(WeaponBullets[i], 0f);
            }
        }

        // 更新备用子弹图片
        for (int i = 0; i < WeaponBackBullets.Count; i++)
        {
            if (i < WeaponBackBulletNum)
            {
                // 设置为完全不透明
                SetImageAlpha(WeaponBackBullets[i], 1f);
            }
            else
            {
                // 设置为完全透明
                SetImageAlpha(WeaponBackBullets[i], 0f);
            }
        }
    }

    /// <summary>
    /// 设置 Image 的透明度
    /// </summary>
    /// <param name="image">目标图片</param>
    /// <param name="alpha">透明度值（0 表示完全透明，1 表示完全不透明）</param>
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
