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
    [SerializeField] private AudioSource damageSource;
    [SerializeField] private AudioClip damageClip;
    // [SerializeField] public AudioSource p1GunSource;
    // [SerializeField] private AudioClip p1GunClip;
    // [SerializeField] public AudioSource p2GunSource;
    // [SerializeField] private AudioClip p2GunClip;
    // [SerializeField] public AudioSource p1UtilitySource;
    // [SerializeField] private AudioClip p1UtilityClip;
    // [SerializeField] public AudioSource p2UtilitySource;
    // [SerializeField] private AudioClip p2UtilityClip;
    [Header("Idle Audio")]
    [SerializeField] private AudioSource engineSource;
    public AudioSource turningSource;
    [SerializeField] private AudioClip engineClip;
    [SerializeField] private AudioClip turningClip;

    void Start()
    {
        turningSource.loop = true;
        turningSource.volume = 0f;

        engineSource.clip = engineClip;
        turningSource.clip = turningClip;

        turningSource.Play();
        engineSource.Play();
    }

    #region Movement Audio

    public void PlayJumpSound()
    {
        moveSource.PlayOneShot(jumpSound);
        Debug.Log("Playing Jump Sound");
    }

    public void PlayLandSound()
    {
        moveSource.PlayOneShot(landSound);
        Debug.Log("Playing Land Sound");
    }

    public void PlayWalkSound()
    {
        moveSource.PlayOneShot(walkSound);
        Debug.Log("Playing Walk Sound");
    }

    public void PlayDashSound()
    {
        moveSource.PlayOneShot(dashSound);
        Debug.Log("Playing Dash Sound");
    }

    #endregion
    #region Combat Audio

    public void PlayDamageClip()
    {
        damageSource.PlayOneShot(damageClip);
    }

    // public void SetP1GunClip(AudioClip clip)
    // {
    //     p1GunSource.clip = clip;
    // }

    // public void PlayP1GunSound()
    // {
    //     p1GunSource.PlayOneShot(p1GunSource.clip);
    //     Debug.Log("Playing P1 Gun Sound");
    // }

    // public void SetP2GunClip(AudioClip clip)
    // {
    //     p2GunSource.clip = clip;
    // }

    // public void PlayP2GunSound()
    // {
    //     p2GunSource.PlayOneShot(p2GunSource.clip);
    //     Debug.Log("Playing P2 Gun Sound");
    // }

    // public void SetP1UtilityClip(AudioClip clip)
    // {
    //     p1UtilitySource.clip = clip;
    // }

    // public void PlayP1UtilitySound()
    // {
    //     p1UtilitySource.PlayOneShot(p1UtilitySource.clip);
    //     Debug.Log("Playing P1 Utility Sound");
    // }

    // public void SetP2UtilityClip(AudioClip clip)
    // {
    //     p2UtilitySource.clip = clip;
    // }

    // public void PlayP2UtilitySound()
    // {
    //     p2UtilitySource.PlayOneShot(p2UtilitySource.clip);
    //     Debug.Log("Playing P2 Utility Sound");
    // }
    #endregion
}
