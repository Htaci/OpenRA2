using UnityEngine;
using System;
using RA2ConversionTools; // 你的 PaletteReader 所在命名空间
// 注意：避免使用 System.Drawing 的 Color 名称，与 UnityEngine.Color 冲突

public static class PaletteManager
{
    // Unity 可用的调色板（每项为 UnityEngine.Color），全局可读写
    public static Color[] Palette { get; private set; } = null;

    // 加载指定路径的色盘文件（.pal），返回是否成功
    public static bool LoadPalette(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
                return false;

            // PaletteReader 返回的是 System.Drawing.Color[]
            var sdPalette = PaletteReader.ReadPalette(filePath);
            if (sdPalette == null || sdPalette.Length == 0)
                return false;

            Palette = new Color[sdPalette.Length];
            for (int i = 0; i < sdPalette.Length; i++)
            {
                var c = sdPalette[i];
                Palette[i] = new Color(c.R / 255f, c.G / 255f, c.B / 255f, 1f);
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"加载色盘失败: {e.Message}");
            Palette = null;
            return false;
        }
    }
}