const Selectors = ["[data-focus-first]", '[aria-invalid="true"]', "[data-form-summary]"];

const firstVisible = (root, selector) => [...root.querySelectorAll(selector)].find((element) => element.checkVisibility());

export function focusFirstError(root, action = null) {
  const own = action ? [...root.querySelectorAll("form")].filter((form) => form.getAttribute("action") === action) : [];
  const target = own.map((form) => firstVisible(form, '[aria-invalid="true"]')).find(Boolean)
    ?? Selectors.map((selector) => firstVisible(root, selector)).find(Boolean);
  target?.focus();
  return target ?? null;
}
