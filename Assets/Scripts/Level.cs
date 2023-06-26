using System.Collections.Generic;
[System.Serializable]
public class Level
{
    public List<int> level_data;
    public int Count {
        get {
            return level_data.Count;
        }
    }
    
    public Level() 
    { 
        level_data = new List<int>();
    }


    public override string ToString() {
        string line = "";
        foreach (var c in level_data) {
            line += c + " ";
        }
        return line;
    }
}
