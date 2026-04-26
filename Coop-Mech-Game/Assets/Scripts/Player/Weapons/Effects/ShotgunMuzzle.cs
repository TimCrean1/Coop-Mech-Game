using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class ShotgunMuzzle : WeaponMuzzle
{
    private GraphicsBuffer _hitsBuffer;
    private GraphicsBuffer _normsBuffer;
    private List<Vector3> _hitPos = new List<Vector3>();

    public override void SendFireEvent(RaycastHit hit)
    {
    }

    private void Start()
    {
        _hitsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 8, Marshal.SizeOf<Vector3>());
        bulletEffect.SetGraphicsBuffer("EndPositions", _hitsBuffer);

        //Debug.LogWarning("does shotgun flash have graphics buffer property?: " +bulletEffect.HasGraphicsBuffer("EndPositions"));
        //Debug.LogWarning("does shotgun flash have end pos count property?: " + bulletEffect.HasInt("EndPositionsCount"));
        //Debug.LogWarning("does shotgun flash have start position property?: " + bulletEffect.HasVector3("StartPosition"));
    }
    public override void SendFireEventList(List<RaycastHit> inputHits)
    {
        if (bulletEffect)
        {
            //Debug.Log("Received fire event list in shotgun muzzle, hit count: " +  inputHits.Count);
            _hitPos.Clear();

            foreach (RaycastHit hit in inputHits)
            {
                _hitPos.Add(hit.point);
                //Debug.Log("adding hit pos: " + hit.point);
            }
            
            //Debug.Log("end positions count (should be same as hit count): "+_hitPos.Count);
            bulletEffect.SetInt("EndPositionsCount", _hitPos.Count);

            //for(int i = 0;  i < _hitPos.Count; i++)
            //{
            //    Debug.DrawLine(transform.position, _hitPos[i], Color.chartreuse, 15f);
            //}

            _hitsBuffer.SetData(_hitPos);
            bulletEffect.SetVector3("StartPosition", transform.position);

            bulletEffect.SendEvent("OnFire");
        }
    }

    private void OnDestroy()
    {
        _hitsBuffer?.Release();
    }
}
