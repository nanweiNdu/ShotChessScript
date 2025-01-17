using UnityEngine;
using Spine.Unity;

public class SpineAnimationController : MonoBehaviour
{
    [SerializeField] private SkeletonAnimation skeletonAnimation; // Spine �������
    private Spine.AnimationState spineAnimationState;

    private const string SitAnimation = "move";
    private const string AttackAnimation = "attack";

    private void Start()
    {
        if (skeletonAnimation == null)
        {
            Debug.LogError("δָ�� SkeletonAnimation �����");
            return;
        }

        // ��ȡ Spine �� AnimationState
        spineAnimationState = skeletonAnimation.AnimationState;

        // ����Ĭ��ѭ������
        PlaySitAnimation();
    }

    /// <summary>
    /// ����Ĭ�ϵ� sit_2 ѭ������
    /// </summary>
    private void PlaySitAnimation()
    {
        spineAnimationState.SetAnimation(0, SitAnimation, true); // ͨ�� 0��ѭ������
    }

    /// <summary>
    /// ����һ�� attack ������Ȼ���л� sit_2
    /// </summary>
    public void PlayAttackAnimation()
    {
        // ��յ�ǰ��������
        spineAnimationState.ClearTracks();

        // ���� attack ��������ѭ����
        spineAnimationState.SetAnimation(0, AttackAnimation, false);

        // �����������л� sit_2
        spineAnimationState.AddAnimation(0, SitAnimation, true, 0);
    }
}
