using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VR SaveData", menuName = "VREditor/PaintOption")]
public class PaintOption : ScriptableObject
{
    public Color Color = Color.white;

    public bool paintRed = true;
    public bool paintGreen = true;
    public bool paintBlue = true;
    public bool paintAlpha = false;
}
