const SearchDelay = 250;

export class ParticipantPicker {
  constructor(root) {
    this.root = root;
    this.url = root.dataset.peopleUrl ?? "";
    this.stepId = root.dataset.stepId ?? "";
    this.search = root.querySelector("[data-people-search]");
    this.results = root.querySelector("[data-people-results]");
    this.status = root.querySelector("[data-people-status]");
    this.chosen = root.querySelector("[data-people-chosen]");
    this.chosenName = root.querySelector("[data-people-chosen-name]");
    this.warning = root.querySelector("[data-people-warning]");
    this.input = root.querySelector("[data-people-input]");
    if (!this.search || !this.url) return;
    this.pending = 0;
    this.timer = 0;
    root.querySelectorAll("[data-people-js]").forEach((part) => {
      part.hidden = false;
    });
    root.querySelectorAll("[data-people-nojs] :is(input, button)").forEach((control) => {
      control.disabled = true;
    });
    const preset = root.querySelector("template[data-people-pending]");
    if (preset) this.choose({ userId: preset.dataset.userId, fullName: preset.dataset.fullName, registryNo: preset.dataset.registryNo, hasOwnActiveRecord: preset.dataset.warn === "true" });

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
      const button = event.target.closest("[data-people-pick]");
      if (button) this.choose(JSON.parse(button.dataset.peoplePick));
    });
    root.closest("form")?.addEventListener("submit", (event) => {
      if (!this.input.value) {
        event.preventDefault();
        this.say("Bir kişi seçin.");
        this.search.setAttribute("aria-invalid", "true");
        this.search.focus();
      }
    });
  }

  async find() {
    const token = ++this.pending;
    this.say("Kişiler aranıyor…");
    try {
      const query = new URLSearchParams({ q: this.search.value.trim(), stepId: this.stepId });
      const response = await fetch(`${this.url}?${query}`, { headers: { Accept: "application/json" }, credentials: "same-origin" });
      if (!response.ok) throw new Error(String(response.status));
      const items = await response.json();
      if (token !== this.pending) return;
      this.show(items);
    } catch {
      if (token !== this.pending) return;
      this.say("Kişiler alınamadı. Bağlantıyı kontrol edip yeniden arayın.");
    }
  }

  show(items) {
    this.results.replaceChildren(
      ...items.map((item) => {
        const row = document.createElement("li");
        const button = document.createElement("button");
        button.type = "button";
        button.className = "lot-result";
        button.disabled = !item.isAvailable;
        button.dataset.peoplePick = JSON.stringify(item);
        const text = document.createElement("span");
        text.className = "min-w-0";
        const title = document.createElement("span");
        title.className = "block break-words font-semibold";
        title.textContent = item.fullName;
        const detail = document.createElement("span");
        detail.className = "block text-sm text-muted";
        detail.textContent = item.isAvailable ? `Sicil ${item.registryNo}` : `Sicil ${item.registryNo}, eklenemez: ${item.unavailableReason ?? ""}`;
        text.append(title, detail);
        button.append(text);
        row.append(button);
        return row;
      }),
    );
    this.results.hidden = items.length === 0;
    this.say(items.length === 0 ? "Aramaya uyan kişi yok." : `${items.length} kişi bulundu.`);
  }

  choose(item) {
    this.input.value = String(item.userId);
    this.input.disabled = false;
    this.chosenName.textContent = `${item.fullName}, sicil ${item.registryNo}`;
    this.chosen.hidden = false;
    this.warning.hidden = !item.hasOwnActiveRecord;
    this.search.removeAttribute("aria-invalid");
    this.search.value = "";
    this.close();
    this.say(`${item.fullName} seçildi.`);
  }

  close() {
    this.results.hidden = true;
    this.results.replaceChildren();
  }

  say(text) {
    if (this.status) this.status.textContent = text;
  }
}
