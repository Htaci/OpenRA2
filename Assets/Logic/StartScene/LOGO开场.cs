using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LOGO开场 : MonoBehaviour
{
    public CanvasGroup LOGO;
    public Animator animator;
    public GameObject GameObject;
    public float fadeInTime = 1f; // 淡入时间
    public float stayTime = 2f; // 停留时间
    public float fadeOutTime = 1f; // 淡出时间

    private void Awake()
    {
        if (LOGO == null)
        {
            Debug.LogError("LOGO没有被绑定!");
            return;
        }

        // 确保LOGO初始状态是不可见的
        LOGO.alpha = 0f;
        //LOGO.blocksRaycasts = false;
        //LOGO.interactable = false;

        // 开始淡入效果
        StartCoroutine(FadeInOut(fadeInTime, stayTime, fadeOutTime));
    }

    private IEnumerator FadeInOut(float fadeIn, float stay, float fadeOut)
    {
        GameObject.SetActive(true);
        // 淡入
        float elapsedTime = 0f;

        while (elapsedTime < 0.5f)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        elapsedTime = 0f;
        while (elapsedTime < fadeIn)
        {
            LOGO.alpha = Mathf.Lerp(0f, 1f, (elapsedTime / fadeIn));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 停留
        elapsedTime = 0f;
        animator.SetTrigger("LOGO");
        while (elapsedTime < stay)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        // 淡出
        elapsedTime = 0f;
        while (elapsedTime < fadeOut)
        {
            LOGO.alpha = Mathf.Lerp(1f, 0f, (elapsedTime / fadeOut));
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        GameObject.SetActive(false);
        // 淡出完成后，可以选择是否禁用LOGO对象
        //LOGO.blocksRaycasts = false;
        //LOGO.interactable = false;
        // Optionally, you can disable the GameObject if you want to free up resources
        // LOGO.gameObject.SetActive(false);
    }
}