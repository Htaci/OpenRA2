using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonTrigger : MonoBehaviour
{
    public Toggle[] smallCategoryToggles; // 存储所有小分类Toggles的数组

    public Image targetImage; // 要修改光线投射目标的对象
    public Animator animator; // 控制动画的 Animator
    public GameObject panel个人; // 要在动画中途激活的面板
    public GameObject panel战役;
    public float panelActivateTime = 0.24f; // 激活面板的时间
    public float animationDuration = 1f; // 动画时长

    public void 主页切换到个人界面()
    {
        // 修改光线投射目标为开启
        targetImage.raycastTarget = true;

        // 按钮触发动画
        animator.SetTrigger("HomeToUser");
    }

    public void 动画修改光透()
    {
        // 动画触发
        targetImage.raycastTarget = false;
    }
    public void 动画打开面板()
    {
        // 动画触发
        panel个人.SetActive(true);
    }


    public void 主页切换到战役界面()
    {
        // 修改光线投射目标为开启
        targetImage.raycastTarget = true;
        Debug.Log("战役动画被触发");
        // 触发动画
        animator.SetTrigger("主页到战役过度");
    }

    public void 战役切换到主页界面()
    {
        // 修改光线投射目标为开启
        targetImage.raycastTarget = true;
        Debug.Log("切换到主页");
        // 触发动画
        animator.SetTrigger("战役到主页过度");
    }
    public void 个人切换到主页界面()
    {
        // 修改光线投射目标为开启
        targetImage.raycastTarget = true;
        // 触发动画
        animator.SetTrigger("个人到主页");
    }

    public void 关闭个人面板()
    {
        panel个人.SetActive(false);
    }
    public void 关闭战役面板()
    {
        panel战役.SetActive(false);
    }
    public void 打开战役面板()
    {
        panel战役.SetActive(true);
        targetImage.raycastTarget = false;
    }

    public void OnMajorCategoryChanged()
    {
        if (smallCategoryToggles.Length > 0)
        {
            smallCategoryToggles[0].isOn = true;
            // 确保只有第一个Toggle是激活状态
            for (int i = 1; i < smallCategoryToggles.Length; i++)
            {
                smallCategoryToggles[i].isOn = false;
            }
        }
    }
}
