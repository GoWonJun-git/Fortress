using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashPlayer : MonoBehaviour
{
    public GameObject PhotonObject;

    void Start() => DontDestroyOnLoad(PhotonObject);
    
}