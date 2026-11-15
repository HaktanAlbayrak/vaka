export class TypePicker {
  constructor(root) {
    this.root = root;
    this.url = root.dataset.typeUrl ?? "";
    this.machine = document.getElementById(root.dataset.machineSelect ?? "");
    this.lock = document.getElementById(root.dataset.lockInput ?? "");
    this.note = document.getElementById(root.dataset.materialNote ?? "");
    this.list = root.querySelector("[data-type-list]");
    this.status = root.querySelector("[data-type-status]");
    this.template = root.querySelector("template[data-type-template]");
    this.defaultNote = document.querySelector("[data-lock-default-note]");
    this.maxLock = this.lock?.max ?? "";
    this.pending = 0;
    if (!this.machine || !this.list || !this.template || !this.url) return;
    this.lockTouched = false;
    this.lock?.addEventListener("input", () => {
      this.lockTouched = true;
    });
    this.machine.addEventListener("change", () => this.load());
    this.list.addEventListener("change", (event) => {
      if (event.target.matches('input[type="radio"]')) this.apply(event.target);
    });
  }

  async load() {
    const token = ++this.pending;
    this.render([]);
    this.apply(null);
    const machineId = this.machine.value;
    if (!machineId) {
      this.say("Önce makineyi seçin.");
      return;
    }
    this.say("Temizlik tipleri yükleniyor…");
    try {
      const response = await fetch(`${this.url}?machineId=${encodeURIComponent(machineId)}`, {
        headers: { Accept: "application/json" },
        credentials: "same-origin",
      });
      if (!response.ok) throw new Error(String(response.status));
      const items = await response.json();
      if (token !== this.pending) return;
      this.render(items);
      this.say(items.length === 0 ? "Bu makineye eşli, yürürlükte versiyonu olan planlı tip yok." : "");
    } catch {
      if (token !== this.pending) return;
      this.say("Temizlik tipleri yüklenemedi. Sayfayı yenileyip tekrar deneyin.");
    }
  }

  render(items) {
    this.list.replaceChildren(
      ...items.map((item, index) => {
        const card = this.template.content.firstElementChild.cloneNode(true);
        const input = card.querySelector("input");
        input.id = index === 0 ? "CleaningTypeId" : `CleaningTypeId-${item.id}`;
        input.value = String(item.id);
        input.dataset.materialRequired = String(item.materialRequired);
        input.dataset.defaultLock = String(item.defaultLockMinutes);
        card.querySelector("[data-type-name]").textContent = item.name;
        card.querySelector("[data-type-meta]").textContent =
          `Versiyon ${item.versionNo} · Makine ${item.defaultLockMinutes} dk tutulur · ${item.materialRequired ? "Malzeme gerekli" : "Malzeme gerekmez"}`;
        return card;
      }),
    );
  }

  apply(input) {
    if (this.note) {
      const key = input ? (input.dataset.materialRequired === "true" ? "required" : "optional") : "none";
      this.note.textContent = this.note.dataset[`text${key[0].toUpperCase()}${key.slice(1)}`] ?? "";
    }
    if (this.defaultNote) {
      this.defaultNote.textContent = input ? ` Tip varsayılanı ${input.dataset.defaultLock} dk.` : "";
    }
    if (input && this.lock && !this.lockTouched) this.lock.value = input.dataset.defaultLock;
  }

  say(text) {
    if (this.status) this.status.textContent = text;
  }
}
