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
        abc += "|";

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

    public (string, string, int, (string, string)) GenerateRandomChord()
    {
        // var roots = new[] { "C", "D", "E", "F", "G", "A", "B", "_D", "_E", "_G", "_A", "_B", "^C", "^D", "^F", "^G", "^A" };
        // var qualities = new[] { "", "m", "7", "m7", "maj7", "dim", "aug" };
        var majorChordSteps = new[] { 0, 4, 3 };
        var minorChordSteps = new[] { 0, 3, 4 };
        var dominant7ChordSteps = new[] { 0, 4, 3, 3 };
        var minor7ChordSteps = new[] { 0, 3, 4, 3 };
        var major7ChordSteps = new[] { 0, 4, 3, 4 };
        var diminishedChordSteps = new[] { 0, 3, 3 };
        var majorFirstInversionSteps = new[] { 4, 3, 5 };
        var minorFirstInversionSteps = new[] { 3, 4, 5 };
        var majorSecondInversionSteps = new[] { 7, 5, 4 };
        var minorSecondInversionSteps = new[] { 7, 5, 3 };

        var chromaticSharp = new[] {
            "C,", "^C,", "D,", "^D,", "E,", "F,", "^F,", "G,", "^G,", "A,", "^A,", "B,",
            "C", "^C", "D", "^D", "E", "F", "^F", "G", "^G", "A", "^A", "B",
            "c", "^c", "d", "^d", "e", "f", "^f", "g", "^g", "a", "^a", "b"
        };

        var chromaticFlat = new[] {
            "C,", "_D,", "D,", "_E,", "E,", "F,", "_G,", "G,", "_A,", "A,", "_B,", "B,",
            "C", "_D", "D", "_E", "E", "F", "_G", "G", "_A", "A", "_B", "B",
            "c", "_d", "d", "_e", "e", "f", "_g", "g", "_a", "a", "_b", "b"
        };

        var chromatic = _random.Next(2) == 0 ? chromaticSharp : chromaticFlat;
        int rootIndex = _random.Next(chromatic.Length - 17); // Ensure we have enough room for the chord steps
        string root = chromatic[rootIndex].TrimEnd(',').ToUpper(); ;

        if (root.StartsWith("^"))
        {
            root = root[1..] + "#"; // Convert to sharp notation
        }
        else if (root.StartsWith("_"))
        {
            root = root[1..] + "b"; // Convert to flat notation
        }
        string chordType = _random.Next(17) switch
        {
            <= 5 => "maj", // Major
            <= 10 => "min", // Minor
            <= 13 => "7",   // Dominant 7th
            <= 14 => "m7",  // Minor 7th
            <= 15 => "maj7",// Major 7th
            _ => "dim", // Diminished
        };

        var inversion = 0;

        if (chordType == "maj")
        {
            inversion = _random.Next(3);
            switch (inversion)
            {
                case 0: // Root position
                    return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, majorChordSteps));
                case 1: // First inversion
                    return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, majorFirstInversionSteps));
                case 2: // Second inversion
                    return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, majorSecondInversionSteps));
            }
        }

        else if (chordType == "min")
        {
            inversion = _random.Next(3);
            switch (inversion)
            {
                case 0: // Root position
                    return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, minorChordSteps));
                case 1: // First inversion
                    return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, minorFirstInversionSteps));
                case 2: // Second inversion
                    return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, minorSecondInversionSteps));
            }
        }
        else if (chordType == "7")
        {
            return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, dominant7ChordSteps));
        }
        else if (chordType == "m7")
        {
            return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, minor7ChordSteps));
        }
        else if (chordType == "maj7")
        {
            return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, major7ChordSteps));
        }
        else // Diminished
        {
            return (root, chordType, inversion, GenerateChordFromSteps(rootIndex, chromatic, diminishedChordSteps));
        }
        return ("", "", 0, ("", ""));
    }

    public (string, string) GenerateChordFromSteps(int rootIndex, string[] chromatic, int[] steps)
    {
        string chord = "[";
        int noteIndex = rootIndex;
        string arpeggio = "";
        int voiceindex = 0;
        int noteDuration = 4;

        foreach (var step in steps)
        {
            noteIndex = noteIndex + step;
            chord += chromatic[noteIndex] + "4";
            arpeggio += "V:" + (voiceindex + 1) + "\n";
            for (int i = 0; i < voiceindex; i++)
            {
                arpeggio += "z";
            }
            arpeggio += chromatic[noteIndex] + noteDuration + "|\n";
            voiceindex++;
            noteDuration--;
        }

        chord += "]";
        return (chord.Trim(), arpeggio);
    }
    

}
