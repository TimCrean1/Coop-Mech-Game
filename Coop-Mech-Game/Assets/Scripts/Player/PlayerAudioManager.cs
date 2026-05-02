using Unity.VisualScripting;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [Header("Movement Audio")]
    [SerializeField] private AudioSource moveSource;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip dashSound;
    [Header("Combat Audio")]
    [SerializeField] private AudioSource p1GunSource;
    [SerializeField] private AudioSource p2GunSource;
    [SerializeField] private AudioSource p1UtilitySource;
    [SerializeField] private AudioSource p2UtilitySource;
    [Header("Idle Audio")]
    [SerializeField] private AudioSource engineSource;
    public AudioSource turningSource;

    void Start()
    {
        turningSource.loop = true;
        turningSource.Play();
        turningSource.volume = 0f;
    }

    #region Movement Audio

    public void PlayJumpSound()
    {
        moveSource.PlayOneShot(jumpSound);
    }

    public void PlayLandSound()
    {
        moveSource.PlayOneShot(landSound);
    }

    public void PlayWalkSound()
    {
        moveSource.PlayOneShot(walkSound);
    }

    public void PlayDashSound()
    {
        moveSource.PlayOneShot(dashSound);
    }

    #endregion
    #region Combat Audio

    public void SetP1GunClip(AudioClip clip)
    {
        p1GunSource.clip = clip;
    }

    public void PlayP1GunSound()
    {
        p1GunSource.PlayOneShot(p1GunSource.clip);
    }

    public void SetP2GunClip(AudioClip clip)
    {
        p2GunSource.clip = clip;
    }

    public void PlayP2GunSound()
    {
        p2GunSource.PlayOneShot(p2GunSource.clip);
    }

    public void SetP1UtilityClip(AudioClip clip)
    {
        p1UtilitySource.clip = clip;
    }

    public void PlayP1UtilitySound()
    {
        p1UtilitySource.PlayOneShot(p1UtilitySource.clip);
    }

    public void SetP2UtilityClip(AudioClip clip)
    {
        p2UtilitySource.clip = clip;
    }

    public void PlayP2UtilitySound()
    {
        p2UtilitySource.PlayOneShot(p2UtilitySource.clip);
    }
    #endregion
}
