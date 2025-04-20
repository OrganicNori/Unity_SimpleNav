using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 自定义的EditorGUI工具箱
/// </summary>
public static class EditorGUIKit
{

    /// <summary>
    /// 制作一个通用弹窗选择字段。
    /// </summary>
    /// <param name="selectIndex"></param>
    /// <param name="displayedOptions"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static int Popup(int selectIndex, string[] displayedOptions, params GUILayoutOption[] options)
    {

        int contrelId = GUIUtility.GetControlID(FocusType.Passive);

        if (GUILayout.Button(displayedOptions[selectIndex], options))
        {
            CustomPopup popup = new CustomPopup();
            popup.select = selectIndex;
            popup.displayedOptions = displayedOptions;
            popup.info = new CustomPopupInfo(contrelId, selectIndex);
            CustomPopupInfo.instance = popup.info;
            PopupWindow.Show(CustomPopupTempStyle.Get(contrelId).rect, popup);
        }

        if (Event.current.type == EventType.Repaint)
        {
            CustomPopupTempStyle style = new CustomPopupTempStyle();
            style.rect = GUILayoutUtility.GetLastRect();
            CustomPopupTempStyle.Set(contrelId, style);
        }
        return CustomPopupInfo.Get(contrelId, selectIndex);
    }

}

/// <summary>
/// 打开popup的选择界面
/// </summary>
public class CustomPopup : PopupWindowContent
{
    public int select;
    public string[] displayedOptions;
    public bool hasopen;
    string filter;
    public CustomPopupInfo info;

    Vector2 scrollPosition;
    public override void OnGUI(Rect rect)
    {
        editorWindow.minSize = new Vector2(200, 400);
        GUILayout.Label("搜索：");
        filter = EditorGUILayout.TextField(filter);
        GUILayout.Space(20);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < displayedOptions.Length; i++)
        {
            string info = displayedOptions[i];

            if (this.filter != null && this.filter.Length != 0)
            {
                if (!info.Contains(this.filter))
                {
                    continue;
                }
            }

            if (select == i)
            {
                info = "--->" + info;
            }
            if (GUILayout.Button(info))
            {
                select = i;
                this.info.Set(i);
                editorWindow.Close();
            }
        }
        EditorGUILayout.EndScrollView();
    }

    public override void OnOpen()
    {
        hasopen = true;
        base.OnOpen();
    }
}


/// <summary>
/// 自定义Popup的Style缓存可以有多个参数，不止是Rect，也可以自定义其他的
/// </summary>
public class CustomPopupTempStyle
{

    public Rect rect;

    static Dictionary<int, CustomPopupTempStyle> temp = new();

    public static CustomPopupTempStyle Get(int contrelId)
    {
        if (!temp.ContainsKey(contrelId))
        {
            return null;
        }
        CustomPopupTempStyle t;
        temp.Remove(contrelId, out t);
        return t;
    }

    public static void Set(int contrelId, CustomPopupTempStyle style)
    {
        temp[contrelId] = style;
    }
}

/// <summary>
/// 存储popup的信息如选择等
/// </summary>
public class CustomPopupInfo
{
    public int SelectIndex { get; private set; }
    public int contrelId;
    public bool used;
    public static CustomPopupInfo instance;

    public CustomPopupInfo(int contrelId, int selectIndex)
    {
        this.contrelId = contrelId;
        this.SelectIndex = selectIndex;
    }

    public static int Get(int controlID, int selected)
    {
        if (instance == null)
        {
            return selected;
        }

        if (instance.contrelId == controlID && instance.used)
        {
            GUI.changed = selected != instance.SelectIndex;
            selected = instance.SelectIndex;
            instance = null;
        }

        return selected;
    }

    public void Set(int selected)
    {
        SelectIndex = selected;
        used = true;
    }
}