using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class 文本显示 : MonoBehaviour
{
    public TextMeshProUGUI Tmp;
    // 消息区
    public GameObject GameObject;

    public bool isNewLine = false;
    public bool isRun = false; // 是否结束了打印
    public string str2 = ""; 

    public float waitTime = 0.2f; //字符间隔


    public void 输出左上角文本(string str)
    {
        StartCoroutine(Numerator(str));
    }

    IEnumerator Numerator(string str)
    {
        for (int i = 0; i < str.Length; ++i)
        {
            Tmp.text += str[i];

            float width = GetComponent<RectTransform>().rect.width;
            GameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(width + 30,26);

            if (width > 700)
            {
                //Debug.LogWarning("超出范围");
                str2 = str[i..];
                isNewLine = true;
                break;
            }
            yield return new WaitForSeconds(waitTime);
        }
        Tmp.text += " ";
    }

    // Update is called once per frame
    void Update()
    {
        //if (isNewLine)
        //{
        //    isNewLine = false;
        //    LuaTrigger LuaTrigger = FindObjectOfType<LuaTrigger>();
        //    LuaTrigger.Print(str2);
        //}
    }
}


