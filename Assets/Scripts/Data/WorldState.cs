using UnityEngine;

[CreateAssetMenu(fileName = "WorldState", menuName = "Game/WorldState")]
public class WorldState : ScriptableObject
{
    [Header("Progreso de Historia")]
    [SerializeField] private bool _zoneCheck; // Hab2 -> Hab3
    [SerializeField] private bool _backFromHab3; // Hab3 -> Hab1
    [SerializeField] private bool _hasKey; // Hab4 -> Hab5
    [SerializeField] private bool _msjRead; // Hab5 -> Final

    [Header("Sistema")]
    [SerializeField] private string _currentRoomName; 

    
    public bool zoneCheck => _zoneCheck;
    public bool backFromHab3 => _backFromHab3;
    public bool hasKey => _hasKey;
    public bool msjRead => _msjRead;
    public string currentRoomName => _currentRoomName;

    // setters
    public void SetZoneCheck(bool state) => _zoneCheck = state;
    public void SetBackFromHab3(bool state) => _backFromHab3 = state;
    public void SetHasKey(bool state) => _hasKey = state;
    public void SetMsjRead(bool state) => _msjRead = state;
    public void SetCurrentRoomName(string name) => _currentRoomName = name;

    public void ResetState()
    {
        _zoneCheck = false;
        _backFromHab3 = false;
        _hasKey = false;
        _msjRead = false;
        _currentRoomName = "Hab1";
    }
}