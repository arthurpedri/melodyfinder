let audioContext = null;
let analyser;
let meydaAnalyzer = null;
let microphoneStream = null;
let dotnetRef;

const NOTE_NAMES = [
  "C",
  "C#",
  "D",
  "D#",
  "E",
  "F",
  "F#",
  "G",
  "G#",
  "A",
  "A#",
  "B",
];

window.audioAnalyzer = {
  async start(dotNetObjectReference) {
    if (audioContext) return; // Already running

    dotnetRef = dotNetObjectReference;

    // Request microphone
    microphoneStream = await navigator.mediaDevices.getUserMedia({
      audio: true,
    });

    // Create audio context
    audioContext = new AudioContext();

    // Create microphone source
    const source = audioContext.createMediaStreamSource(microphoneStream);

    // Create Meyda analyzer
    meydaAnalyzer = Meyda.createMeydaAnalyzer({
      audioContext: audioContext,
      source: source,

      // Chroma is the important part
      featureExtractors: [
        "chroma",
        "rms",
        "zcr",
        "spectralCentroid",
        "spectralFlatness",
        "spectralRolloff",
      ],

      bufferSize: 2048,

      callback: (features) => {
        if (!features) return;
        // Ignore silence/noise
        if (features.rms < 0.02) return;
        const chroma = features.chroma;

        // Convert to nicer structure
        const result = [];

        for (let i = 0; i < 12; i++) {
          result.push({
            note: NOTE_NAMES[i],
            value: chroma[i] > 0.15 ? chroma[i] : 0, // Threshold to reduce noise
          });
        }

        // Send to Blazor
        dotnetRef.invokeMethodAsync("ReceiveChroma", result);
        dotnetRef.invokeMethodAsync("ReceiveFeatures", {
          rms: features.rms,
          zcr: features.zcr,
          centroid: features.spectralCentroid,
          flatness: features.spectralFlatness,
          rolloff: features.spectralRolloff,
          chroma: [...features.chroma],
        });
      },
    });

    meydaAnalyzer.start();
  },

  async stop() {
    // Stop Meyda
    if (meydaAnalyzer) {
      meydaAnalyzer.stop();
      meydaAnalyzer = null;
    }

    // Stop microphone tracks
    if (microphoneStream) {
      microphoneStream.getTracks().forEach((track) => track.stop());

      microphoneStream = null;
    }

    // Close audio context
    if (audioContext) {
      await audioContext.close();

      audioContext = null;
    }

    console.log("audio stopped");
  },
};
