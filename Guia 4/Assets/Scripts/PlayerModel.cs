using System;
using System.Collections.Generic;

[Serializable]
public class Player
{
    public string uid;          // PK (Auth UID)
    public string nickname;
    public int level;
    public int xp;
    public int coins;

    public List<InventoryItem> inventory = new();
    public Player()
    {
    }
    public Player(string uid, string nickname)
    {
        this.uid = uid;
        this.nickname = nickname;
        this.level = 1;
        this.xp = 0;
        this.coins = 0;
        this.inventory = new List<InventoryItem>();
    }
}

[Serializable]
public class InventoryItem
{
    public string itemId;       // PK compuesta: (uid,itemId) en relación ER
    public int quantity;
}
