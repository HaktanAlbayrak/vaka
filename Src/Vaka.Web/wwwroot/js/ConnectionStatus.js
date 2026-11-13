export class ConnectionStatus {
  constructor(root) {
    this.root = root;
    this.apply(navigator.onLine);
    window.addEventListener("online", () => this.apply(true));
    window.addEventListener("offline", () => this.apply(false));
  }

  apply(online) {
    this.root.dataset.online = String(online);
    document.querySelectorAll("[data-requires-online]").forEach((control) => {
      if (!online && !control.disabled) {
        control.disabled = true;
        control.dataset.offlineLock = "true";
        control.title = "Bağlantı yokken yapılamaz";
      } else if (online && control.dataset.offlineLock === "true") {
        control.disabled = false;
        delete control.dataset.offlineLock;
        control.removeAttribute("title");
      }
    });
  }
}
