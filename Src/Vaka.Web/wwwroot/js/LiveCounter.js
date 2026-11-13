export class LiveCounter {
  constructor(element) {
    this.element = element;
    this.base = Number(element.dataset.seconds);
    this.paused = element.dataset.paused === "true";
    this.startedAt = performance.now();
    this.progress = element.dataset.progressTarget ? document.getElementById(element.dataset.progressTarget) : null;
    this.progressMax = Number(element.dataset.progressMax);
    this.render(this.base);
    if (!this.paused) {
      this.timer = setInterval(() => this.tick(), 1000);
    }
  }

  tick() {
    const shown = this.base + Math.floor((performance.now() - this.startedAt) / 1000);
    this.render(shown);
  }

  render(seconds) {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const rest = seconds % 60;
    const pad = (value) => String(value).padStart(2, "0");
    this.element.textContent = hours > 0 ? `${hours}:${pad(minutes)}:${pad(rest)}` : `${pad(minutes)}:${pad(rest)}`;
    if (this.progress && this.progressMax) {
      this.progress.value = Math.min(seconds, this.progressMax);
    }
  }
}
