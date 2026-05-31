window.DocsHelpers = {
  _observer: null,
  _dotnet: null,

  initScrollSpy(dotnet) {
    this._dotnet = dotnet;

    if (this._scrollHandler) {
      window.removeEventListener('scroll', this._scrollHandler);
    }

    const navbarHeight = 80;

    this._scrollHandler = () => {
      const headings = Array.from(document.querySelectorAll('article [id]'));
      if (!headings.length) return;

      // Find the last heading that has scrolled past the navbar
      let active = headings[0];
      for (const h of headings) {
        if (h.getBoundingClientRect().top <= navbarHeight + 8) {
          active = h;
        } else {
          break;
        }
      }

      if (active.id !== this._lastSlug) {
        this._lastSlug = active.id;
        dotnet.invokeMethodAsync('SetActiveSlug', active.id);
        history.replaceState(null, '', window.location.pathname + '#' + active.id);
      }
    };

    window.addEventListener('scroll', this._scrollHandler, { passive: true });
    this._scrollHandler(); // run once on init
  },

  scrollToSlug(slug) {
    const el = document.getElementById(slug);
    if (el) {
      const navbarHeight = 80; // h-16 (64px) + some breathing room
      const top = el.getBoundingClientRect().top + window.scrollY - navbarHeight;
      window.scrollTo({ top, behavior: 'smooth' });
    }
  },

  // Returns the current hash (without the #) if present
  getInitialHash() {
    return window.location.hash ? window.location.hash.substring(1) : null;
  }
};
