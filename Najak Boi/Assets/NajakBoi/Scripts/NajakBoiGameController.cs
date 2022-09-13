using System;
using System.Collections;
using System.Collections.Generic;
using NajakBoi.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NajakBoiGameController : MonoBehaviour
{
    public GameObject deutPrefab;
    public GameObject deut2Prefab;
    public TerrainGenerator generator;
    public static GameObject Deut;
    public static GameObject Deut2;
    public static bool switchingDeuts;

    public static NajakBoiGameController Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Deut = GameObject.Find("Deut");
        SmoothCamera.Target = Deut.transform;
        
        Deut2 = GameObject.Find("Deut2");
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RespawnDeut()
    {
        if(Deut)
            Destroy(Deut);

        var spawnPos = new Vector3(Random.Range(3, generator.terrainWidth - 3), generator.terrainHeight - 3, 0);
        Deut = Instantiate(deutPrefab, spawnPos, Quaternion.identity);
        Deut.name = "Deut";
        SmoothCamera.Target = Deut.transform;
    }    
    
    public void RespawnDeut2()
    {
        if(Deut2)
            Destroy(Deut2);

        var spawnPos = new Vector3(Random.Range(3, 50 + generator.terrainWidth - 3), generator.terrainHeight - 3, 0);
        Deut2 = Instantiate(deut2Prefab, spawnPos, Quaternion.identity);
        Deut2.name = "Deut2";
    }
    
    public void RegenerateTerrain()
    {
        generator.GenerateTerrain();
    }

    public void SwitchPlayers(Transform previous)
    {
        switchingDeuts = true;
        StartCoroutine(WaitSwitchPlayers(previous));
    }
    IEnumerator WaitSwitchPlayers(Transform previous)
    {
        
        yield return new WaitForSeconds(2.0f);
        SmoothCamera.Target = previous;
        yield return new WaitForSeconds(2.0f);
        
        var isDeut = SmoothCamera.Target == Deut.transform;
        var isDeut2 = SmoothCamera.Target == Deut2.transform;
        
        if (isDeut)
            SmoothCamera.Target = Deut2.transform;
        else if (isDeut2)
            SmoothCamera.Target = Deut.transform;
        else
        {
            Debug.Log(SmoothCamera.Target + " target is neither Deut nor Deut2");
        }

        switchingDeuts = false;
    }
}
