export class MediaViewer {
  constructor(dialog) {
    this.dialog = dialog;
    this.stage = dialog.querySelector("[data-media-stage]");
    this.title = dialog.querySelector("[data-media-title]");
    this.opener = null;
    document.addEventListener("click", (event) => {
      const link = event.target.closest("[data-media-viewer]");
      if (!link || event.defaultPrevented || event.metaKey || event.ctrlKey || event.shiftKey) return;
      event.preventDefault();
      this.open(link);
    });
    dialog.querySelector("[data-media-close]")?.addEventListener("click", () => dialog.close());
    dialog.addEventListener("click", (event) => {
      if (event.target === dialog) dialog.close();
    });
    dialog.addEventListener("close", () => this.clear());
  }

  open(link) {
    this.opener = link;
    this.stage.replaceChildren(this.build(link));
    this.title.textContent = link.dataset.mediaName ?? "Önizleme";
    this.dialog.showModal();
  }

  build(link) {
    if (link.dataset.mediaKind === "video") {
      const video = document.createElement("video");
      video.controls = true;
      video.preload = "metadata";
      video.src = link.href;
      return video;
    }
    const image = document.createElement("img");
    image.src = link.href;
    image.alt = link.dataset.mediaName ?? "";
    return image;
  }

  clear() {
    this.stage.querySelector("video")?.pause();
    this.stage.replaceChildren();
    this.opener?.focus();
  }
}
