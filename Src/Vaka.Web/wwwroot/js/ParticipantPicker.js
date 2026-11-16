const SearchDelay = 250;

export class ParticipantPicker {
  constructor(root) {
    this.root = root;
    this.url = root.dataset.peopleUrl ?? "";
    this.stepId = root.dataset.stepId ?? "";
    this.search = root.querySelector("[data-people-search]");
    this.results = root.querySelector("[data-people-results]");
    this.status = root.querySelector("[data-people-status]");
    this.loading = root.querySelector("[data-people-loading]");
    this.chosen = root.querySelector("[data-people-chosen]");
    this.chosenName = root.querySelector("[data-people-chosen-name]");
    this.change = root.querySelector("[data-people-change]");
    this.warning = root.querySelector("[data-people-warning]");
    this.input = root.querySelector("[data-people-input]");
    if (!this.search || !this.results || !this.url) return;
    this.pending = 0;
    this.timer = 0;
    this.controller = null;
    this.options = [];
    this.activeIndex = -1;
    root.querySelectorAll("[data-people-js]").forEach((part) => {
      part.hidden = false;
    });
    root.querySelectorAll("[data-people-nojs] :is(input, button)").forEach((control) => {
      control.disabled = true;
    });
    const preset = root.querySelector("template[data-people-pending]");
    if (preset) this.choose({ userId: preset.dataset.userId, fullName: preset.dataset.fullName, registryNo: preset.dataset.registryNo, hasOwnActiveRecord: preset.dataset.warn === "true" }, false);

    this.search.addEventListener("input", () => {
      clearTimeout(this.timer);
      this.timer = setTimeout(() => this.find(), SearchDelay);
    });
    this.search.addEventListener("focus", () => {
      if (this.results.hidden) this.find();
    });
    this.search.addEventListener("blur", () => this.close());
    this.search.addEventListener("keydown", (event) => this.onKey(event));
    this.results.addEventListener("mousedown", (event) => event.preventDefault());
    this.results.addEventListener("click", (event) => {
      const row = event.target.closest("[role='option']");
      if (!row) return;
      const index = Number(row.dataset.index);
      if (this.options[index]?.isAvailable) this.choose(this.options[index], true);
    });
    this.change?.addEventListener("click", () => {
      this.clearChoice();
      this.search.focus();
    });
  }

  onKey(event) {
    const open = !this.results.hidden;
    switch (event.key) {
      case "ArrowDown":
      case "ArrowUp": {
        event.preventDefault();
        const step = event.key === "ArrowDown" ? 1 : -1;
        if (!open) {
          clearTimeout(this.timer);
          this.find(step);
        } else {
          this.move(step);
        }
        break;
      }
      case "Home":
      case "End":
        if (open) {
          event.preventDefault();
          this.activate(-1);
          this.move(event.key === "Home" ? 1 : -1);
        }
        break;
      case "Enter":
        event.preventDefault();
        if (open && this.options[this.activeIndex]) {
          this.choose(this.options[this.activeIndex], true);
        } else if (!open) {
          clearTimeout(this.timer);
          this.find(1);
        }
        break;
      case "Escape":
        if (open) {
          event.preventDefault();
          event.stopPropagation();
          this.close();
        }
        break;
      default:
        break;
    }
  }

  async find(direction = 0) {
    const token = ++this.pending;
    this.controller?.abort();
    this.controller = new AbortController();
    this.setLoading(true);
    this.say("Kişiler aranıyor…");
    try {
      const query = new URLSearchParams({ q: this.search.value.trim(), stepId: this.stepId });
      const response = await fetch(`${this.url}?${query}`, { headers: { Accept: "application/json" }, credentials: "same-origin", signal: this.controller.signal });
      if (!response.ok) throw new Error(String(response.status));
      const data = await response.json();
      if (token !== this.pending) return;
      const items = Array.isArray(data) ? data : (data.items ?? []);
      this.show(items, Array.isArray(data) ? false : data.truncated === true, direction);
    } catch {
      if (token !== this.pending) return;
      this.setLoading(false);
      this.close();
      this.say("Kişiler alınamadı. Bağlantıyı kontrol edip yeniden arayın.", true);
    }
  }

