using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ModeInfo
{
    public string name;
    public Sprite iconActive;
    public Sprite iconInactive;
    public Sprite bgActive;
}

[CreateAssetMenu(fileName = "ModeLibrary", menuName = "UI/Mode Library", order = 1)]
public class ModeLibrary : ScriptableObject
{
    public List<ModeInfo> modes = new List<ModeInfo>();
    public ModeInfo Get(int index) => modes[index];
}
