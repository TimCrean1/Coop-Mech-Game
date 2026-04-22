using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using static Unity.Cinemachine.IInputAxisOwner.AxisDescriptor;

public class ShotgunMuzzle : WeaponMuzzle
{
    private GraphicsBuffer _gBuffer;
    private List<Vector3> _hitPos = new List<Vector3>();

    public override void SendFireEvent(RaycastHit hit)
    {
    }

    private void Start()
    {
        _gBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, 8, Marshal.SizeOf<Vector3>());
        bulletEffect.SetGraphicsBuffer("EndPositions", _gBuffer);

        Debug.LogWarning("does shotgun flash have graphics buffer property?: " +bulletEffect.HasGraphicsBuffer("EndPositions"));
        Debug.LogWarning("does shotgun flash have end pos count property?: " + bulletEffect.HasInt("EndPositionsCount"));
        Debug.LogWarning("does shotgun flash have start position property?: " + bulletEffect.HasVector3("StartPosition"));
    }
    public override void SendFireEventList(List<RaycastHit> hitList)
    {
        if (bulletEffect)
        {
            Debug.Log("Received fire event list in shotgun muzzle, hit count: " +  hitList.Count);
            _hitPos.Clear();

            foreach (RaycastHit hit in hitList)
            {
                _hitPos.Add(hit.point);
                Debug.Log("adding hit pos: " + hit.point);
            }
            
            Debug.Log("end positions count (should be same as hit count): "+_hitPos.Count);
            bulletEffect.SetInt("EndPositionsCount", _hitPos.Count);


            for (int i = _hitPos.Count; i < 8; i++)
            {
                _hitPos.Add(Vector3.zero);
                Debug.Log("adding zero vector: " + _hitPos[i]);
            }
            _gBuffer.SetData(_hitPos);
            bulletEffect.SetVector3("StartPosition", transform.position);

            bulletEffect.SendEvent("OnFire");
        }
    }

    private void OnDestroy()
    {
        _gBuffer?.Release();
    }
}
