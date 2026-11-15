export class FormValidation {
  constructor(form) {
    this.form = form;
    form.noValidate = true;
    form.addEventListener("submit", (event) => {
      const firstInvalid = this.check();
      if (firstInvalid) {
        event.preventDefault();
        firstInvalid.focus();
      }
    });
  }

  check() {
    let first = null;
    const radioGroups = new Set();
    this.form.querySelectorAll("[required]").forEach((field) => {
      if (field.type === "radio") {
        if (radioGroups.has(field.name)) return;
        radioGroups.add(field.name);
      }
      const empty = field.type === "radio" ? !this.form.querySelector(`[name="${field.name}"]:checked`) : !field.value.trim();
      this.mark(field, empty);
      if (empty && !first) first = field;
    });
    return first;
  }

  mark(field, invalid) {
    const errorId = `${field.id || field.name}-error`;
    let error = document.getElementById(errorId);
    if (invalid) {
      field.setAttribute("aria-invalid", "true");
      if (!error) {
        error = document.createElement("p");
        error.id = errorId;
        error.className = "field-error";
        error.textContent = field.dataset.requiredMessage || "Bu alanı doldurun.";
        (field.closest("[data-field], .field") || field.parentElement).appendChild(error);
        field.setAttribute("aria-describedby", `${field.getAttribute("aria-describedby") || ""} ${errorId}`.trim());
      }
    } else {
      field.removeAttribute("aria-invalid");
      if (error) error.remove();
    }
  }
}
