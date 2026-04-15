using UnityEngine;

[CreateAssetMenu(fileName = "WorldState", menuName = "Game/WorldState")]
public class WorldState : ScriptableObject
{
    public bool noteWall;
    public bool masterKey;
    public int roomsExplored;
    public string currentRoomName; // El Manager leerá esto

    public void ResetState()
    {
        noteWall = false;
        masterKey = false;
        roomsExplored = 0;
        currentRoomName = "";
    }
}