using UnityEngine;
using UnityEngine.EventSystems;

public static class EventSystemBuilder
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateEventSystemIfMissing()
    {
        // 保证场景里最多只有 1 个 EventSystem
        if (EventSystem.current != null) return;

        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();   // PC / Mac / Linux 输入模块
        // 如果项目在用 Input System 包，可换成 InputSystemUIInputModule
    }


}