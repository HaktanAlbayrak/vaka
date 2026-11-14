export class FormBusy {
  constructor(form) {
    this.form = form;
    this.buttons = [...form.querySelectorAll('button[type="submit"]')];
    form.addEventListener("submit", (event) => {
      if (event.defaultPrevented) return;
      this.start();
    });
    window.addEventListener("pageshow", (event) => {
      if (event.persisted) this.stop();
    });
  }

  start() {
    this.form.setAttribute("aria-busy", "true");
    // Düğmeyi olaydan sonra kapatmak, gönderilen form verisini etkilemez.
    setTimeout(() => this.buttons.forEach((button) => (button.disabled = true)), 0);
    showLoading(true);
  }

  stop() {
    this.form.removeAttribute("aria-busy");
    this.buttons.forEach((button) => {
      if (button.dataset.offlineLock !== "true") button.disabled = false;
    });
    showLoading(false);
  }
}

export function showLoading(on) {
  const loading = document.querySelector("[data-loading]");
  const target = document.querySelector("[data-loading-target]");
  if (!loading || !target) return;
  loading.hidden = !on;
  target.hidden = on;
}

export function bindBusyLinks(scope) {
  scope.querySelectorAll("a[data-busy-link]").forEach((link) => {
    link.addEventListener("click", (event) => {
      if (event.defaultPrevented || event.metaKey || event.ctrlKey || event.shiftKey) return;
      showLoading(true);
    });
  });
}
