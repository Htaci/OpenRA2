using UnityEngine;
using UnityEngine.UI;
public class 继续游戏 : MonoBehaviour
{
    public Canvas canvasToDisable;//要关闭的画布
    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(回到游戏);
        }
    }

    void 回到游戏()
    {
        if (canvasToDisable != null)
        {
            Time.timeScale = 1; //恢复游戏
            Debug.Log("恢复游戏");
            canvasToDisable.gameObject.SetActive(false);
        }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
