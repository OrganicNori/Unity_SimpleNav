using System.Collections.Generic;
using UnityEngine;
using System;



[Serializable]
[CreateAssetMenu(fileName = "SceneNav", menuName = "SceneNavList", order = 0)]
public class ElementList : ScriptableObject
{
    public List<Element> data;
}