using UnityEngine;

public class Player_Input : MonoBehaviour
{

    //Mouse
    public bool mousePositionInput
    { get { return Input.GetKey(KeyCode.Mouse1); } }

    public bool mouseInteractInput
    { get { return Input.GetKeyDown(KeyCode.Mouse0); } }

    //Skills
    public bool qskill
    { get { return Input.GetKeyDown(KeyCode.Q); } }

    public bool wskill
    { get { return Input.GetKeyDown(KeyCode.W); } }

    public bool eskill
    { get { return Input.GetKeyDown(KeyCode.E); } }

    public bool rskill
    { get { return Input.GetKeyDown(KeyCode.R); } }

    //UI
    public bool inventoryButtonPressed
    { get { return Input.GetKeyDown(KeyCode.I); } }

    public bool pauseButtonPressed
    { get { return Input.GetKeyDown(KeyCode.Escape); } }

    public bool scrollButtonPressed
    { get { return Input.GetKeyDown(KeyCode.O); } }

}
