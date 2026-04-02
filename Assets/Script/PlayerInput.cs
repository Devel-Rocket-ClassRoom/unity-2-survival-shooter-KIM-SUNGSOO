using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static readonly string moveAxis = "Vertical";
    public static readonly string RotateAxis = "Horizontal";
    public static readonly string FireButton = "Fire1";
    public static readonly string ReloadButton = "Reload";

    public float Move { get; private set; }
    public float Rotate { get; private set; }
    public bool Fire { get; private set; }

    public bool Reload { get; private set; }

    private void Update()
    {
        Move = Input.GetAxis(moveAxis);
        Rotate = Input.GetAxis(RotateAxis);
        Fire = Input.GetButton(FireButton);
        Reload = Input.GetButton(ReloadButton);
    }
}
