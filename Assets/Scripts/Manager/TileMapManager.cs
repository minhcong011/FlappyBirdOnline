using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapManager : SingletonBase<TileMapManager>
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject ground;
    [SerializeField] private GameObject pipeBonus;
    [SerializeField] private List<GameObject> holder = new();


    private int posSpawnGround;
    private int posSpawnPipeBonus = 140;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpawnPipeBonus();
        SpawnGround();
    }
    private void SpawnGround()
    {
        if(player.transform.position.x > posSpawnGround)
        {
            holder.Add(Instantiate(ground, new Vector2(posSpawnGround + 12, -4), Quaternion.identity));
            posSpawnGround += 12;
        }
    }
    private void SpawnPipeBonus()
    {
        if (player.transform.position.x > posSpawnPipeBonus)
        {
            posSpawnPipeBonus += 110;
            holder.Add(Instantiate(pipeBonus, new Vector2(posSpawnPipeBonus, 0), Quaternion.identity));
        }
    }
    public void ClearHolder()
    {
        posSpawnGround = 0;
        posSpawnPipeBonus = 140;
        foreach (GameObject obj in holder)
        {
            Destroy(obj);
        }
        holder.Clear();
    }
}
