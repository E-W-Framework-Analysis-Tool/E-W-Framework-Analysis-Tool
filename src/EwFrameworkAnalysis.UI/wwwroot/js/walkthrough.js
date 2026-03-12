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
    return {
      x: r.x, y: r.y, width: r.width, height: r.height,
      bottom: r.bottom, right: r.right,
      vw: window.innerWidth, vh: window.innerHeight
    };
  },
  scrollIntoView: function (stepId) {
    return new Promise((resolve) => {
      const el = document.querySelector(`[data-walkthrough="${stepId}"]`);
      if (!el) { resolve(); return; }

      const r = el.getBoundingClientRect();
      const alreadyVisible =
        r.top >= 0 && r.bottom <= window.innerHeight &&
        r.left >= 0 && r.right <= window.innerWidth;

      if (alreadyVisible) { resolve(); return; }

      // Blur the spotlight while scrolling
      document.querySelector('.walkthrough-spotlight')?.classList.add('is-scrolling');

      const fallback = setTimeout(() => {
        document.querySelector('.walkthrough-spotlight')?.classList.remove('is-scrolling');
        resolve();
      }, 500);

      const onScrollEnd = () => {
        clearTimeout(fallback);
        document.querySelector('.walkthrough-spotlight')?.classList.remove('is-scrolling');
        resolve();
      };

      window.addEventListener('scrollend', onScrollEnd, { once: true });
      document.addEventListener('scrollend', onScrollEnd, { once: true });

      el.scrollIntoView({ behavior: 'smooth', block: 'center' });
    });
  }
};
