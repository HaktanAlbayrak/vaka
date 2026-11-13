import { ThemeToggle } from "./ThemeToggle.js";
import { ShellNav } from "./ShellNav.js";
import { ConnectionStatus } from "./ConnectionStatus.js";
import { PinInput } from "./PinInput.js";
import { LiveCounter } from "./LiveCounter.js";
import { FormValidation } from "./FormValidation.js";

new ThemeToggle(document.querySelector("[data-theme-toggle]"));
new ShellNav(document);
new ConnectionStatus(document.documentElement);
document.querySelectorAll("[data-pin]").forEach((element) => new PinInput(element));
document.querySelectorAll("[data-live-counter]").forEach((element) => new LiveCounter(element));
document.querySelectorAll("form[data-validate]").forEach((form) => new FormValidation(form));
document.querySelector('main [aria-invalid="true"]')?.focus();
