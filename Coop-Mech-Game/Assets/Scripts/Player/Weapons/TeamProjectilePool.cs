using System.Collections.Generic;
using UnityEngine;

public class TeamProjectilePool : MonoBehaviour
{
    /// <summary>
    /// 
    /// This script will live on the MatchGameManager
    /// 
    /// this is because the size of the pool will likely be dependent on what weapons are being 
    /// used by the players which will be determined in the lobby by what mechs they pick
    /// 
    /// we don't want to spawn in hundreds of laser projectiles if the players 
    /// in the match aren't using laser weapons
    /// 
    /// </summary>


    [SerializeField] private int numCanProj = 10;
    [SerializeField] private int numAutoProj = 100;
    [SerializeField] private int numLasProj = 100;

    //public List<CannonProjectile> cannonProj = new List<CannonProjectile>();
    //public List<AutoCannonProjectile> autoCannonProj = new List<AutoCannonProjectile>(); 
    //public List<LaserGunProjectile> lasGunProj = new List<LaserGunProjectile>(); 

    private void Start()
    {
        for(int i = 0; i < numCanProj; i++)
        {
            //instantiate below map

            //cannonProj.Add(CannonProjectile);
        }

        for (int i = 0; i < numAutoProj; i++)
        {
            //instantiate below map

            //autoCannonProj.Add(AutoCannonProjectile);
        }

        for (int i = 0; i < numCanProj; i++)
        {
            //instantiate below map

            //lasGunProj.Add(LaserGunProjectile);
        }
    }

}
