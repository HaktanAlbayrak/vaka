const StorageKey = "vaka-theme";

export class ThemeToggle {
  constructor(button) {
    if (!button) return;
    this.button = button;
    this.root = document.documentElement;
    this.sync();
    button.addEventListener("click", () => {
      const next = this.root.dataset.theme === "dark" ? "light" : "dark";
      this.root.dataset.theme = next;
      try {
        localStorage.setItem(StorageKey, next);
      } catch (error) {
        // Depolama kapalıysa tercih yalnız bu sayfada kalır.
      }
      this.sync();
    });
  }

  sync() {
    this.button.setAttribute("aria-pressed", String(this.root.dataset.theme === "dark"));
  }
}
