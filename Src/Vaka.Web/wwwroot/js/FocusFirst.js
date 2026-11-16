const Selectors = ["[data-focus-first]", '[aria-invalid="true"]', "[data-form-summary]"];

const insideClosedDetails = (element) => {
  for (let details = element.closest("details:not([open])"); details; details = details.parentElement?.closest("details:not([open])")) {
    if (!element.closest("summary")?.parentElement?.isSameNode(details)) {
      return true;
    }
  }
  return false;
};

const isVisible = (element) => typeof element.checkVisibility === "function"
  ? element.checkVisibility()
  : element.getClientRects().length > 0 && !insideClosedDetails(element);

const firstVisible = (root, selector) => [...root.querySelectorAll(selector)].find(isVisible);

export function focusFirstError(root, action = null) {
  const own = action ? [...root.querySelectorAll("form")].filter((form) => form.getAttribute("action") === action) : [];
  const target = own.map((form) => firstVisible(form, '[aria-invalid="true"]')).find(Boolean)
    ?? Selectors.map((selector) => firstVisible(root, selector)).find(Boolean);
  target?.focus();
  return target ?? null;
}
