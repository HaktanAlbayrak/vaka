export class KindToggle {
  constructor(form) {
    this.field = form.querySelector("[data-lock-field]");
    this.input = form.querySelector("[data-lock-input]");
    this.radios = [...form.querySelectorAll("[data-kind-radio]")];
    if (!this.field || !this.input || this.radios.length === 0) return;
    this.hint = this.field.querySelector(".field-hint");
    this.radios.forEach((radio) => radio.addEventListener("change", () => this.sync()));
    this.sync();
  }

  sync() {
    const emergency = this.radios.some((radio) => radio.checked && radio.dataset.emergency === "true");
    this.field.hidden = emergency;
    // Gizli alan gönderilmez; acil tipte dolu kilit süresi sunucuda reddedilir.
    this.input.disabled = emergency;
  }
}
