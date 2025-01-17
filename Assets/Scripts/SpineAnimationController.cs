using UnityEngine;
using Spine.Unity;

public class SpineAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation; // Spine 动画组件
    private Spine.AnimationState spineAnimationState;

    private const string SitAnimation = "move";
    private const string AttackAnimation = "attack";

    private void Start()
    {
        if (skeletonAnimation == null)
        {
            Debug.LogError("未指定 SkeletonAnimation 组件！");
            return;
        }

        // 获取 Spine 的 AnimationState
        spineAnimationState = skeletonAnimation.AnimationState;

        // 设置默认循环动画
        PlaySitAnimation();
    }

    /// <summary>
    /// 播放默认的 sit_2 循环动画
    /// </summary>
    private void PlaySitAnimation()
    {
        spineAnimationState.SetAnimation(0, SitAnimation, true); // 通道 0，循环播放
    }

    /// <summary>
    /// 播放一次 attack 动画，然后切回 sit_2
    /// </summary>
    public void PlayAttackAnimation()
    {
        // 清空当前动画队列
        spineAnimationState.ClearTracks();

        // 播放 attack 动画（不循环）
        spineAnimationState.SetAnimation(0, AttackAnimation, false);

        // 动画结束后切回 sit_2
        spineAnimationState.AddAnimation(0, SitAnimation, true, 0);
    }
}
