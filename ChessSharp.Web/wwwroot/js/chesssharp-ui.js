window.chessSharpUi = {
  init() {
    if (window.__chessSharpUiReady) {
      return;
    }

    window.__chessSharpUiReady = true;
    window.__chessSharpUiAudio = null;
  },

  animateMove(origin, target) {
    const originSquare = document.querySelector(`[data-square="${origin}"]`);
    const targetSquare = document.querySelector(`[data-square="${target}"]`);

    if (!originSquare || !targetSquare) {
      return;
    }

    const piece = targetSquare.querySelector(".piece-image");

    if (!piece) {
      return;
    }

    const fromRect = originSquare.getBoundingClientRect();
    const toRect = targetSquare.getBoundingClientRect();
    const deltaX = fromRect.left - toRect.left;
    const deltaY = fromRect.top - toRect.top;

    piece.style.transition = "none";
    piece.style.setProperty("--anim-x", `${deltaX}px`);
    piece.style.setProperty("--anim-y", `${deltaY}px`);

    requestAnimationFrame(() => {
      requestAnimationFrame(() => {
        piece.style.transition = "transform 190ms ease-out, filter 160ms ease";
        piece.style.setProperty("--anim-x", "0px");
        piece.style.setProperty("--anim-y", "0px");
      });
    });
  },

  playSound(type) {
    const AudioContextCtor = window.AudioContext || window.webkitAudioContext;

    if (!AudioContextCtor) {
      return;
    }

    if (!window.__chessSharpUiAudio) {
      window.__chessSharpUiAudio = new AudioContextCtor();
    }

    const context = window.__chessSharpUiAudio;

    if (context.state === "suspended") {
      context.resume();
    }

    const now = context.currentTime;
    const gain = context.createGain();
    gain.connect(context.destination);
    gain.gain.setValueAtTime(0.0001, now);

    const oscillator = context.createOscillator();
    oscillator.type = "triangle";

    const profile = type === "check"
      ? { frequency: 610, peak: 0.038, duration: 0.22 }
      : type === "capture"
        ? { frequency: 420, peak: 0.032, duration: 0.18 }
        : { frequency: 520, peak: 0.024, duration: 0.16 };

    oscillator.frequency.setValueAtTime(profile.frequency, now);
    gain.gain.exponentialRampToValueAtTime(profile.peak, now + 0.01);
    gain.gain.exponentialRampToValueAtTime(0.0001, now + profile.duration);

    oscillator.connect(gain);
    oscillator.start(now);
    oscillator.stop(now + profile.duration + 0.03);
  }
};