  show(items, truncated, direction) {
    this.setLoading(false);
    this.options = items;
    this.results.replaceChildren(...items.map((item, index) => this.buildOption(item, index)));
    const note = truncated ? " Liste kısaltıldı. Aramayı daraltın." : "";
    if (items.length === 0) {
      this.close();
      this.say("Aramaya uyan aktif operatör yok.");
      return;
    }
    if (truncated) {
      const extra = document.createElement("li");
      extra.className = "combo-note";
      extra.setAttribute("role", "presentation");
      extra.textContent = "Liste kısaltıldı. Aramayı daraltın.";
      this.results.append(extra);
    }
    this.results.hidden = false;
    this.search.setAttribute("aria-expanded", "true");
    this.activate(-1);
    if (direction !== 0) this.move(direction);
    this.say(`${items.length} kişi bulundu.${note}`);
  }

  buildOption(item, index) {
    const row = document.createElement("li");
    row.id = `${this.results.id}-option-${index}`;
    row.setAttribute("role", "option");
    row.setAttribute("aria-selected", "false");
    row.className = "combo-option";
    row.dataset.index = String(index);
    if (!item.isAvailable) row.setAttribute("aria-disabled", "true");
    const name = document.createElement("span");
    name.className = "min-w-0 break-words text-lg font-bold";
    name.textContent = item.fullName;
    const registry = document.createElement("span");
    registry.className = "mono text-sm text-muted";
    registry.textContent = `Sicil ${item.registryNo}`;
    row.append(name, registry);
    const note = item.isAvailable
      ? item.hasOwnActiveRecord ? "Kendi açık kaydı var, yine de eklenebilir." : ""
      : `Eklenemez: ${item.unavailableReason ?? ""}`;
    if (note) {
      const extra = document.createElement("span");
      extra.className = item.isAvailable ? "block text-sm font-semibold text-warn" : "block text-sm font-semibold";
      extra.textContent = note;
      row.append(extra);
    }
    return row;
  }

  move(step) {
    const count = this.options.length;
    if (count === 0) return;
    let index = this.activeIndex;
    for (let tries = 0; tries < count; tries += 1) {
      index = index < 0 ? (step > 0 ? 0 : count - 1) : (index + step + count) % count;
      if (this.options[index].isAvailable) {
        this.activate(index);
        return;
      }
    }
  }

  activate(index) {
    this.results.querySelectorAll("[role='option']").forEach((row) => {
      row.dataset.active = "false";
      row.setAttribute("aria-selected", "false");
    });
    this.activeIndex = index;
    const row = index >= 0 ? this.results.querySelector(`[data-index="${index}"]`) : null;
    if (!row) {
      this.search.removeAttribute("aria-activedescendant");
      return;
    }
    row.dataset.active = "true";
    row.setAttribute("aria-selected", "true");
    this.search.setAttribute("aria-activedescendant", row.id);
    row.scrollIntoView({ block: "nearest" });
  }

  choose(item, focusSearch) {
    this.input.value = String(item.userId);
    this.input.disabled = false;
    this.chosenName.textContent = `${item.fullName}, sicil ${item.registryNo}`;
    this.chosen.hidden = false;
    this.warning.hidden = !item.hasOwnActiveRecord;
    this.showFieldError();
    this.search.value = "";
    clearTimeout(this.timer);
    this.pending += 1;
    this.setLoading(false);
    this.close();
    this.say(`${item.fullName} seçildi.${item.hasOwnActiveRecord ? " Kendi açık kaydı var." : ""}`);
    if (focusSearch) this.search.focus();
  }

  clearChoice() {
    this.input.value = "";
    this.input.disabled = true;
    this.chosen.hidden = true;
    this.warning.hidden = true;
    this.say("Seçim temizlendi.");
  }

  showFieldError() {
    this.search.removeAttribute("aria-invalid");
    document.getElementById(`${this.search.id}-error`)?.remove();
  }

  close() {
    this.results.hidden = true;
    this.results.replaceChildren();
    this.options = [];
    this.activeIndex = -1;
    this.search.setAttribute("aria-expanded", "false");
    this.search.removeAttribute("aria-activedescendant");
  }

  setLoading(on) {
    const listOpen = !this.results.hidden;
    if (this.loading) this.loading.hidden = !on || listOpen;
    this.results.classList.toggle("opacity-60", on && listOpen);
    if (on && listOpen) this.results.setAttribute("aria-busy", "true");
    else this.results.removeAttribute("aria-busy");
  }

  say(text, isError = false) {
    if (!this.status) return;
    this.status.textContent = text;
    this.status.classList.toggle("text-danger", isError);
    this.status.classList.toggle("font-semibold", isError);
    this.status.classList.toggle("text-muted", !isError);
  }
}
