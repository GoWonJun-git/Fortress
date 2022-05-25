using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

public class ParticleManager : MonoBehaviour
{
    public PhotonView PV;
    
    public GameManager gameManager;
    public CameraManager cameraManager;
    public GameObject particle_Up1;
    public GameObject particle_Up2;
    public GameObject particle_Wind;
    public GameObject particle_Light;

    // Particle 변경 Packet을 호출.
    public void ChangeParticlePosition(int wave)
    {
        int rand1 = UnityEngine.Random.Range((int)cameraManager.minX + 2, 0);
        int rand2 = UnityEngine.Random.Range(0, (int)cameraManager.maxX - 2);
        PV.RPC("ChangeParticlePosition_RPC", RpcTarget.All, rand1, rand2);
    }

    // Particle 변경 로직.
    [PunRPC]
    public void ChangeParticlePosition_RPC(int rand1, int rand2)
    {
        particle_Up1.transform.position = new Vector2(rand1, -5);
        particle_Up2.transform.position = new Vector2(rand2, -5);

        if (particle_Wind.transform.position.x >= 0)
            particle_Wind.transform.localPosition = new Vector3((float)particleSystemTransform.wind_MinX, particle_Wind.transform.localPosition.y, 0);
        else
            particle_Wind.transform.localPosition = new Vector3((float)particleSystemTransform.wind_ManX, particle_Wind.transform.localPosition.y, 0);

        Transform parent = particle_Wind.transform.parent;
        particle_Wind.transform.parent = null;
        particle_Wind.transform.localScale = new Vector3(particle_Wind.transform.lossyScale.x * -1, particle_Wind.transform.lossyScale.y, 0);
        particle_Wind.transform.parent = parent;

        if (particle_Light.transform.position.x >= 20)
            particle_Light.transform.localPosition = new Vector3((float)particleSystemTransform.light_MinX, particle_Light.transform.localPosition.y, 0);
        else
            particle_Light.transform.localPosition = new Vector3((float)particleSystemTransform.light_MaxX, particle_Light.transform.localPosition.y, 0);
    }

}
