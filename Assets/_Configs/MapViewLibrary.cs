using UnityEngine;

[CreateAssetMenu(menuName = "MapView/MapViewLibrary")]
public class MapViewLibrary : ScriptableObject
{
    public MapViewInfo[] modes;

    public MapViewInfo Get(int i) => modes[i];
}

[System.Serializable]
public struct MapViewInfo
{
    public string name;          
    public string label;         
    public Sprite bgActive;     
}
