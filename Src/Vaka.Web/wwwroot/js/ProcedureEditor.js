import { PinInput } from "./PinInput.js";
import { FormValidation } from "./FormValidation.js";
import { FormBusy } from "./FormBusy.js";

const BytesPerMb = 1048576;

export class ProcedureEditor {
  constructor(host) {
    this.host = host;
    this.announce = host.querySelector("[data-editor-announce]");
    this.busy = false;
    host.addEventListener("submit", (event) => {
      const form = event.target.closest("form[data-editor-form]");
      if (!form || event.defaultPrevented) return;
      event.preventDefault();
      if (this.busy) return;
      const input = form.querySelector("[data-media-input]");
      if (input && !this.checkFile(input)) return;
      this.send(form, event.submitter);
    });
    host.addEventListener("change", (event) => {
      if (event.target.matches("[data-media-input]")) this.checkFile(event.target);
    });
  }

  get region() {
    return this.host.querySelector("[data-editor-region]");
  }

  checkFile(input) {
    const message = this.fileProblem(input);
    const error = input.closest(".field")?.querySelector("[data-media-error]");
    if (error) {
      error.hidden = !message;
      error.textContent = message ?? "";
    }
    if (message) input.setAttribute("aria-invalid", "true");
    else input.removeAttribute("aria-invalid");
    if (message) input.focus();
    return !message;
  }

  // Yalnız kolaylık; izin verilen biçim ve boyut kararı sunucudadır.
  fileProblem(input) {
    const file = input.files?.[0];
    if (!file) return null;
    const allowed = (input.accept || "").split(",").map((item) => item.trim()).filter(Boolean);
    if (file.type && allowed.length > 0 && !allowed.includes(file.type)) {
      return "Bu dosya türü kabul edilmiyor. Seçici yalnız izin verilen biçimleri gösterir.";
    }
    const isVideo = file.type.startsWith("video/");
    const limitMb = Number(isVideo ? this.host.dataset.maxVideoMb : this.host.dataset.maxPhotoMb);
    if (limitMb > 0 && file.size > limitMb * BytesPerMb) {
      return `${isVideo ? "Video" : "Fotoğraf"} en çok ${limitMb} MB olabilir.`;
    }
    return null;
  }

  async send(form, submitter) {
    this.busy = true;
    form.setAttribute("aria-busy", "true");
    const body = new FormData(form, submitter);
    const buttons = [...form.querySelectorAll("button")];
    buttons.forEach((button) => (button.disabled = true));
    const multipart = form.enctype === "multipart/form-data";
    try {
      const response = await fetch(form.action, {
        method: "POST",
        body: multipart ? body : new URLSearchParams(body),
        credentials: "same-origin",
      });
      const target = new URL(response.url);
      if (response.redirected && target.pathname !== location.pathname) {
        location.assign(response.url);
        return;
      }
      const html = await response.text();
      const next = new DOMParser().parseFromString(html, "text/html").querySelector("[data-editor-region]");
      if (!response.ok || !next) {
        this.fail(form, buttons, response.status);
        return;
      }
      this.region.replaceWith(document.importNode(next, true));
      this.afterRender(form);
    } catch {
      this.fail(form, buttons, 0);
    } finally {
      this.busy = false;
    }
  }

  fail(form, buttons, status) {
    form.removeAttribute("aria-busy");
    buttons.forEach((button) => (button.disabled = false));
    const text = status === 413
      ? "Dosya sunucunun kabul ettiği boyutu aşıyor. Daha küçük bir dosya seçin."
      : "İşlem tamamlanamadı. Bağlantıyı kontrol edip tekrar deneyin.";
    let box = form.querySelector("[data-editor-fail]");
    if (!box) {
      box = document.createElement("p");
      box.className = "notice notice-danger";
      box.tabIndex = -1;
      box.setAttribute("role", "alert");
      box.dataset.editorFail = "";
      form.prepend(box);
    }
    box.textContent = text;
    box.focus();
  }

  afterRender(form) {
    const region = this.region;
    region.querySelectorAll("[data-pin]").forEach((element) => new PinInput(element));
    region.querySelectorAll("form[data-validate]").forEach((element) => new FormValidation(element));
    region.querySelectorAll("form[data-busy-form]").forEach((element) => new FormBusy(element));
    const error = region.querySelector("[data-editor-error]");
    const status = region.querySelector("[data-editor-status]");
    this.announce.textContent = (error ?? status)?.textContent.trim() ?? "";
    if (error) {
      error.focus();
      return;
    }
    const candidates = (form.dataset.focus ?? "").split(" ").filter(Boolean);
    const next = candidates.map((id) => (id.startsWith("last-media:") ? document.querySelector(`#phase-${id.slice(11)}-steps > li:last-child [data-media-summary]`) : document.getElementById(id))).find((element) => element && !element.disabled);
    (next ?? status ?? region).focus();
  }
}
