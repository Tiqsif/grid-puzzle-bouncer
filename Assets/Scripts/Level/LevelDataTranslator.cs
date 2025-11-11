using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataTranslator : MonoBehaviour
{
    private void Start()
    {
        string outputPath = "Assets/LevelSO/Translations/";

        Dictionary<string, Type> cellTypeMapping = new Dictionary<string, Type>
        {
            { "P", Type.Player },
            { "E", Type.Enemy },
            { "R", Type.Rock },
            { "M", Type.Mushroom },
            { "W", Type.Water },
            { "L", Type.LilyPad },
            { "F", Type.Flower },
            { "TM", Type.TiltedMushroom },
            { "TF", Type.TiltedFlower },
            { "MR", Type.MovingRock },
            { "MF", Type.MushroomFrog },
            { "FF", Type.FlowerFrog },
            { "SF", Type.Sunflower },
            { "T", Type.Turtoise },
            { "MP", Type.MushroomPad },
            { "FP", Type.FlowerPad }
        };

        List<string> rooms = new List<string> {
        // Level 1
        ". . . .\n" +
        ". M . E\n" +
        "P . . .\n" +
        ". . . ."
        ,
        // Level 2
        ". M M .\n" +
        "M . . M\n" +
        "P M . E\n" +
        "M . M ."
        ,
        // Level 3
        ". . . .\n" +
        ". P E M\n" +
        "M M . M\n" +
        ". E E ."
        ,
        // Level 4
        "R . .\n" +
        "P M E\n" +
        ". . R" 
        ,
        // Level 5
        "R . . .\n" +
        "E M E M\n" +
        "M . R .\n" +
        "P . R ."
        ,
        // Level 6
        ". . . . R\n" +
        "R M . E E\n" +
        "M . R E .\n" +
        "P E M M R\n" +
        "R E M . ."
        ,
        // Level 7
        ". . R M\n" +
        "M . E M\n" +
        "E M . .\n" +
        "P M R ."
        ,
        // Level 8
        ". E . R\n" +
        ". . R E\n" +
        ". F F .\n" +
        "M P . M"
        ,
        // Level 9
        ". . R .\n" +
        ". E M E\n" +
        ". F . .\n" +
        "R P F R"
        ,
        // Level 10
        ". E R .\n" +
        ". M E E\n" +
        "F F . M\n" +
        "P F R ."
        ,
        // Level 11
        "P R . E R\n" +
        ". E W F .\n" +
        ". F F M E\n" +
        ". . . F .\n" +
        ". F R . ."
        ,
        // Level 12
        ". . . M\n" +
        ". E E R\n" +
        "W W E M\n" +
        "P F R E\n" +
        ". . . ."
        ,
        // Level 13
        ". R E .\n" +
        "E M F .\n" +
        "L E E .\n" +
        "P L . ."
        ,
        // Level 14
            
        ". E M R\n" +
        ". F W .\n" +
        ". L F .\n" +
        "R P . ."
        ,
        // Level 15
        ". . . .\n" +
        ". P T R\n" +
        ". . M .\n" +
        ". . . ."


        /*
        ,
        // Level - all units delete later
        "R M W L\n" +
        "F TM TF .\n" +
        "MF FF SF T\n" +
        ". MR P E"
        */
        };

        foreach (string room in rooms)
        {
            string[] lines = room.Split('\n'); // split by new line
            // space's are not counted for width
            int width = lines[0].Length; // this includes spaces
            int height = lines.Length;
            int strippedWidth = 0;
            List<SpawnData> spawnDataList = new List<SpawnData>();
            for (int y = 0; y < height; y++)
            {
                string line = lines[y];
                string[] cells = line.Split(' '); // split by space
                strippedWidth = cells.Length;
                for (int x = 0; x < cells.Length; x++)
                {
                    string cell = cells[x];
                    if (cell == ".")
                    {
                        continue;
                    }
                    
                    // cell position 0,0 is bottom left
                    Vector2Int cellPosition = new Vector2Int(x, height - y - 1);
                    bool randomRotation = true;
                    Type type = Type.Empty;
                    if (cellTypeMapping.TryGetValue(cell, out Type unitType))
                    {
                        type = unitType;
                    }
                    else
                    {
                        Debug.LogError("Unknown cell type: " + cell);
                        Debug.LogError("Room: " + rooms.IndexOf(room));
                    }
                    SpawnData spawnData = new SpawnData(type, cellPosition, randomRotation, 0f);
                    spawnDataList.Add(spawnData);
                }
            }
            LevelSO levelSO = ScriptableObject.CreateInstance<LevelSO>();
            levelSO.cellSize = 1f;
            levelSO.gridSize = new Vector2Int(strippedWidth, height);
            levelSO.spawnDataList = spawnDataList;
            UnityEditor.AssetDatabase.CreateAsset(levelSO, outputPath + "Level_" + (rooms.IndexOf(room)+1)  + ".asset");
        }
    }
}
