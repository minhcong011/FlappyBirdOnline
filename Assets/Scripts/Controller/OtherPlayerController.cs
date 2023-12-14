using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerController : Player
{
    public override void Start()
    {
        base.Start();
    }
    public override void Update()
    {
        UpdateTranform();
        ManageActive();
    }
    private void ManageActive()
    {
        if (data.isDelete) Destroy(gameObject);
    }
    private void UpdateTranform()
    {
        transform.position = new Vector2(data.x, data.y);
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, data.rotationZ);
    }
    public void SetData(PlayerData data, string name)
    {
        this.name = name;
        this.data = data;
        UIManager.Instance.CreatePlayerName(gameObject, data);
    } 
    public PlayerData GetData()
    {
        return data;
    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
    }
}
