using Chess;
using UnityEngine;

namespace ChessUI
{
    public enum MoveSfxType
    {
        Default,
        Capture,
        Check
    }

    public class MoveAudioPlayer : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip moveClip;
        [SerializeField] private AudioClip captureClip;
        [SerializeField] private AudioClip checkClip;

        public void Play(MoveSfxType type)
        {
            AudioClip clip = type switch
            {
                MoveSfxType.Check => checkClip,
                MoveSfxType.Capture => captureClip,
                _ => moveClip
            };

            if (clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}

