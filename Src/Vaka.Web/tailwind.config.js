const channel = (name) => `rgb(var(--${name}) / <alpha-value>)`;

module.exports = {
  content: ["./Views/**/*.cshtml", "./wwwroot/js/**/*.js"],
  theme: {
    colors: {
      transparent: "transparent",
      current: "currentColor",
      bg: channel("bg"),
      surface: channel("surface"),
      sunken: channel("sunken"),
      line: channel("line"),
      "line-strong": channel("line-strong"),
      ink: channel("ink"),
      muted: channel("muted"),
      accent: channel("accent"),
      "accent-ink": channel("accent-ink"),
      "accent-soft": channel("accent-soft"),
      ok: channel("ok"),
      "ok-soft": channel("ok-soft"),
      warn: channel("warn"),
      "warn-soft": channel("warn-soft"),
      danger: channel("danger"),
      "danger-soft": channel("danger-soft"),
      "chart-1": channel("chart-1"),
      "chart-2": channel("chart-2"),
      "chart-3": channel("chart-3"),
    },
    fontFamily: {
      display: ["var(--font-display)"],
      sans: ["var(--font-sans)"],
      mono: ["var(--font-mono)"],
    },
    borderRadius: {
      none: "0",
      ui: "var(--radius-ui)",
      card: "var(--radius-card)",
      full: "9999px",
    },
    extend: {
      spacing: {
        rail: "var(--rail-width)",
        "rail-collapsed": "var(--rail-collapsed)",
      },
      maxWidth: {
        page: "1600px",
        prose: "68ch",
      },
      transitionDuration: {
        180: "180ms",
      },
    },
  },
  plugins: [],
};
