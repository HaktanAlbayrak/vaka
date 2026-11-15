export class Fullscreen {
  constructor(button) {
    if (!button || !document.fullscreenEnabled) return;
    this.button = button;
    button.hidden = false;
    this.sync();
    button.addEventListener("click", () => this.toggle());
    document.addEventListener("fullscreenchange", () => this.sync());
  }

  async toggle() {
    try {
      if (document.fullscreenElement) {
        await document.exitFullscreen();
      } else {
        await document.documentElement.requestFullscreen();
      }
    } catch (error) {
      // İstek reddedilirse durum değişmez; gerçek durum fullscreenchange ile gelir.
    }
  }

  sync() {
    const active = Boolean(document.fullscreenElement);
    this.button.setAttribute("aria-pressed", String(active));
    this.button.setAttribute("aria-label", active ? "Tam ekrandan çık" : "Tam ekrana geç");
  }
}
