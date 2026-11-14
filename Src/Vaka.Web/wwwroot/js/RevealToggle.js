export class RevealToggle {
  constructor(element) {
    this.input = element.querySelector("[data-reveal-input]");
    this.button = element.querySelector("[data-reveal-toggle]");
    this.button.addEventListener("click", () => {
      const shown = this.input.type === "text";
      this.input.type = shown ? "password" : "text";
      this.button.setAttribute("aria-pressed", String(!shown));
      this.button.setAttribute("aria-label", shown ? "Parolayı göster" : "Parolayı gizle");
      this.button.querySelector("use").setAttribute("href", "/lib/icons/Icons.svg#" + (shown ? "eye" : "eye-off"));
    });
  }
}
