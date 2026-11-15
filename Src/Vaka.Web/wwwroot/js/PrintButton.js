export class PrintButton {
  constructor(button) {
    button.addEventListener("click", () => window.print());
  }
}
