namespace MelodyFinder.Services;

public class MusicService
{
    private readonly Random _random = new();

    public string[] GetScale(string key)
    {
        var majorSteps = new[] { 2, 2, 1, 2, 2, 2, 1 };
        var minorSteps = new[] { 2, 1, 2, 2, 1, 2, 2 };
        var isMinor = key.EndsWith("m");
        var root = isMinor ? key[..^1] : key;

        var chromatic = new[] {
            "C", "^C", "D", "^D", "E", "F", "^F", "G", "^G", "A", "^A", "B"
        };

        int startIndex = Array.IndexOf(chromatic, root);
        if (startIndex == -1) return Array.Empty<string>();

        var steps = isMinor ? minorSteps : majorSteps;
        var scale = new List<string> { root };
        int idx = startIndex;

        foreach (var step in steps)
        {
            idx = (idx + step) % chromatic.Length;
            scale.Add(chromatic[idx]); // Remove accidental if present
        }

        return scale.ToArray();
    }

    public string GenerateMelody()
    {
        var keys = new[] { "C", "G", "D", "A", "F", "Am", "Em", "Dm" }; // Add more keys with the correct notation accidentals
        var key = keys[_random.Next(keys.Length)];
        var scale = GetScale(key);
        if (scale.Length == 0) return "";

        var abc = $"X:1\nT:Random Melody\nM:4/4\nK:{key}\n";
        int bars = 2;
        int notesPerBar = 4;
        string[] durations = { "2", "" }; 
        // string[] durations = { "", "/", "2" }; // quarter, eighth, half
        int currentNote = 0;

        for (int b = 0; b < bars; b++)
        {
            for (int n = 0; n < notesPerBar; n++)
            {
                int step = _random.Next(-3, 4);
                currentNote = Math.Clamp(currentNote + step, 0, scale.Length - 1);
                string note = scale[currentNote];
                string duration = durations[_random.Next(durations.Length)];
                abc += note + duration + " ";
            }
            // abc += "|";
        }
        abc += "||";

        return abc.Trim();
    }
}
