export class LiveTimer {
  constructor(element) {
    this.element = element;
    this.base = Number(element.dataset.seconds);
    this.running = element.dataset.running === "true";
    this.progress = element.dataset.progressTarget ? document.getElementById(element.dataset.progressTarget) : null;
    this.progressMax = Number(element.dataset.progressMax);
    this.startedAt = performance.now();
    if (this.running) {
      this.timer = setInterval(() => this.tick(), 1000);
    }
  }

  tick() {
    this.render(this.base + Math.floor((performance.now() - this.startedAt) / 1000));
  }

  render(seconds) {
    const hours = Math.floor(seconds / 3600);
    const minutes = Math.floor((seconds % 3600) / 60);
    const rest = seconds % 60;
    const parts = [];
    if (hours > 0) parts.push([hours, "sa"]);
    if (minutes > 0) parts.push([minutes, "dk"]);
    if (rest > 0 || parts.length === 0) parts.push([rest, "sn"]);
    const nodes = parts.flatMap(([value, unit]) => {
      const number = document.createElement("span");
      number.className = "mono font-bold";
      number.textContent = String(value);
      const label = document.createElement("span");
      label.className = "dur-unit";
      label.textContent = unit;
      return [number, label];
    });
    const wrapper = document.createElement("span");
    wrapper.className = "whitespace-nowrap";
    wrapper.append(...nodes);
    this.element.replaceChildren(wrapper);
    if (this.progress && this.progressMax) {
      this.progress.value = Math.min(seconds, this.progressMax);
    }
  }
}
