const OfflineText = "İşlem yapılmadı, bağlantı yok. Bağlantı gelince aynı düğmeyle yeniden deneyin, işlem tek kez yapılır.";

export class CommandForm {
  constructor(form) {
    this.form = form;
    this.status = form.querySelector("[data-command-status]");
    this.message = form.querySelector("[data-command-message]");
    this.busy = false;
    form.addEventListener("submit", (event) => {
      if (event.defaultPrevented || event.submitter?.name === "refresh") return;
      event.preventDefault();
      if (!this.busy) this.send(event.submitter);
    });
  }

  async send(submitter) {
    if (navigator.onLine === false) {
      this.fail(OfflineText);
      return;
    }
    this.setBusy(true);
    const body = new URLSearchParams(new FormData(this.form, submitter));
    try {
      const response = await fetch(this.form.action, { method: "POST", body, credentials: "same-origin" });
      const target = new URL(response.url);
      if (response.redirected && target.pathname + target.search !== location.pathname + location.search) {
        location.assign(response.url);
        return;
      }
      if (response.status >= 500 || response.status === 400) {
        this.fail(`İşlem tamamlanamadı (kod ${response.status}). Yeniden deneyebilirsiniz, aynı işlem iki kez yapılmaz.`);
        return;
      }
      const next = new DOMParser().parseFromString(await response.text(), "text/html");
      const finishForm = next.querySelector("form[data-finish-reason]");
      const dialog = document.querySelector("[data-finish-dialog]");
      if (finishForm && dialog) {
        this.showFinish(dialog, finishForm, submitter);
        return;
      }
      const main = next.getElementById("main");
      if (!main) {
        this.fail("Sunucu yanıtı okunamadı. Sayfayı yenileyip kaydın durumunu kontrol edin.");
        return;
      }
      this.replace(main, next.title);
    } catch {
      this.fail(OfflineText);
    }
  }

  showFinish(dialog, finishForm, submitter) {
    bindFinishDialog(dialog);
    const body = dialog.querySelector("[data-finish-body]");
    const fromDialog = dialog.contains(this.form);
    body.replaceChildren(document.importNode(finishForm, true));
    document.dispatchEvent(new CustomEvent("vaka:content", { detail: { root: body } }));
    if (!fromDialog) {
      const pin = this.form.querySelector("[data-pin-input]");
      if (pin) {
        pin.value = "";
        pin.dispatchEvent(new Event("input", { bubbles: true }));
      }
      dialog.returnTarget = submitter ?? null;
    }
    this.setBusy(false);
    if (!dialog.open) dialog.showModal();
    const focus = body.querySelector('[aria-invalid="true"]') ?? body.querySelector("[data-form-summary]") ?? body.querySelector("textarea, input:not([type=hidden])");
    focus?.focus();
  }

  replace(main, title) {
    document.querySelector("[data-finish-dialog][open]")?.close();
    const current = document.getElementById("main");
    current.replaceChildren(...[...main.childNodes].map((node) => document.importNode(node, true)));
    document.title = title;
    document.dispatchEvent(new CustomEvent("vaka:content"));
    const focus = current.querySelector('[aria-invalid="true"]') ?? current.querySelector("[data-form-summary]") ?? current;
    focus.focus();
  }

  fail(text) {
    this.setBusy(false);
    if (!this.status || !this.message) return;
    this.message.textContent = text;
    this.status.hidden = false;
    this.status.scrollIntoView({ block: "nearest" });
  }

  setBusy(on) {
    this.busy = on;
    if (on && this.status) this.status.hidden = true;
    this.form.toggleAttribute("aria-busy", on);
    this.form.querySelectorAll('button[type="submit"]').forEach((button) => {
      button.disabled = on;
    });
  }
}

function bindFinishDialog(dialog) {
  if (dialog.dataset.bound === "true") return;
  dialog.dataset.bound = "true";
  dialog.addEventListener("click", (event) => {
    if (!event.target.closest("[data-finish-cancel]")) return;
    event.preventDefault();
    dialog.close();
  });
  dialog.addEventListener("close", () => {
    dialog.querySelector("[data-finish-body]")?.replaceChildren();
    dialog.returnTarget?.focus();
  });
}
