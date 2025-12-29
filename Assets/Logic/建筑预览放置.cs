using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

public class 建筑预览放置 : MonoBehaviour
{
    public List<GameObject> 占地; // 将预览建筑预制件里的建筑下方的格子放到这里面
    private List<bool> 上次占地是否可以放置;
    public GameObject 建筑; // 预览建筑的建筑本身
    private GridManager gridManager;
    public Material 建筑Material白; // 建筑预览材质
    public Material 建筑Material红;
    public Material 建筑Material绿;
    public Material 占地Material白; // 占地方块材质
    public Material 占地Material红;
    public Material 占地Material绿;
    private bool is建筑是否可以放置 = false;
    private bool is上次建筑是否可以放置 = false ;

    private Renderer renderera;
    private Material currentMaterial;

    void Update()
    {

        for (int i = 0; i < 占地.Count; i++)
        {
            GameObject obj = 占地[i];
            if (obj != null)
            {
                // 获取对象的世界坐标
                Vector3 worldPosition = obj.transform.position;
                Vector3 gridPosition = new Vector3(Mathf.Round(worldPosition.x), Mathf.Round(worldPosition.y), Mathf.Round(worldPosition.z));
                int gridInfo = gridManager.GetGridInfo((int)gridPosition.x, (int)gridPosition.z);
                
                if (gridInfo == -1)
                {
                    if (上次占地是否可以放置[i] != false)
                    {
                        renderera = obj.GetComponent<Renderer>();
                        // 将材质实例应用到目标对象上
                        renderera.material = 占地Material红;
                        //Debug.LogWarning("不能放置！");
                        上次占地是否可以放置[i] = false;
                        is建筑是否可以放置 = false;
                    }
                }
                else
                {
                    if (上次占地是否可以放置[i] != true)
                    {
                        renderera = obj.GetComponent<Renderer>();
                        // 将材质实例应用到目标对象上
                        renderera.material = 占地Material绿;
                        //Debug.LogWarning("可以放置！");
                        上次占地是否可以放置[i] = true;
                        is建筑是否可以放置 = true;
                    }
                }
            }
            else
            {
                Debug.LogWarning("列表中存在空的GameObject引用！");
            }
        }

        if (is建筑是否可以放置 != is上次建筑是否可以放置)
        {
            renderera = 建筑.GetComponent<Renderer>();
            if (is建筑是否可以放置)
            {
                renderera.material = 建筑Material绿;
                is上次建筑是否可以放置 = true ;
            }
            else
            {
                renderera.material = 建筑Material红;
                is上次建筑是否可以放置 = false ;
            }
            
        }

    }
    void Start()
    {
        GameObject gridObject = GameObject.Find("网格地图");
        if (gridObject != null)
        {
            gridManager = gridObject.GetComponent<GridManager>();
        }
        else
        {
            Debug.LogError("找不到网格管理器对象!");
        }

        if (上次占地是否可以放置 == null)
        {
            上次占地是否可以放置 = new List<bool>();
        }
        上次占地是否可以放置.Add(false);

        for (int i = 0; i < 占地.Count; i++)
        {
            上次占地是否可以放置.Add(false);
        }


        占地材质实例();
        建筑材质实例();
    }

    void 占地材质实例()
    {
        foreach (GameObject obj in 占地)
        {
            if (obj != null)
            {
                // 获取目标对象的Renderer组件
                Renderer renderer = obj.GetComponent<Renderer>();
                // 将材质实例应用到目标对象上
                renderer.material = 占地Material红;
            }
            else
            {
                Debug.LogWarning("列表中存在空的GameObject引用！");
            }
        }

    }
    void 建筑材质实例()
    {
        // 获取目标对象的Renderer组件
        Renderer renderer = 建筑.GetComponent<Renderer>();
        // 将材质实例应用到目标对象上
        renderer.material = 建筑Material红;

    }
}
