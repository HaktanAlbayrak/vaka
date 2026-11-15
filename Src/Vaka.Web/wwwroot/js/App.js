import { ThemeToggle } from "./ThemeToggle.js";
import { ShellNav } from "./ShellNav.js";
import { ConnectionStatus } from "./ConnectionStatus.js";
import { PinInput } from "./PinInput.js";
import { RevealToggle } from "./RevealToggle.js";
import { LiveCounter } from "./LiveCounter.js";
import { FormValidation } from "./FormValidation.js";
import { FormBusy, bindBusyLinks } from "./FormBusy.js";
import { UnsavedGuard } from "./UnsavedGuard.js";
import { CascadeSelect } from "./CascadeSelect.js";
import { KindToggle } from "./KindToggle.js";
import { ProcedureEditor } from "./ProcedureEditor.js";
import { MediaViewer } from "./MediaViewer.js";
import { TypePicker } from "./TypePicker.js";
import { LotPicker } from "./LotPicker.js";
import { CommandForm } from "./CommandForm.js";

document.documentElement.dataset.js = "true";
new ThemeToggle(document.querySelector("[data-theme-toggle]"));
new ShellNav(document);
new ConnectionStatus(document.documentElement);

// Sayfa içeriği bağlayıcıları; komut formu yanıtı içeriği değiştirince yeniden çalışır.
function bindContent(root) {
  root.querySelectorAll("[data-pin]").forEach((element) => new PinInput(element));
  root.querySelectorAll("[data-reveal]").forEach((element) => new RevealToggle(element));
  root.querySelectorAll("[data-live-counter]").forEach((element) => new LiveCounter(element));
  root.querySelectorAll("form[data-validate]").forEach((form) => new FormValidation(form));
  root.querySelectorAll("form[data-busy-form]").forEach((form) => new FormBusy(form));
  root.querySelectorAll("form[data-unsaved-guard]").forEach((form) => new UnsavedGuard(form));
  root.querySelectorAll("select[data-cascade-url]").forEach((select) => new CascadeSelect(select));
  root.querySelectorAll("[data-type-picker]").forEach((element) => new TypePicker(element));
  root.querySelectorAll("[data-lot-picker]").forEach((element) => new LotPicker(element));
  root.querySelectorAll("form[data-kind-form]").forEach((form) => new KindToggle(form));
  root.querySelectorAll("[data-editor-host]").forEach((host) => new ProcedureEditor(host));
  root.querySelectorAll("[data-media-dialog]").forEach((dialog) => new MediaViewer(dialog));
  root.querySelectorAll("form[data-command-form]").forEach((form) => new CommandForm(form));
  bindBusyLinks(root);
}

bindContent(document);
document.addEventListener("vaka:content", () => {
  const main = document.getElementById("main");
  if (main) bindContent(main);
});
(document.querySelector('main [aria-invalid="true"]') ?? document.querySelector("main [data-form-summary]"))?.focus();
