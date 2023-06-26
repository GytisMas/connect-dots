using System.Collections.Generic;
[System.Serializable]
public class Levels
{
    public List<Level> levels;
    public int Count {
        get {
            return levels.Count;
        }
    }
    
    public Levels() {

    }
}
