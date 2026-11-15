const SearchDelay = 250;

export class LotPicker {
  constructor(root) {
    this.root = root;
    this.url = root.dataset.lotUrl ?? "";
    this.recordId = root.closest("[data-record-id]")?.dataset.recordId ?? "";
    this.single = root.dataset.lotMode === "single";
    this.nameId = root.dataset.nameId ?? "";
    this.nameReason = root.dataset.nameReason ?? "";
    this.box = root.querySelector("[data-lot-search-box]");
    this.search = root.querySelector("[data-lot-search]");
    this.results = root.querySelector("[data-lot-results]");
    this.selected = root.querySelector("[data-lot-selected]");
    this.empty = root.querySelector("[data-lot-empty]");
    this.status = root.querySelector("[data-lot-status]");
    this.rowTemplate = root.querySelector("template[data-lot-row-template]");
    this.badgeTemplate = root.querySelector("template[data-lot-badge-template]");
    if (!this.search || !this.url) return;
    this.pending = 0;
    this.timer = 0;
    this.box.hidden = false;
    document.querySelectorAll("[data-lot-nojs] :is(input, textarea, button)").forEach((control) => {
      control.disabled = true;
    });
    const pending = root.querySelector("template[data-lot-pending]");
    if (pending) this.selected.append(pending.content.cloneNode(true));
    this.reindex();

    this.search.addEventListener("input", () => {
      clearTimeout(this.timer);
      this.timer = setTimeout(() => this.find(), SearchDelay);
    });
    this.search.addEventListener("focus", () => {
      if (this.results.hidden) this.find();
    });
    this.search.addEventListener("keydown", (event) => {
      if (event.key === "Enter") {
        event.preventDefault();
        clearTimeout(this.timer);
        this.find();
      }
      if (event.key === "Escape") this.close();
    });
    this.results.addEventListener("click", (event) => {
      const button = event.target.closest("[data-lot-pick]");
      if (button) this.pick(JSON.parse(button.dataset.lotPick));
    });
    this.selected.addEventListener("click", (event) => {
      const button = event.target.closest("[data-lot-remove]");
      if (!button) return;
      button.closest("[data-lot-row]").remove();
      this.reindex();
      this.search.focus();
    });
    this.root.closest("form")?.addEventListener("submit", (event) => {
      if (this.root.dataset.lotRequired === "true" && this.rows.length === 0) {
        event.preventDefault();
        this.say("Bir lot seçin.");
        this.search.setAttribute("aria-invalid", "true");
        this.search.focus();
      }
    });
  }

  get rows() {
    return [...this.selected.querySelectorAll("[data-lot-row]")];
  }

  async find() {
    const token = ++this.pending;
    this.say("Lotlar aranıyor…");
    try {
      const query = new URLSearchParams({ q: this.search.value.trim() });
      if (this.recordId) query.set("recordId", this.recordId);
      const response = await fetch(`${this.url}?${query}`, {
        headers: { Accept: "application/json" },
        credentials: "same-origin",
      });
      if (!response.ok) throw new Error(String(response.status));
      const items = await response.json();
      if (token !== this.pending) return;
      this.show(items);
    } catch {
      if (token !== this.pending) return;
      this.say("Lotlar alınamadı. Bağlantıyı kontrol edip yeniden arayın.");
    }
  }

  show(items) {
    this.results.replaceChildren(
      ...items.map((item) => {
        const row = document.createElement("li");
        const button = document.createElement("button");
        button.type = "button";
        button.className = "lot-result";
        button.dataset.lotPick = JSON.stringify(item);
        const text = document.createElement("span");
        text.className = "min-w-0";
        const title = document.createElement("span");
        title.className = "block break-words font-semibold";
        title.textContent = `${item.materialName} (${item.materialCode})`;
        const detail = document.createElement("span");
        detail.className = "block text-sm text-muted";
        detail.textContent = `Lot ${item.lotNo} · SKT ${formatDate(item.expiryDate)}`;
        text.append(title, detail);
        button.append(text);
        if (item.isExpired && this.badgeTemplate) button.append(this.badgeTemplate.content.cloneNode(true));
        row.append(button);
        return row;
      }),
    );
    this.results.hidden = items.length === 0;
    this.say(items.length === 0 ? "Aramaya uyan lot yok." : `${items.length} lot bulundu.`);
  }

  pick(item) {
    if (!this.single && this.rows.some((row) => row.dataset.lotId === String(item.id))) {
      this.say("Bu lot zaten seçili.");
      return;
    }
    if (this.single) this.rows.forEach((row) => row.remove());
    const row = this.rowTemplate.content.firstElementChild.cloneNode(true);
    const label = `${item.materialName} (${item.materialCode}), lot ${item.lotNo}, SKT ${formatDate(item.expiryDate)}`;
    row.dataset.lotId = String(item.id);
    row.dataset.expired = String(item.isExpired);
    row.querySelector("[data-lot-id-input]").value = String(item.id);
    row.querySelector("[data-lot-label]").textContent = label;
    row.querySelector("[data-lot-remove]").setAttribute("aria-label", `${label} lotunu çıkar`);
    row.querySelector("[data-lot-expired-note]").hidden = !item.isExpired;
    row.querySelector("[data-lot-reason]").hidden = !item.isExpired;
    row.querySelector("[data-lot-reason-input]").required = item.isExpired;
    this.selected.append(row);
    this.reindex();
    this.search.removeAttribute("aria-invalid");
    this.search.value = "";
    this.close();
    this.say(`${label} eklendi.`);
    if (item.isExpired) row.querySelector("[data-lot-reason-input]").focus();
    else this.search.focus();
  }

  close() {
    this.results.hidden = true;
    this.results.replaceChildren();
  }

  reindex() {
    this.rows.forEach((row, index) => {
      const idInput = row.querySelector("[data-lot-id-input]");
      const reason = row.querySelector("[data-lot-reason-input]");
      idInput.name = this.nameId.replace("{i}", String(index));
      reason.name = this.nameReason.replace("{i}", String(index));
      reason.id = reason.name.replaceAll(".", "-");
      reason.previousElementSibling?.setAttribute("for", reason.id);
    });
    if (this.empty) this.empty.hidden = this.rows.length > 0;
  }

  say(text) {
    if (this.status) this.status.textContent = text;
  }
}

function formatDate(value) {
  const [year, month, day] = String(value).split("-");
  return `${day}.${month}.${year}`;
}
