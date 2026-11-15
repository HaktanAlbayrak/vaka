export class PinInput {
  constructor(element) {
    this.input = element.querySelector("[data-pin-input]");
    this.slots = [...element.querySelectorAll("[data-pin-slots] > span")];
    this.reveal = element.querySelector("[data-pin-reveal]");
    this.input.addEventListener("beforeinput", (event) => {
      if (event.inputType?.startsWith("insert") && event.data && /\D/.test(event.data)) event.preventDefault();
    });
    this.input.addEventListener("paste", (event) => {
      event.preventDefault();
      const digits = (event.clipboardData?.getData("text") ?? "").replace(/\D/g, "");
      const { selectionStart: from, selectionEnd: to, value, maxLength } = this.input;
      const room = maxLength > 0 ? maxLength - (value.length - (to - from)) : digits.length;
      if (room <= 0 || digits.length === 0) return;
      this.input.setRangeText(digits.slice(0, room), from, to, "end");
      this.input.dispatchEvent(new Event("input", { bubbles: true }));
    });
    this.input.addEventListener("input", () => {
      this.input.value = this.input.value.replace(/\D/g, "");
      this.paint();
    });
    if (this.reveal) {
      this.reveal.addEventListener("click", () => {
        const shown = this.input.type === "text";
        this.input.type = shown ? "password" : "text";
        this.reveal.setAttribute("aria-pressed", String(!shown));
        this.reveal.setAttribute("aria-label", shown ? "PIN'i göster" : "PIN'i gizle");
        this.reveal.querySelector("use").setAttribute("href", "/lib/icons/Icons.svg#" + (shown ? "eye" : "eye-off"));
      });
    }
    this.paint();
  }

  paint() {
    const length = this.input.value.length;
    this.slots.forEach((slot, index) => {
      slot.dataset.filled = String(index < length);
    });
  }
}
