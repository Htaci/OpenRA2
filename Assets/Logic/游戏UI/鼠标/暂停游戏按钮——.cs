using UnityEngine;
using UnityEngine.UI;
public class 暂停游戏按钮 : MonoBehaviour
{
    public Canvas canvasToDisable;//要打开的画布
    
    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(打开暂停界面);
            
        }
    }

    void 打开暂停界面()
    {
        if (canvasToDisable != null)
        {
            canvasToDisable.gameObject.SetActive(true);
            Time.timeScale = 0; //暂停游戏
            Debug.Log("暂停游戏");
        }

    }
}
