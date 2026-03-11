window.Walkthrough = {
  start: function () {
    document.body.style.overflow = 'hidden';
  },
  stop: function () {
    document.body.style.overflow = '';
  },
  getRect: function (stepId) {
    const el = document.querySelector(`[data-walkthrough="${stepId}"]`);
    if (!el) return null;
    const r = el.getBoundingClientRect();
    return { x: r.x, y: r.y, width: r.width, height: r.height, bottom: r.bottom, right: r.right };
  },
  scrollIntoView: function (stepId) {
    const el = document.querySelector(`[data-walkthrough="${stepId}"]`);
    el?.scrollIntoView({ behavior: 'smooth', block: 'center' });
  }
};
