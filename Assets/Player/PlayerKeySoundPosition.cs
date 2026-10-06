using UnityEngine;

public class PlayerKeySoundPosition : MonoBehaviour
{
    public KeyCode triggerKey = KeyCode.Q;
    public AudioClip soundClip;
    [Range(0f, 1f)]
    public float volume = 1f;

    void Update()
    {
        if (Input.GetKeyDown(triggerKey))
        {
            if (soundClip != null)
            {
                // เล่นเสียง ณ ตำแหน่งปัจจุบันของตัวละคร
                AudioSource.PlayClipAtPoint(soundClip, transform.position, volume);
            }
        }
    }
}