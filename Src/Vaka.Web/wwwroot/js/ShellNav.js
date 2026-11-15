const RailKey = "vaka-rail";
const IconFile = "/lib/icons/Icons.svg#";

export class ShellNav {
  constructor(scope) {
    this.root = document.documentElement;
    this.drawer = scope.querySelector("[data-drawer]");
    this.railToggle = scope.querySelector("[data-rail-toggle]");
    this.opener = null;
    this.background = [...scope.querySelectorAll(".shell, .bottom-bar, .skip-link")];
    this.bindRail();
    this.bindDrawer(scope);
    this.revealCurrent(scope.querySelector(".rail .rail-nav"));
  }

  // Etkin menü öğesini menü listesinin görünür alanına getirir; yalnız liste kayar.
  revealCurrent(nav) {
    const link = nav?.querySelector("[aria-current=page]");
    if (!link || nav.clientHeight === 0) return;
    const box = nav.getBoundingClientRect();
    const item = link.getBoundingClientRect();
    if (item.top < box.top) nav.scrollTop -= box.top - item.top + 8;
    else if (item.bottom > box.bottom) nav.scrollTop += item.bottom - box.bottom + 8;
  }

  bindRail() {
    if (!this.railToggle) return;
    this.syncRail();
    this.railToggle.addEventListener("click", () => {
      const collapsed = this.root.dataset.rail === "collapsed";
      if (collapsed) {
        delete this.root.dataset.rail;
      } else {
        this.root.dataset.rail = "collapsed";
      }
      try {
        localStorage.setItem(RailKey, collapsed ? "expanded" : "collapsed");
      } catch (error) {
        // Depolama kapalıysa tercih yalnız bu sayfada kalır.
      }
      this.syncRail();
    });
  }

  syncRail() {
    const collapsed = this.root.dataset.rail === "collapsed";
    this.railToggle.setAttribute("aria-expanded", String(!collapsed));
    this.railToggle.querySelector(".nav-label").textContent = collapsed ? "Genişlet" : "Daralt";
    this.railToggle.querySelector("use").setAttribute("href", IconFile + (collapsed ? "panel-left-open" : "panel-left-close"));
  }

  bindDrawer(scope) {
    if (!this.drawer) return;
    scope.querySelectorAll("[data-drawer-open]").forEach((link) => {
      link.addEventListener("click", (event) => {
        event.preventDefault();
        this.open(link);
      });
    });
    this.drawer.querySelectorAll("[data-drawer-close], .nav-link[href]").forEach((link) => {
      link.addEventListener("click", (event) => {
        if (link.hasAttribute("data-drawer-close")) event.preventDefault();
        this.close();
      });
    });
    scope.addEventListener("keydown", (event) => {
      if (this.drawer.dataset.open !== "true") return;
      if (event.key === "Escape") this.close();
      if (event.key === "Tab") this.trapFocus(event);
    });
  }

  trapFocus(event) {
    const items = [...this.drawer.querySelectorAll(".drawer-panel a[href], .drawer-panel button:not([disabled])")];
    if (items.length === 0) return;
    const first = items[0];
    const last = items[items.length - 1];
    const active = document.activeElement;
    if (!this.drawer.contains(active) || (event.shiftKey && active === first)) {
      event.preventDefault();
      (event.shiftKey ? last : first).focus();
    } else if (!event.shiftKey && active === last) {
      event.preventDefault();
      first.focus();
    }
  }

  open(opener) {
    this.opener = opener;
    this.drawer.dataset.open = "true";
    opener.setAttribute("aria-expanded", "true");
    this.background.forEach((element) => element.setAttribute("inert", ""));
    this.revealCurrent(this.drawer.querySelector(".rail-nav"));
    const first = this.drawer.querySelector(".drawer-panel a[href], .drawer-panel button");
    first.focus();
    if (!this.drawer.contains(document.activeElement)) requestAnimationFrame(() => first.focus());
  }

  close() {
    this.drawer.dataset.open = "false";
    this.background.forEach((element) => element.removeAttribute("inert"));
    if (this.opener) {
      this.opener.setAttribute("aria-expanded", "false");
      this.opener.focus();
    }
    if (location.hash === "#menu") history.replaceState(null, "", location.pathname + location.search);
  }
}
