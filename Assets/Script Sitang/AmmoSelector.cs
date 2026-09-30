using UnityEngine;

public class AmmoSelector : MonoBehaviour
{
    public SpriteRenderer selectedAmmo;

    public Color ammo1Color = Color.white;
    public Color ammo2Color = Color.red;

    public void SelectAmmo(int index)
    {
        if (index == 0)
        {
            selectedAmmo.color = ammo1Color;
        }
        else if (index == 1)
        {
            selectedAmmo.color = ammo2Color;
        }
    }
}