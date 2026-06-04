using Unity.VisualScripting;
using Unity.Netcode;
using UnityEngine;

public class PlayerAudioManager : NetworkBehaviour
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

    [Rpc(SendTo.ClientsAndHost)]
    public void PlayWalkSoundClientRpc()
    {
        if (NetworkManager.Singleton == null) {return;}
        moveSource.PlayOneShot(walkSound);
        Debug.Log("Playing Walk Sound");
    }
    
    [Rpc(SendTo.ClientsAndHost)]
    public void PlayDashSoundClientRpc()
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
    #endregion
}
