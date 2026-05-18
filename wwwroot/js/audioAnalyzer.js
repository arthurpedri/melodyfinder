let audioContext;
let analyser;
let meydaAnalyzer;
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
    dotnetRef = dotNetObjectReference;

    // Request microphone
    const stream = await navigator.mediaDevices.getUserMedia({
      audio: true,
    });

    // Create audio context
    audioContext = new AudioContext();

    // Create microphone source
    const source = audioContext.createMediaStreamSource(stream);

    // Create Meyda analyzer
    meydaAnalyzer = Meyda.createMeydaAnalyzer({
      audioContext: audioContext,
      source: source,

      // Chroma is the important part
      featureExtractors: ["chroma", "rms"],

      bufferSize: 2048,

      callback: (features) => {
        if (!features || !features.chroma) return;
        // Ignore silence/noise
        // if (features.rms < 0.03) return;
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
        // dotnetRef.invokeMethodAsync("ReceiveFeatures", {
        //   rms: features.rms,
        //   zcr: features.zcr,
        //   centroid: features.spectralCentroid,
        //   flatness: features.spectralFlatness,
        //   rolloff: features.spectralRolloff,
        //   chroma: [...features.chroma],
        // });
      },
    });

    meydaAnalyzer.start();
  },

  stop() {
    if (meydaAnalyzer) meydaAnalyzer.stop();

    if (audioContext) audioContext.close();
  },
};
