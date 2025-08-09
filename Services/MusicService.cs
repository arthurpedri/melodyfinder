namespace MelodyFinder.Services;

public class MusicService
{
    private readonly Random _random = new();

    public string[] GetScale(string key)
    {
        var majorSteps = new[] { 2, 2, 1, 2, 2, 2, 1 };
        var minorSteps = new[] { 2, 1, 2, 2, 1, 2, 2 };
        var isMinor = key.EndsWith('m');
        var isFlat = key.StartsWith('_');
        var root = isMinor ? key[..^1] : key;


        var chromaticSharp = new[] {
            "C", "^C", "D", "^D", "E", "F", "^F", "G", "^G", "A", "^A", "B"
        };

        var chromaticFlat = new[] {
            "C", "_D", "D", "_E", "E", "F", "_G", "G", "_A", "A", "_B", "B"
        };

        var chromatic = isFlat ? chromaticFlat : chromaticSharp;

        int startIndex = Array.IndexOf(chromatic, root);
        if (startIndex == -1) return Array.Empty<string>();

        var steps = isMinor ? minorSteps : majorSteps;

        int idx = startIndex;

        var scale = new List<string> { root.StartsWith('_') || root.StartsWith('^') ? root[1..] : root };

        foreach (var step in steps)
        {
            idx = (idx + step) % chromatic.Length;
            if (chromatic[idx].StartsWith('_') || chromatic[idx].StartsWith('^'))
                scale.Add(chromatic[idx][1..]); // Remove the accidental symbol
            else
                scale.Add(chromatic[idx]);
        }

        return scale.ToArray();
    }

    public string GenerateMelody()
    {
        // The keys of Dbm, Gbm, Cbm, Abm are not supported in abc notation
        var keys = new[] { "C", "Am", "G", "Em", "D", "Bm", "A", "E", "B", "F", "Dm", "_B", "Gm", "_E", "Cm", "_A", "Fm", "_D", "_Bm" }; // Add more keys with the correct notation accidentals
        var key = keys[_random.Next(keys.Length)];
        var scale = GetScale(key);
        if (scale.Length == 0) return "";
        bool isMinor = key.EndsWith('m');
        key = isMinor ? key[..^1] : key; // Remove the 'm' for minor keys

        if (key.StartsWith('_'))
        {
            key = key[1..] + "b"; // Convert to flat notation
        }
        else if (key.StartsWith('^'))
        {
            key = key[1..] + "#"; // Convert to sharp notation
        }
        key = isMinor ? key + "m" : key; // Append 'm' for minor keys

        var abc = $"X:1\nQ:100\nT:Random Melody\nM:4/4\nK:{key}\n";
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

    public string GenerateRandomNote()
    {
        var notes = new[] {
            "C,", "^C,", "D,", "^D,", "E,", "F,", "^F,", "G,", "^G,", "A,", "^A,", "B,",
            "C", "^C", "D", "^D", "E", "F", "^F", "G", "^G", "A", "^A", "B",
            "c", "^c", "d", "^d", "e", "f", "^f", "g", "^g", "a", "^a", "b"
        };
        var note = notes[_random.Next(notes.Length)];
        return note;
    }
    

}
