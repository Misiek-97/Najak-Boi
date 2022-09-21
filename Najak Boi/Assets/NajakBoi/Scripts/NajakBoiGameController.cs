using System.Collections;
using NajakBoi.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class NajakBoiGameController : MonoBehaviour
{
    public GameObject najakPrefab;
    public GameObject najak2Prefab;
    public TerrainGenerator generator;
    public static GameObject Najak;
    public static GameObject Najak2;
    public static bool switchingNajaks;

    public static NajakBoiGameController Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Najak = GameObject.Find("NajakBoi");
        SmoothCamera.Target = Najak.transform;
        
        Najak2 = GameObject.Find("NajakBoi2");
    }
    
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RespawnNajak()
    {
        if(Najak)
            Destroy(Najak);

        var spawnPos = new Vector3(Random.Range(3, generator.terrainWidth - 3), generator.terrainHeight - 3, 0);
        Najak = Instantiate(najakPrefab, spawnPos, Quaternion.identity);
        Najak.name = "NajakBoi";
        SmoothCamera.Target = Najak.transform;
    }    
    
    public void RespawnNajak2()
    {
        if(Najak2)
            Destroy(Najak2);

        var spawnPos = new Vector3(Random.Range(3, 50 + generator.terrainWidth - 3), generator.terrainHeight - 3, 0);
        Najak2 = Instantiate(najak2Prefab, spawnPos, Quaternion.identity);
        Najak2.name = "NajakBoi2";
    }
    
    public void RegenerateTerrain()
    {
        generator.GenerateTerrain();
    }

    public void SwitchPlayers(Transform previous)
    {
        switchingNajaks = true;
        StartCoroutine(WaitSwitchPlayers(previous));
    }
    IEnumerator WaitSwitchPlayers(Transform previous)
    {
        
        yield return new WaitForSeconds(2.0f);
        SmoothCamera.Target = previous;
        yield return new WaitForSeconds(2.0f);
        
        var isNajak = SmoothCamera.Target == Najak.transform;
        var isNajak2 = SmoothCamera.Target == Najak2.transform;
        
        if (isNajak)
            SmoothCamera.Target = Najak2.transform;
        else if (isNajak2)
            SmoothCamera.Target = Najak.transform;
        else
        {
            Debug.Log(SmoothCamera.Target + " target is neither Najak nor Najak2");
        }

        switchingNajaks = false;
    }
}
