using UnityEngine;

public class GameSettingsManager : MonoBehaviour
{
    public static GameSettingsManager Instance { get; private set; }

    private const string RunInBackgroundKey = "RunInBackground";
    private const string WindowModeKey = "WindowMode";
    private const string ResolutionKey = "Resolution";

    void Awake()
    {
        // 确保只存在一个实例
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 初始化设置
        LoadSettings();
    }

    public void OnRunInBackgroundToggleChanged(bool isOn)
    {
        Application.runInBackground = isOn;
        PlayerPrefs.SetInt(RunInBackgroundKey, isOn ? 1 : 0); // 保存设置
        PlayerPrefs.Save(); // 确保设置已保存
    }

    public void OnWindowModeDropdownChanged(int mode)
    {
        if (mode == 0) // 无边窗口模式
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else if (mode == 1) // 全屏模式
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        PlayerPrefs.SetInt(WindowModeKey, mode);
        PlayerPrefs.Save();
    }

    public void OnResolutionDropdownChanged(int resolutionIndex)
    {
        switch (resolutionIndex)
        {
            case 0: // 1920x1080
                Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
                break;
            case 1: // 3840x2160
                Screen.SetResolution(3840, 2160, Screen.fullScreenMode);
                break;
        }
        PlayerPrefs.SetInt(ResolutionKey, resolutionIndex);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        // 加载 RunInBackground 设置
        bool runInBackground = PlayerPrefs.GetInt(RunInBackgroundKey, 1) == 1; // 默认值为 1（true）
        Application.runInBackground = runInBackground;

        // 加载 WindowMode 设置
        int windowMode = PlayerPrefs.GetInt(WindowModeKey, 0); // 默认值为 0（无边窗口模式）
        if (windowMode == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // 全屏窗口
        }
        else if (windowMode == 1)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; // 独占全屏
        }
        else if(windowMode == 2)
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow; // 最大化窗口
        }
        else if(windowMode == 3)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed; // 窗口
        }

        // 加载 Resolution 设置
        int resolutionIndex = PlayerPrefs.GetInt(ResolutionKey, 0); // 默认值为 0（1920x1080）
        if (resolutionIndex == 0)
        {
            Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
        }
        else if (resolutionIndex == 1)
        {
            Screen.SetResolution(3840, 2160, Screen.fullScreenMode);
        }
    }


}


public static class Settings
{
    public static PlayerSettingsA playerSettings = new PlayerSettingsA();


    public static void LoadSettings()
    {
        // 当游戏窗口失去焦点时停止运行
        Application.runInBackground = false;

        // 加载 WindowMode 设置
        int windowMode =3; // 默认值为 0（无边窗口模式）
        if (windowMode == 0)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // 无边全屏窗口
        }
        else if (windowMode == 1)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; // 独占全屏
        }
        else if (windowMode == 2)
        {
            Screen.fullScreenMode = FullScreenMode.MaximizedWindow; // 最大化窗口
        }
        else if (windowMode == 3)
        {
            Screen.fullScreenMode = FullScreenMode.Windowed; // 窗口
        }

        // 加载 Resolution 设置
        //int resolutionIndex = PlayerPrefs.GetInt(ResolutionKey, 0); // 默认值为 0（1920x1080）
        //if (resolutionIndex == 0)
        //{
        //    Screen.SetResolution(1920, 1080, Screen.fullScreenMode);
        //}
        //else if (resolutionIndex == 1)
        //{
        //    Screen.SetResolution(3840, 2160, Screen.fullScreenMode);
        //}
    }
}

public class PlayerSettingsA
{
    // 玩家名字
    public string playerName;
    // 玩家唯一标识符
    public int UID;
    // 玩家在线状态
    public int playerOnlineStatus;

}

