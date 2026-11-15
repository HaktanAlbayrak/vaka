export class CascadeSelect {
  constructor(source) {
    this.source = source;
    this.target = document.getElementById(source.dataset.cascadeTarget ?? "");
    this.url = source.dataset.cascadeUrl ?? "";
    this.status = document.getElementById(source.dataset.cascadeStatus ?? "");
    if (!this.target || !this.url) return;
    this.param = source.dataset.cascadeParam ?? "facilityId";
    this.noun = source.dataset.cascadeNoun ?? "Hatlar";
    this.noneText = source.dataset.cascadeNone ?? "Bu tesiste hat yok.";
    this.blankText = this.target.options[0]?.textContent ?? "";
    this.blankValue = this.target.options[0]?.value ?? "";
    this.busyList = document.getElementById(this.target.dataset.cascadeBusy ?? "");
    this.busyBox = document.getElementById(this.target.dataset.cascadeBusyBox ?? "");
    this.pending = 0;
    source.form?.querySelectorAll("[data-cascade-fallback]").forEach((item) => {
      item.hidden = true;
    });
    source.addEventListener("change", () => this.load());
  }

  async load() {
    const token = ++this.pending;
    const value = this.source.value;
    this.fill([]);
    if (!value) return;

    this.target.disabled = true;
    this.say(`${this.noun} yükleniyor…`);
    try {
      const response = await fetch(`${this.url}?${this.param}=${encodeURIComponent(value)}`, {
        headers: { Accept: "application/json" },
        credentials: "same-origin",
      });
      if (!response.ok) throw new Error(String(response.status));
      const items = await response.json();
      if (token !== this.pending) return;
      this.fill(items);
      this.say(items.length === 0 ? this.noneText : "");
    } catch {
      if (token !== this.pending) return;
      this.say(`${this.noun} yüklenemedi. Sayfayı yenileyip tekrar deneyin.`);
    } finally {
      if (token === this.pending) this.target.disabled = false;
    }
  }

  fill(items) {
    this.target.replaceChildren(new Option(this.blankText, this.blankValue));
    items.forEach((item) => {
      const busy = item.isAvailable === false;
      const option = new Option(busy ? `${item.label} - ${item.busyReason ?? ""}` : item.label, String(item.id));
      option.disabled = busy;
      this.target.append(option);
    });
    this.target.value = this.blankValue;
    this.paintBusy(items.filter((item) => item.isAvailable === false));
    this.target.dispatchEvent(new Event("change", { bubbles: true }));
  }

  paintBusy(busy) {
    if (!this.busyList) return;
    this.busyList.replaceChildren(
      ...busy.map((item) => {
        const row = document.createElement("li");
        row.className = "break-words";
        const name = document.createElement("span");
        name.className = "font-semibold";
        name.textContent = `${item.label}:`;
        row.append(name, ` ${item.busyReason ?? ""}`);
        return row;
      }),
    );
    if (this.busyBox) this.busyBox.hidden = busy.length === 0;
  }

  say(text) {
    if (this.status) this.status.textContent = text;
  }
}
