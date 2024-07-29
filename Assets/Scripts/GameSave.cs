using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class GameSave
{
    const string keySave = "save_v0.1";

    public static void Save()
    {
        var data = new Data(MapCtrl.current.Level-1, CombatArena.current.player.health.Value);
        var shapes = new List<string>();
        foreach (var item in DataManager.current.shapes)
        {
            var shape = new List<string>();

            foreach (var block in item.GetBlocks())
            {
                shape.Add($"{block.pos.x},{block.pos.y},{block.data.id}");
            }

            shapes.Add(string.Join(':', shape));
        }

        data.shapes = string.Join('|', shapes);

        var json = JsonUtility.ToJson(data);
        //Debug.Log(json);
        PlayerPrefs.SetString(keySave, json);
        PlayerPrefs.Save();
    }

    public static void Clear()
    {
        PlayerPrefs.DeleteKey(keySave);
    }

    public static bool HasSave() => PlayerPrefs.HasKey(keySave);

    public static void Load()
    {
        if (!HasSave()) return;

        var json = PlayerPrefs.GetString(keySave);
        var data = JsonUtility.FromJson<Data>(json);

        var dataCtrl = DataManager.current;
        var gameData = dataCtrl.gameData;
        MapCtrl.current.Level = data.level;
        CombatArena.current.startingPlayerHealth = data.playerHp;
        var dict = gameData.blocks.ToDictionary(x=>x.id);
        var shapes = new List<BtShapeData>();
        foreach (var item in data.shapes.Split('|'))
        {
            var blocks = new List<BtBlockInfo>(); 
            foreach (var block in item.Split(':'))
            {
                BtBlockData bData;
                var tokensStr = block.Split(',');
                if (tokensStr.Length != 3)
                {
                    Debug.LogError($"Could not load save data (length err): \n{json}");
                    return;
                }

                var tokens = new List<int>();
                foreach (var s in tokensStr)
                {
                    if (int.TryParse(s, out var n))
                    {
                        tokens.Add(n);
                    }
                    else
                    {
                        Debug.LogError($"Could not load save data (non int): \n{json}");
                        return;
                    }
                }

                if (!dict.TryGetValue(tokens[2], out bData))
                {
                    bData = gameData.emptyBlock;
                }

                blocks.Add(new BtBlockInfo(bData, new Vector2Int(tokens[0], tokens[1])));
            }

            shapes.Add(new BtShapeData(blocks));
        }

        DataManager.current.shapes = shapes;
    }


    [System.Serializable]
    public class Data
    {
        public int level;
        public int playerHp;
        public string shapes;

        public Data(int level, int playerHp)
        {
            this.level = level;
            shapes = "";
            this.playerHp = playerHp;
        }
    }
}