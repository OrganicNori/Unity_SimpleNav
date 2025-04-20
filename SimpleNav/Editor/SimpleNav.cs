
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using System.Collections;
using System.Collections.Generic;
public class SimpleNav : EditorWindow
{
    //private SerializedProperty _conversations;
    ReorderableList reorderableList;
    //List<Element> data = new List<Element>();
    public ElementList sceneViewList;
    Vector2 scrollPos;
    Transform currentCameraTf;
    private Vector3 PivotPoint;
    private float SceneSize;    //
    private string assetPath;

    // private const string filePath = "Assets/SimpleNav/Editor/SceneNav.asset";
    // private int SaveTime;
    //
    void Awake()
    {
        string SceneNavPath = GetAssetPath();
        assetPath = GetAssetPath("SceneFile/");//获取当前目录下的SceneFile文件夹路径


        if (File.Exists(SceneNavPath))
        {
            Debug.Log("SimpleNav:触发了文件存在");
            sceneViewList = AssetDatabase.LoadAssetAtPath<ElementList>(SceneNavPath);
            //sceneViewList = AssetDatabase.LoadAssetAtPath<ElementList>(filePath);

        }
        else
        {
            Debug.Log("SimpleNav:触发了文件不存在,重新生成");

            sceneViewList = CreateInstance<ElementList>();
            AssetDatabase.CreateAsset(sceneViewList, SceneNavPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        //sceneViewList加载完成时，触发一个方法

    }
    //默认无参为SceneNav.asset,有参则使用参数为相对路径
    private string GetAssetPath(string relativePath = "SceneNav.asset")
    {
        // 获取当前脚本所在目录
        var script = MonoScript.FromScriptableObject(this);
        string scriptPath = AssetDatabase.GetAssetPath(script);
        string scriptDirectory = Path.GetDirectoryName(scriptPath);

        // 返回相对于脚本目录的路径
        return Path.Combine(scriptDirectory, relativePath);
    }


    // 定义一个菜单项
    [MenuItem("Tools/Scene Nav Window")]
    public static void ShowWindow()
    {
        // 获取窗口
        GetWindow<SimpleNav>("SceneNav");
    }
    private void OnEnable()
    {

        //读取当前目录下的ElementList文件
        reorderableList = new ReorderableList(sceneViewList.data, typeof(Element));
        //设置单个元素的高度
        reorderableList.elementHeight = 105;
        //绘制单个元素
        reorderableList.drawElementCallback = OnElementCallback;
        reorderableList.onAddCallback = OnAddCallback;
        // reorderableList.onMouseDragCallback = OnMouseDragCallback;
        reorderableList.onRemoveCallback = OnRemoveCallback;
        // reorderableList.onSelectCallback = OnSelectCallback;
        reorderableList.onChangedCallback = OnChangedCallback;
        reorderableList.onReorderCallback = OnReorderCallback;

        //背景色
        reorderableList.drawElementBackgroundCallback = OnElementBackgroundCallback;

        //头部
        reorderableList.drawHeaderCallback = OnHeaderCallback;


    }
    private void OnReorderCallback(ReorderableList list)
    {
        EditorUtility.SetDirty(sceneViewList);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    private void OnChangedCallback(ReorderableList list)
    {
        Debug.Log("OnChangedCallback");

        /* //设置更改的元素为ditry并且保存
        Debug.Log("更改了数值:INDEX:" + list.index + sceneViewList.data[list.index]);
        EditorUtility.SetDirty(sceneViewList.data[list.index]);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh(); */
    }
    void SaveChange()
    {
        Debug.Log("SceneNav保存了当前属性");
        for (int i = 0; i < sceneViewList.data.Count; i++)
        {
            EditorUtility.SetDirty(sceneViewList.data[i]);
        }
        AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
    }
    /*   private void OnMouseDragCallback(ReorderableList list)
      {
          GUI.color = Color.green;
      }
      private void OnSelectCallback(ReorderableList list)
      {
          GUI.color = Color.green;
      } */
    private void OnRemoveCallback(ReorderableList list)
    {
        if (EditorUtility.DisplayDialog("是否移除", "是否移除", "是", "否"))
        {
            var RemoveiD = list.index;
            var NAME = sceneViewList.data[RemoveiD].name;
            string PngName = sceneViewList.data[RemoveiD].icon.name;
            //使用assetdatabase清除对应目录的文件
            //使用sceneViewList.data清除对应的文件
            sceneViewList.data.RemoveAt(RemoveiD);
            //AssetDatabase.DeleteAsset(assetPath + NAME);
            ClearFiles(assetPath + NAME, assetPath + PngName);
            Repaint();


        }

        //使用assetdatabase清除对应目录的文件
        //使用sceneViewList.data清除对应的文件

    }
    private void OnRemoveAtIndex(int index)
    {
        if (index < 0 || index >= sceneViewList.data.Count)
            return;

        if (EditorUtility.DisplayDialog("是否移除", "是否移除", "是", "否"))
        {
            var element = sceneViewList.data[index];
            string elementName = element.name; // 或者使用其他唯一标识
            string PngName = element.icon.name;

            // 先删除资源
            ClearFiles(assetPath + elementName, assetPath + PngName);

            // 然后从列表中移除
            sceneViewList.data.RemoveAt(index);

            // 强制立即刷新UI
            Repaint();
        }

    }
    //定义一个携程方法
    private void OnAddCallback(ReorderableList list)
    {
        Debug.Log("触发了一个添加");
        Element newElement = ScriptableObject.CreateInstance<Element>();
        //检查目录是否存在
        if (!Directory.Exists(assetPath))
            Directory.CreateDirectory(assetPath);


        //获取当前列表元素数量.并生成唯一的文件名.

        //TODO 检查每个文件名,不重复才行
        GetCurrentViewPoz();



        var icon = TakeScreenShot();
        int index = sceneViewList.data.Count;
        string fileName = "Element" + (index + 1) + ".asset";
        //获取当前文件夹路径
        string fullPath = assetPath + "/" + fileName;

        newElement.names = "newElement" + (index + 1);
        newElement.icon = icon;
        newElement.Note = "Note";
        newElement.position = PivotPoint;
        newElement.rotation = currentCameraTf.rotation;
        newElement.Size = SceneSize;

        //获取当前SceneViewCamera的
        sceneViewList.data.Add(newElement);


        AssetDatabase.CreateAsset(newElement, fullPath);

        //只是添加到这个运行时脚本,并未保存
        //sceneViewList.Apply();
        //AssetDatabase.CreateAsset(sceneViewList, filePath);
        EditorUtility.SetDirty(sceneViewList);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /*    private void OnInspectorUpdate()
       {
           //去掉了定时保存的功能,改为手动
            SaveTime++;
            if (SaveTime > 150)
            {
                SaveTime = 0;
                SaveChange();
            }
            AssetDatabase.SaveAssets(); 

       }
       */
    private void OnHeaderCallback(Rect rect)
    {
        EditorGUI.LabelField(rect, "SceneNavList");
    }
    private void OnElementBackgroundCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        // GUI.color = Color.white;
    }
    private void OnElementCallback(Rect rect, int index, bool isActive, bool isFocused)
    {
        // 如果是被选中/拖拽的元素，设为绿色

        if (sceneViewList.data == null || index < 0 || index >= sceneViewList.data.Count)
            return;
        Texture2D Icon1 = EditorGUIUtility.FindTexture("TreeEditor.Trash");
        Texture2D Icon2 = EditorGUIUtility.FindTexture("ScaleTool");

        //检测是否为空
        var element = sceneViewList.data[index];
        if (element == null)
            return;

        sceneViewList.data[index].icon = (Texture2D)EditorGUI.ObjectField(new Rect(rect.x, rect.y, 150, 100), sceneViewList.data[index].icon, typeof(Texture2D), false);
        sceneViewList.data[index].names = EditorGUI.TextField(new Rect(rect.x + 230, rect.y, rect.width - 250, 20), sceneViewList.data[index].names);
        sceneViewList.data[index].Note = EditorGUI.TextArea(new Rect(rect.x + 160, rect.y + 40, rect.width - 150, 60), sceneViewList.data[index].Note);
        if (GUI.Button(new Rect(rect.x + 195, rect.y, 32, 32), Icon1))
        {

            // OnRemoveCallback(reorderableList);---这个方法,..会报错
            // 传递当前索引，而不是依赖list.index
            OnRemoveAtIndex(index);
            // OnRemoveCallback(reorderableList);
        }
        if (GUI.Button(new Rect(rect.x + 155, rect.y, 32, 32), Icon2))
        {
            FindScenePoz(index);
        }
        // EditorGUI.DrawPreviewTexture(new Rect(rect.x, rect.y, 100, 100), (Texture)sceneViewList.data[index].icon);

        //sceneViewList.data[index].hp = EditorGUI.IntSlider(new Rect(rect.x + 105, rect.y + 30, rect.width - 100, 20), "hp", sceneViewList.data[index].hp, 0, 100);
        // EditorGUI.PrefixLabel(new Rect(rect.x + 105, rect.y + 60, rect.width - 100, 20), new GUIContent("pos"));
        //  sceneViewList.data[index].position = EditorGUI.Vector3Field(new Rect(rect.x + 130, rect.y + 60, rect.width - 120, 20), "", sceneViewList.data[index].position);
        // 恢复默认颜色（避免影响其他UI）

    }
    private void OnGUI()
    {
        //让GUILayout.BeginScrollView根据当前位置滑动

        scrollPos = GUILayout.BeginScrollView(scrollPos, false, true);
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        Color bc = GUI.backgroundColor;
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Add NavPoint", GUILayout.Height(30)))
        {
            reorderableList.onAddCallback?.Invoke(reorderableList);
        }
        GUI.backgroundColor = bc;

        if (GUILayout.Button("Save Data", GUILayout.Height(30)))
        { SaveChange(); };
        // 设置为红色
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Clean All", GUILayout.Height(30)))
        {
            CleanAllData();
        };
        GUI.backgroundColor = bc;
        // GUI.color = originalColor;
        GUILayout.EndHorizontal();
        reorderableList.DoLayoutList();

        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
    void CleanAllData()
    {
        if (EditorUtility.DisplayDialog("是否移除", "是否移除", "是", "否"))
        {

            sceneViewList.data.Clear();
            //使用assetdatabase清除对应目录的文件
            //使用sceneViewList.data清除对应的文件
            if (!Directory.Exists(assetPath))
            {
                Debug.LogWarning("目录不存在：" + assetPath);
                return;
            }
            else
            {
                ClearDirectoryFiles(assetPath);

            }

        }
    }
    void ClearFiles(string fileName, string PngName)
    {
        try
        {
            //先读取文件的Icon名称

            AssetDatabase.DeleteAsset(fileName + ".asset");
            string pngPath = PngName + ".png";
            Debug.Log("pngPath：" + pngPath);
            if (File.Exists(pngPath))
            {
                File.Delete(pngPath);
                Debug.Log("删除图片文件：" + pngPath);
                // 同时删除.meta文件
                File.Delete(pngPath + ".meta");
            }

            Debug.Log("删除文件：" + fileName + ".asset");
            Debug.Log("删除文件：" + fileName + ".meta");
        }
        catch (System.Exception e)
        {
            Debug.LogError("清理目录时发生错误：" + e.Message);
        }
        AssetDatabase.Refresh();

    }

    /*  void ClearFiles(string baseFileName)
     {
         try
         {
             // 删除 .asset 文件
             string assetPath = baseFileName + ".asset";
             if (File.Exists(assetPath))
             {
                 AssetDatabase.DeleteAsset(assetPath);
                 Debug.Log("删除文件: " + assetPath);
             }

             // 删除 .png 文件（图标）
             string pngPath = baseFileName + ".png";
             if (File.Exists(pngPath))
             {
                 File.Delete(pngPath);
                 File.Delete(pngPath + ".meta"); // 同时删除 .meta 文件
                 Debug.Log("删除图片文件: " + pngPath);
             }

             AssetDatabase.Refresh();
         }
         catch (System.Exception e)
         {
             Debug.LogError("删除文件时出错: " + e.Message);
         }
     } */

    void ClearDirectoryFiles(string path)
    {
        try
        {
            // 获取目录中的文件和子目录
            string[] files = Directory.GetFiles(path);
            string[] dirs = Directory.GetDirectories(path);

            // 删除所有文件
            foreach (string file in files)
            {
                File.Delete(file);
                Debug.Log("删除文件：" + file);
            }

            // 递归删除所有子目录
            foreach (string dir in dirs)
            {
                ClearDirectoryFiles(dir);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("清理目录时发生错误：" + e.Message);
        }
        AssetDatabase.Refresh();
    }
    void GetCurrentViewPoz()
    {
        // SceneView.lastActiveSceneView.Repaint();
        SceneView sw = SceneView.lastActiveSceneView;
        //SceneView sw = SceneView.currentDrawingSceneView;
        if (sw.camera == null)
        {
            Debug.LogError("Unable to GetCurrentView, no camera attached to current scene view");
            return;
        }
        // 获取摄像机位置
        //必须是获取曲轴点,位置会有偏移
        PivotPoint = sw.pivot;
        currentCameraTf = sw.camera.transform;
        //设置SceneSize为sw的实际Size
        SceneSize = sw.cameraDistance * Mathf.Sin(sw.camera.fieldOfView * 0.5f * Mathf.Deg2Rad);

    }
    void FindScenePoz(int index)
    {
        var scene = SceneView.lastActiveSceneView;
        scene.pivot = sceneViewList.data[index].position;
        scene.rotation = sceneViewList.data[index].rotation;
        scene.size = sceneViewList.data[index].Size;
        scene.Repaint();

        //camera.transform.rotation = sceneViewList.data[index].rotation;
    }
    Texture2D TakeScreenShot()
    {
        //0.获取最后一个激活的场景视图
        //如果需要在帧最后截图,可以考虑携程
        SceneView sw = SceneView.lastActiveSceneView;

        //原始版本:RenderTexture Rt = new RenderTexture(256, 256, 0);
        //改动版3:2的比例
        int resolutionwidth = 300;
        int resolutionheight = 200;
        RenderTexture Rt = new RenderTexture(resolutionwidth, resolutionheight, 24, RenderTextureFormat.ARGB32);

        // 2.处理UI Canvas（如果有）
        var canvases = Object.FindObjectsOfType<Canvas>();
        List<Canvas> modifiedCanvases = new List<Canvas>();
        foreach (var canvas in canvases)
        {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                modifiedCanvases.Add(canvas);
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = sw.camera;
            }
        }

        //2.1.检测是否有场景视图
        if (sw == null)
        {
            Debug.LogError("Unable to capture editor screenshot, no scene view found");
            return null;
        }
        if (sw.camera == null)
        {
            Debug.LogError("Unable to capture editor screenshot, no camera attached to current scene view");
            return null;
        }
        // 3. 备份原始设置
        var prevRT = sw.camera.targetTexture;
        var prevClearFlags = sw.camera.clearFlags;
        sw.camera.clearFlags = CameraClearFlags.Skybox; // 确保背景清晰

        // 3.1.渲染截图
        sw.camera.targetTexture = Rt;
        sw.camera.Render();
        RenderTexture.active = Rt;
        Texture2D screenShot = new Texture2D(resolutionwidth, resolutionheight, TextureFormat.ARGB32, false);

        //4.1读取像素,屏幕左下脚为0点
        screenShot.ReadPixels(new Rect(0, 0, Rt.width, Rt.height), 0, 0);
        screenShot.Apply();
        //保存像素信息
        //截图模糊是因为比例

        // 5. 恢复设置
        foreach (var canvas in modifiedCanvases)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera = null;
        }
        //恢复之前的设置
        sw.camera.targetTexture = prevRT;
        sw.camera.clearFlags = prevClearFlags;


        //保存当前的screenshot为png文件
        byte[] bytes = screenShot.EncodeToPNG();//将纹理数据保存成一个Png数据
        string FIleName = assetPath + "/SceneView" + $"{System.DateTime.Now:yyyyMMdd_HHmmss_ffff}" + ".png";
        //string FIleName = assetPath + sceneViewList.data[index].names + ".png";


        File.WriteAllBytes(FIleName, bytes);

        // 强制刷新 AssetDatabase，确保 PNG 被识别为 Texture2D
        AssetDatabase.ImportAsset(FIleName, ImportAssetOptions.ForceUpdate);
        Texture2D loadedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(FIleName);
        //如果File写入失败报错
        Debug.Log("Screenshot written to file ");
        //清空缓存
        RenderTexture.active = null;
        DestroyImmediate(Rt);
        //释放内存
        SceneView.lastActiveSceneView.camera.targetTexture = null;
        AssetDatabase.Refresh();
        return loadedTexture;
    }

}