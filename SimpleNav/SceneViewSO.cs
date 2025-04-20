using UnityEngine;


[CreateAssetMenu(fileName = "SceneViewSO", menuName = "SceneViewSO", order = 0)]
public class SceneViewSO : ScriptableObject
{


    public Texture SceneView;
    public string Name;
    public string Details;
    public Vector3 Position;
    public Vector3 Rotation;
    public string SceneViewPath;
    public void SetSceneView(Texture sceneView, string name, string details, Vector3 position, Vector3 rotation, string sceneViewPath)
    {

    }


}

