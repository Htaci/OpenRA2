using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFB;
using UnityEngine.EventSystems;

public class 打开项目按钮 : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        var path = StandaloneFileBrowser.OpenFolderPanel("选择项目文件夹", "", false);
    }
}
