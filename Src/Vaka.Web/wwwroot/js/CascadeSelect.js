export class CascadeSelect {
  constructor(source) {
    this.source = source;
    this.target = document.getElementById(source.dataset.cascadeTarget ?? "");
    this.url = source.dataset.cascadeUrl ?? "";
    this.status = document.getElementById(source.dataset.cascadeStatus ?? "");
    if (!this.target || !this.url) return;
    this.blankText = this.target.options[0]?.textContent ?? "";
    this.blankValue = this.target.options[0]?.value ?? "";
    this.pending = 0;
    source.form?.querySelectorAll("[data-cascade-fallback]").forEach((item) => {
      item.hidden = true;
    });
    source.addEventListener("change", () => this.load());
  }

  async load() {
    const token = ++this.pending;
    const facilityId = this.source.value;
    this.fill([]);
    if (!facilityId) return;

    this.target.disabled = true;
    this.say("Hatlar yükleniyor…");
    try {
      const response = await fetch(`${this.url}?facilityId=${encodeURIComponent(facilityId)}`, {
        headers: { Accept: "application/json" },
        credentials: "same-origin",
      });
      if (!response.ok) throw new Error(String(response.status));
      const items = await response.json();
      if (token !== this.pending) return;
      this.fill(items);
      this.say(items.length === 0 ? "Bu tesiste hat yok." : "");
    } catch {
      if (token !== this.pending) return;
      this.say("Hatlar yüklenemedi. Sayfayı yenileyip tekrar deneyin.");
    } finally {
      if (token === this.pending) this.target.disabled = false;
    }
  }

  fill(items) {
    this.target.replaceChildren(new Option(this.blankText, this.blankValue));
    items.forEach((item) => this.target.append(new Option(item.label, String(item.id))));
    this.target.value = this.blankValue;
  }

  say(text) {
    if (this.status) this.status.textContent = text;
  }
}
