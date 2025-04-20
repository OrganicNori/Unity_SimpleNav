using UnityEngine;
using System;
using Unity.Mathematics;

[Serializable]

[CreateAssetMenu(fileName = "Element", menuName = "SceneNavElement", order = 0)]
public class Element : ScriptableObject
{
    public Texture2D icon;
    public string names;
    [TextArea(3, 5)]
    public string Note;
    //public int hp;
    public Vector3 position;
    public Quaternion rotation;

    //这里的size是size = cameraDistance * sin(fov / 2)
    public float Size;
}