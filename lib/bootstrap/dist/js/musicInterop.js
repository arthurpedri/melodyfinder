window.musicInterop = {
  renderABC: function (elementId, abcString) {
    if (!window.ABCJS) {
      console.error("abcjs not loaded");
      return;
    }
    ABCJS.renderAbc(elementId, abcString);
  },

  playABC: async function (abcString) {
    if (!window.ABCJS) {
      console.error("abcjs not loaded");
      return;
    }

    const visualObj = ABCJS.renderAbc("notation", abcString)[0]; // use the rendered object
    const synth = new ABCJS.synth.CreateSynth();
    const audioContext = new (window.AudioContext ||
      window.webkitAudioContext)();

    try {
      await synth.init({
        visualObj: visualObj,
        audioContext: audioContext,
        millisecondsPerMeasure: visualObj.millisecondsPerMeasure(),
      });

      const synthControl = new ABCJS.synth.SynthController();
      synthControl.load("#audio-controls", null, {
        displayRestart: true,
        displayPlay: true,
        displayProgress: true,
        displayClock: true,
      });

      synthControl.setTune(visualObj, false).then(() => {
        synthControl.play();
      });
    } catch (err) {
      console.error("Playback failed", err);
    }
  },
};
