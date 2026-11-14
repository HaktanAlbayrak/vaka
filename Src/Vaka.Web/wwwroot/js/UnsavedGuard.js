export class UnsavedGuard {
  constructor(form) {
    this.dirty = false;
    form.addEventListener("input", () => (this.dirty = true));
    form.addEventListener("change", () => (this.dirty = true));
    form.addEventListener("submit", (event) => {
      if (!event.defaultPrevented) this.dirty = false;
    });
    window.addEventListener("beforeunload", (event) => {
      if (!this.dirty) return;
      event.preventDefault();
      event.returnValue = "";
    });
  }
}
