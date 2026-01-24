using System.Collections.Generic;
using UnityEngine;

public class TeamProjectilePool : MonoBehaviour
{
    /// <summary>
    /// 
    /// This script will live on the Player prefab, since there is only one per team
    /// 
    /// this is because the size of the pool will likely be dependent on what weapons are being used by the players
    /// 
    /// we don't want to spawn in hundreds of laser projectiles if the players 
    /// in the match aren't using laser weapons
    /// 
    /// </summary>

    [SerializeField] private Vector3 ProjectileSpawnPos = new Vector3(0f, -10f, 0f);

    [SerializeField] private int numCanProj = 10;
    [SerializeField] private int numAutoProj = 100;
    [SerializeField] private int numLasProj = 100;
    [SerializeField] private int numFX = 50;

    [SerializeField] private CannonProjectile cannonProj;
    //[SerializeField] private BaseProjectile mgProj;
    //[SerializeField] private BaseProjectile lasProj;
    [SerializeField] private BaseEffect baseEffect;

    public List<CannonProjectile> cannonProjectilesList = new List<CannonProjectile>();
    //public ListMGProjectile> MGProj = new List<MGProjectile>(); 
    //public List<LaserProjectile> lasProj = new List<LaserProjectile>(); 
    public List<BaseEffect> effectsList = new List<BaseEffect>();

    private int cannonIndex = 0;
    private int mgIndex = 0;
    private int lasIndex = 0;
    private int effectIndex = 0;
    private BaseProjectile projToRet;


    private void Start()
    {
        for(int i = 0; i < numCanProj; i++)
        {
            //instantiate below map
            CannonProjectile cann;

            cann = Instantiate(cannonProj, ProjectileSpawnPos, Quaternion.identity);

            cannonProjectilesList.Add(cann);
            cannonProjectilesList[i].gameObject.SetActive(false);
            //cann.gameObject.SetActive(false);
        }

        for (int i = 0; i < numAutoProj; i++)
        {
            //instantiate below map
            //MGProjectile mg = Instantiate(mgProj, ProjectileSpawnPos, Quaternion.identity);

            //cannonProjectilesList.Add(mg);
            //mg.enabled = false;
        }

        for (int i = 0; i < numLasProj; i++)
        {
            //instantiate below map
            //LaserProjectile las = Instantiate(lasProj, ProjectileSpawnPos, Quaternion.identity);

            //cannonProjectilesList.Add(las);
            //las.enabled = false;
        }

        for(int i = 0; i < numFX; i++)
        {
            //instantiate below map
            //BaseEffect ef = Instantiate(baseEffect, ProjectileSpawnPos, Quaternion.identity);

            //effectsList.Add(ef);
            //ef.enabled = false;
        }
    }

    public BaseProjectile GetNextProjectile(BaseWeapon weaponType)
    {
        switch (weaponType)
        {
            case WeaponCannon:
                projToRet = cannonProjectilesList[cannonIndex];
                cannonIndex = (cannonIndex + 1) % cannonProjectilesList.Count;
                return projToRet;
            default:
                return null;
        }
    }

    public BaseEffect GetNextEffect()
    {
        BaseEffect effect = effectsList[effectIndex];
        effectIndex = (effectIndex + 1) % effectsList.Count;
        return effect;
    }

}
