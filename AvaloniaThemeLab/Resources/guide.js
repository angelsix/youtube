(() => {
  // --- 1. Interactive app name substitution across commands, paths, namespaces and XAML ---
  (() => {
    const input = document.getElementById('app-name') || document.getElementById('app-name-input');
    const reset = document.getElementById('app-name-reset');
    if (!input) return;

    const hint = document.getElementById('app-name-hint') || input.closest('.app-name-bar')?.querySelector('.hint');
    const defaultName = document.body.dataset.defaultApp || input.getAttribute('value') || input.placeholder || 'MyApp';
    const storageKey = 'angelsix.guide.appName.' + defaultName;
    const slots = [...document.querySelectorAll('.app-name')];

    const sanitize = (raw) => {
      const trimmed = (raw || '').trim();
      const cleaned = trimmed.replace(/\s+/g, '').replace(/[^A-Za-z0-9._-]/g, '');
      const project = cleaned || defaultName;
      let ns = project.replace(/-/g, '_');
      if (/^[0-9]/.test(ns)) ns = '_' + ns;
      return { trimmed, cleaned, project, ns };
    };

    const apply = (raw, persist = true) => {
      const { trimmed, cleaned, project, ns } = sanitize(raw);
      const isCustom = project !== defaultName;
      slots.forEach((el) => {
        el.textContent = el.dataset.ns ? ns : project;
        el.classList.toggle('custom', isCustom);
      });
      if (reset) reset.disabled = trimmed === defaultName || trimmed === '';

      if (hint) {
        if (trimmed && cleaned !== trimmed) {
          hint.textContent = `Using "${project}" (spaces and unsupported characters removed for valid .NET commands).`;
          hint.className = 'hint show warn';
        } else if (isCustom) {
          hint.textContent = `All commands, paths and namespaces now use "${project}".`;
          hint.className = 'hint show';
        } else if (hint.id === 'app-name-hint') {
          hint.textContent = '';
          hint.className = 'hint';
        }
      }

      if (persist) {
        try {
          if (trimmed && trimmed !== defaultName) localStorage.setItem(storageKey, trimmed);
          else localStorage.removeItem(storageKey);
        } catch {}
      }
    };

    try {
      const saved = localStorage.getItem(storageKey);
      if (saved) input.value = saved;
    } catch {}
    apply(input.value, false);

    input.addEventListener('input', () => apply(input.value));
    if (reset) {
      reset.addEventListener('click', () => {
        input.value = defaultName;
        apply(defaultName);
        input.focus();
        input.select();
      });
    }
  })();

  // --- 2. Copy buttons on every code block (automatically omits diff-removed lines) ---
  const getCopyText = (code) => {
    const clone = code.cloneNode(true);
    clone.querySelectorAll('.diff-del').forEach((el) => {
      if (el.nextSibling && el.nextSibling.nodeType === Node.TEXT_NODE && el.nextSibling.nodeValue.startsWith('\n')) {
        el.nextSibling.nodeValue = el.nextSibling.nodeValue.slice(1);
      }
      el.remove();
    });
    return clone.textContent;
  };
  document.querySelectorAll('pre').forEach((pre) => {
    const button = document.createElement('button');
    button.className = 'copy';
    button.type = 'button';
    button.textContent = 'Copy';
    button.addEventListener('click', () => {
      const code = pre.querySelector('code');
      if (!code) return;
      navigator.clipboard.writeText(getCopyText(code)).then(() => {
        button.textContent = 'Copied';
        setTimeout(() => { button.textContent = 'Copy'; }, 1200);
      });
    });
    pre.appendChild(button);
  });

  // --- 3. TOC active section highlight + paper-surface Word Wrap toggle below the TOC card ---
  const tocNav = document.querySelector('nav.toc');
  if (tocNav && !tocNav.parentElement.classList.contains('toc-rail')) {
    const rail = document.createElement('div');
    rail.className = 'toc-rail';
    tocNav.parentNode.insertBefore(rail, tocNav);
    rail.appendChild(tocNav);

    const prefs = document.createElement('div');
    prefs.className = 'toc-prefs';
    const label = document.createElement('label');
    label.className = 'pref-toggle';
    const wrapCb = document.createElement('input');
    wrapCb.type = 'checkbox';
    wrapCb.id = 'pref-word-wrap';
    const textSpan = document.createElement('span');
    textSpan.textContent = 'Word wrap code';
    label.appendChild(wrapCb);
    label.appendChild(textSpan);
    prefs.appendChild(label);

    // Full-width expand / contract icon button (<> / ><)
    const EXPAND_SVG = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><polyline points="7 8 3 12 7 16"/><polyline points="17 8 21 12 17 16"/><line x1="3" y1="12" x2="21" y2="12"/></svg>';
    const CONTRACT_SVG = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round"><polyline points="6 8 10 12 6 16"/><polyline points="18 8 14 12 18 16"/><line x1="2" y1="12" x2="10" y2="12"/><line x1="14" y1="12" x2="22" y2="12"/></svg>';
    const widthBtn = document.createElement('button');
    widthBtn.type = 'button';
    widthBtn.className = 'pref-width-btn';
    prefs.appendChild(widthBtn);
    rail.appendChild(prefs);

    const wrapKey = 'angelsix.guide.wordWrap';
    let wrapOn = true;
    try {
      const saved = localStorage.getItem(wrapKey);
      if (saved !== null) wrapOn = saved === '1';
    } catch {}
    wrapCb.checked = wrapOn;
    document.body.classList.toggle('code-wrap', wrapOn);

    wrapCb.addEventListener('change', () => {
      document.body.classList.toggle('code-wrap', wrapCb.checked);
      try { localStorage.setItem(wrapKey, wrapCb.checked ? '1' : '0'); } catch {}
    });

    const widthKey = 'angelsix.guide.fullWidth';
    let isWide = false;
    try {
      isWide = localStorage.getItem(widthKey) === '1';
    } catch {}
    const syncWidthBtn = () => {
      document.body.classList.toggle('full-width', isWide);
      widthBtn.classList.toggle('is-wide', isWide);
      widthBtn.innerHTML = isWide ? CONTRACT_SVG : EXPAND_SVG;
      const tip = isWide ? 'Return to centered width' : 'Expand to full width';
      widthBtn.title = tip;
      widthBtn.setAttribute('aria-label', tip);
      widthBtn.setAttribute('aria-pressed', isWide ? 'true' : 'false');
    };
    syncWidthBtn();
    widthBtn.addEventListener('click', () => {
      isWide = !isWide;
      syncWidthBtn();
      try { localStorage.setItem(widthKey, isWide ? '1' : '0'); } catch {}
    });
  }

  const links = [...document.querySelectorAll('nav.toc a')];
  const targets = links.map((a) => document.querySelector(a.getAttribute('href')));
  const mark = () => {
    let current = targets[0];
    for (const t of targets) if (t && t.getBoundingClientRect().top < 120) current = t;
    if (!current) return;
    links.forEach((a) => a.classList.toggle('active', a.getAttribute('href') === '#' + current.id));
  };
  document.addEventListener('scroll', mark, { passive: true });
  mark();
  links.forEach((a) => a.addEventListener('click', () => setTimeout(() => {
    links.forEach((l) => l.classList.toggle('active', l === a));
  }, 0)));

  // --- 3b. Rider Dark syntax highlighter for C# and XAML/SVG file blocks (preserves .app-name & diff spans) ---
  (() => {
    const esc = (s) => s.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');

    const CS_KW = new Set([
      'using','namespace','public','private','protected','internal','partial','sealed','static','abstract',
      'virtual','override','readonly','const','class','struct','record','interface','enum','void','var',
      'new','return','if','else','switch','case','default','for','foreach','in','while','do','break',
      'continue','try','catch','finally','throw','is','as','not','and','or','null','true','false',
      'this','base','get','set','init','value','async','await','typeof','nameof','ref','out','params',
      'string','bool','int','double','float','decimal','long','byte','char','object','dynamic','where'
    ]);

    function highlightCsText(text) {
      const rx = /(\/\/\/?[^\n]*|\/\*[\s\S]*?\*\/|^\s*#(?:region|endregion|if|elif|else|endif|pragma|nullable)[^\n]*|\$@"(?:""|[^"])*"|@"(?:""|[^"])*"|\$"(?:\\.|[^"\\])*"|"(?:\\.|[^"\\])*"|'(?:\\.|[^'\\])*'|\b\d+(?:\.\d+)?[fFdDmMuUlL]?\b|\b[A-Za-z_][A-Za-z0-9_]*\b)/gm;
      let out = '';
      let last = 0;
      let m;
      while ((m = rx.exec(text)) !== null) {
        if (m.index > last) out += esc(text.slice(last, m.index));
        const tok = m[0];
        if (tok.startsWith('//') || tok.startsWith('/*')) {
          out += '<span class="hl-cm">' + esc(tok) + '</span>';
        } else if (/^\s*#/.test(tok)) {
          out += '<span class="hl-pp">' + esc(tok) + '</span>';
        } else if (tok.startsWith('"') || tok.startsWith('$') || tok.startsWith('@') || tok.startsWith("'")) {
          out += '<span class="hl-str">' + esc(tok) + '</span>';
        } else if (/^\d/.test(tok)) {
          out += '<span class="hl-num">' + esc(tok) + '</span>';
        } else if (CS_KW.has(tok)) {
          out += '<span class="hl-kw">' + esc(tok) + '</span>';
        } else {
          // Look ahead for '(' to identify method calls/declarations (unless preceded by 'new ' or 'class ')
          const after = text.slice(rx.lastIndex);
          const before = text.slice(Math.max(0, m.index - 12), m.index);
          if (/^\s*(?:<[^>]+>\s*)?\(/.test(after) && !/\b(?:new|class|record|struct)\s+$/.test(before)) {
            out += '<span class="hl-fn">' + esc(tok) + '</span>';
          } else if (/^[A-Z]/.test(tok)) {
            out += '<span class="hl-ty">' + esc(tok) + '</span>';
          } else {
            out += esc(tok);
          }
        }
        last = rx.lastIndex;
      }
      if (last < text.length) out += esc(text.slice(last));
      return out;
    }

    function highlightMarkupExt(inner) {
      // Tokenize inside { ... } e.g. size:Scaled {size:IconSizeXl}, 3 or Binding Theme.IsDark, Mode=TwoWay
      const rx = /(\{|\}|'[^']*'|,|=|\b\d+(?:\.\d+)?\b|[A-Za-z_][A-Za-z0-9_.:-]*)/g;
      let out = '';
      let last = 0;
      let m;
      let expectExtName = true;
      while ((m = rx.exec(inner)) !== null) {
        if (m.index > last) out += esc(inner.slice(last, m.index));
        const tok = m[0];
        if (tok === '{') {
          out += '<span class="hl-mxb">{</span>';
          expectExtName = true;
        } else if (tok === '}') {
          out += '<span class="hl-mxb">}</span>';
          expectExtName = false;
        } else if (tok === ',' || tok === '=') {
          out += '<span class="hl-punc">' + esc(tok) + '</span>';
          expectExtName = false;
        } else if (tok.startsWith("'")) {
          out += '<span class="hl-str">' + esc(tok) + '</span>';
          expectExtName = false;
        } else if (/^\d/.test(tok)) {
          out += '<span class="hl-num">' + esc(tok) + '</span>';
          expectExtName = false;
        } else {
          const after = inner.slice(rx.lastIndex);
          if (expectExtName) {
            out += '<span class="hl-mx">' + esc(tok) + '</span>';
            expectExtName = false;
          } else if (/^\s*=/.test(after)) {
            out += '<span class="hl-mxp">' + esc(tok) + '</span>';
          } else {
            out += '<span class="hl-mxp">' + esc(tok) + '</span>';
          }
        }
        last = rx.lastIndex;
      }
      if (last < inner.length) out += esc(inner.slice(last));
      return out;
    }

    function highlightXamlText(text, state) {
      // Tokenize XAML/XML while tracking whether we're inside a tag or an attribute value across .app-name nodes
      let out = '';
      let i = 0;
      const len = text.length;
      while (i < len) {
        if (state.inQuote) {
          const endQ = text.indexOf(state.inQuote, i);
          if (endQ === -1) {
            out += '<span class="hl-str">' + esc(text.slice(i)) + '</span>';
            break;
          }
          const chunk = text.slice(i, endQ + 1);
          out += '<span class="hl-str">' + esc(chunk) + '</span>';
          state.inQuote = null;
          i = endQ + 1;
          continue;
        }
        if (state.inComment) {
          const endC = text.indexOf('-->', i);
          if (endC === -1) {
            out += '<span class="hl-cm">' + esc(text.slice(i)) + '</span>';
            break;
          }
          out += '<span class="hl-cm">' + esc(text.slice(i, endC + 3)) + '</span>';
          state.inComment = false;
          i = endC + 3;
          continue;
        }
        if (!state.inTag) {
          const lt = text.indexOf('<', i);
          if (lt === -1) {
            out += esc(text.slice(i));
            break;
          }
          if (lt > i) out += esc(text.slice(i, lt));
          if (text.startsWith('<!--', lt)) {
            const endC = text.indexOf('-->', lt + 4);
            if (endC === -1) {
              out += '<span class="hl-cm">' + esc(text.slice(lt)) + '</span>';
              state.inComment = true;
              break;
            }
            out += '<span class="hl-cm">' + esc(text.slice(lt, endC + 3)) + '</span>';
            i = endC + 3;
            continue;
          }
          // Opening or closing tag start
          const tagMatch = /^<\/?\s*([A-Za-z_][A-Za-z0-9_.:-]*)/.exec(text.slice(lt));
          if (tagMatch) {
            const bracket = text[lt + 1] === '/' ? '</' : '<';
            const fullTag = tagMatch[1];
            const dotIdx = fullTag.indexOf('.');
            let tagHtml;
            if (dotIdx !== -1) {
              tagHtml = '<span class="hl-ty">' + esc(fullTag.slice(0, dotIdx)) + '</span>'
                      + '<span class="hl-punc">.</span>'
                      + '<span class="hl-attr">' + esc(fullTag.slice(dotIdx + 1)) + '</span>';
            } else {
              tagHtml = '<span class="hl-ty">' + esc(fullTag) + '</span>';
            }
            out += '<span class="hl-ty">' + esc(bracket) + '</span>' + tagHtml;
            state.inTag = true;
            i = lt + tagMatch[0].length;
            continue;
          }
          out += '&lt;';
          i = lt + 1;
          continue;
        }
        // Inside a tag: handle />, >, attributes, =, and "..."
        const rest = text.slice(i);
        const closeM = /^(\/?>)/.exec(rest);
        if (closeM) {
          out += '<span class="hl-ty">' + esc(closeM[1]) + '</span>';
          state.inTag = false;
          i += closeM[1].length;
          continue;
        }
        const wsM = /^(\s+)/.exec(rest);
        if (wsM) {
          out += wsM[1];
          i += wsM[1].length;
          continue;
        }
        const attrM = /^([A-Za-z_][A-Za-z0-9_.:-]*)(\s*=)/.exec(rest);
        if (attrM) {
          out += '<span class="hl-attr">' + esc(attrM[1]) + '</span><span class="hl-punc">' + esc(attrM[2]) + '</span>';
          i += attrM[0].length;
          continue;
        }
        if (rest[0] === '"' || rest[0] === "'") {
          const q = rest[0];
          const endQ = rest.indexOf(q, 1);
          if (endQ === -1) {
            out += '<span class="hl-str">' + esc(rest) + '</span>';
            state.inQuote = q;
            break;
          }
          const val = rest.slice(1, endQ);
          if (val.startsWith('{') && val.endsWith('}') && !val.startsWith('{}')) {
            out += '<span class="hl-str">' + esc(q) + '</span>' + highlightMarkupExt(val) + '<span class="hl-str">' + esc(q) + '</span>';
          } else {
            out += '<span class="hl-str">' + esc(rest.slice(0, endQ + 1)) + '</span>';
          }
          i += endQ + 1;
          continue;
        }
        out += esc(rest[0]);
        i += 1;
      }
      return out;
    }

    function highlightElement(el, lang, state) {
      // Unwrap any legacy single-letter highlight spans (k, s, t, c) while keeping .app-name, .diff-add, .diff-del
      [...el.querySelectorAll('span.k, span.s, span.t, span.c')].forEach((sp) => {
        sp.replaceWith(document.createTextNode(sp.textContent));
      });
      el.normalize();

      const walk = (node) => {
        if (node.nodeType === Node.TEXT_NODE) {
          const val = node.nodeValue;
          if (!val) return;
          const htmlStr = lang === 'cs' ? highlightCsText(val) : highlightXamlText(val, state);
          if (htmlStr !== esc(val)) {
            const tmp = document.createElement('span');
            tmp.innerHTML = htmlStr;
            const frag = document.createDocumentFragment();
            while (tmp.firstChild) frag.appendChild(tmp.firstChild);
            node.replaceWith(frag);
          }
          return;
        }
        if (node.nodeType === Node.ELEMENT_NODE) {
          if (node.classList.contains('app-name')) return;
          [...node.childNodes].forEach(walk);
        }
      };
      walk(el);
    }

    document.querySelectorAll('.file-header').forEach((header) => {
      const pre = header.nextElementSibling;
      if (!pre || pre.tagName !== 'PRE') return;
      const code = pre.querySelector('code');
      if (!code) return;
      const lang = header.classList.contains('cs') ? 'cs' : (header.classList.contains('axaml') || header.classList.contains('svg') ? 'xaml' : null);
      if (!lang) return;
      highlightElement(code, lang, { inTag: false, inQuote: null, inComment: false });
    });
  })();

  // --- 4. Collapsible file headers (animated), 30-line fade clamp, scroll auto-expand & bottom step-done button ---
  const MAX_LINES = 30;
  const CHEV_SVG = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.4" stroke-linecap="round" stroke-linejoin="round"><polyline points="6 9 12 15 18 9"/></svg>';
  const CHECK_SVG = '<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="20 6 9 17 4 12"/></svg>';
  const headers = [...document.querySelectorAll('.file-header')];
  const items = [];

  headers.forEach((header) => {
    const pre = header.nextElementSibling;
    if (!pre || pre.tagName !== 'PRE') return;

    const wrap = document.createElement('div');
    wrap.className = 'file-code-wrap';
    const inner = document.createElement('div');
    inner.className = 'file-code-inner';
    pre.parentNode.insertBefore(wrap, pre);
    inner.appendChild(pre);
    wrap.appendChild(inner);

    const codeEl = pre.querySelector('code');
    const lineCount = codeEl ? codeEl.textContent.split('\n').length : 0;
    const isLong = lineCount > MAX_LINES;

    if (isLong) {
      wrap.classList.add('is-clamped');
      const fade = document.createElement('div');
      fade.className = 'file-fade';
      const btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'file-expand-btn';
      btn.textContent = '… Show all ' + lineCount + ' lines';
      btn.addEventListener('click', () => {
        pre.style.maxHeight = pre.scrollHeight + 'px';
        wrap.classList.remove('is-clamped');
        wrap.classList.add('is-expanded-full');
        setTimeout(() => { pre.style.maxHeight = ''; }, 340);
      });
      fade.appendChild(btn);
      inner.appendChild(fade);

      const lessRow = document.createElement('div');
      lessRow.className = 'file-collapse-more';
      const lessBtn = document.createElement('button');
      lessBtn.type = 'button';
      lessBtn.className = 'file-expand-btn';
      lessBtn.textContent = 'Show fewer lines';
      lessBtn.addEventListener('click', () => {
        const beforeBottom = wrap.getBoundingClientRect().bottom;
        pre.style.maxHeight = pre.scrollHeight + 'px';
        void pre.offsetHeight;
        wrap.classList.remove('is-expanded-full');
        wrap.classList.add('is-clamped');
        pre.style.maxHeight = '';
        const startTime = performance.now();
        const keepAnchored = (now) => {
          const diff = wrap.getBoundingClientRect().bottom - beforeBottom;
          if (Math.abs(diff) > 0.5) {
            window.scrollBy({ top: diff, behavior: 'instant' });
          }
          if (now - startTime < 380) requestAnimationFrame(keepAnchored);
        };
        requestAnimationFrame(keepAnchored);
      });
      lessRow.appendChild(lessBtn);
      inner.appendChild(lessRow);
    }

    const chev = document.createElement('span');
    chev.className = 'file-chev';
    chev.innerHTML = '<span class="chev-label"></span>' + CHEV_SVG;
    header.appendChild(chev);
    header.setAttribute('role', 'button');
    header.setAttribute('tabindex', '0');

    const setCollapsed = (collapsed, instant = false) => {
      const wasCollapsed = header.classList.contains('is-collapsed');
      header.classList.toggle('is-collapsed', collapsed);
      header.setAttribute('aria-expanded', collapsed ? 'false' : 'true');
      const lbl = chev.querySelector('.chev-label');
      if (lbl) lbl.textContent = collapsed ? (lineCount + ' lines') : 'Hide';

      if (instant) {
        wrap.classList.add('no-anim');
        wrap.classList.toggle('is-collapsed', collapsed);
        wrap.style.height = collapsed ? '0px' : '';
        void wrap.offsetHeight;
        wrap.classList.remove('no-anim');
        return;
      }

      if (wasCollapsed === collapsed && ((collapsed && wrap.style.height === '0px') || (!collapsed && wrap.style.height === ''))) {
        return;
      }

      if (collapsed) {
        // Animate smoothly from current pixel height down to 0
        wrap.style.height = wrap.getBoundingClientRect().height + 'px';
        void wrap.offsetHeight;
        wrap.classList.add('is-collapsed');
        wrap.style.height = '0px';
      } else {
        // Animate smoothly from current height (usually 0) down to target pixel height, then release to auto
        const fromH = wrap.getBoundingClientRect().height;
        wrap.classList.remove('is-collapsed');
        wrap.style.height = fromH + 'px';
        const targetH = inner.scrollHeight;
        void wrap.offsetHeight;
        wrap.style.height = targetH + 'px';
      }
    };

    wrap.addEventListener('transitionend', (e) => {
      if (e.target !== wrap || e.propertyName !== 'height') return;
      if (!wrap.classList.contains('is-collapsed')) {
        wrap.style.height = '';
      }
    });

    const toggle = () => {
      const nowCollapsed = !header.classList.contains('is-collapsed');
      header.dataset.userToggled = '1';
      setCollapsed(nowCollapsed);
    };

    header.addEventListener('click', toggle);
    header.addEventListener('keydown', (e) => {
      if (e.key === 'Enter' || e.key === ' ') {
        e.preventDefault();
        toggle();
      }
    });

    const stepLi = header.closest('ol.steps > li');
    items.push({ header, wrap, stepLi, setCollapsed });
  });

  // Initial state: collapse all file blocks instantly (no flash) except those in the very first step with a file
  const firstStepWithFile = items.find((it) => it.stepLi)?.stepLi || null;
  items.forEach((it, index) => {
    const openInitially = firstStepWithFile ? (it.stepLi === firstStepWithFile) : (index === 0);
    it.setCollapsed(!openInitially, true);
  });

  const stepMap = new Map();
  items.forEach((it) => {
    if (!it.stepLi) return;
    if (!stepMap.has(it.stepLi)) stepMap.set(it.stepLi, []);
    stepMap.get(it.stepLi).push(it);
  });

  // Wire every step checkbox + add bottom "Done with this step" button
  // Checking step i completes 0..i; undoing step i reopens i..end (expanding step i immediately).
  const stepRecords = [];
  document.querySelectorAll('ol.steps > li').forEach((li) => {
    const cb = li.querySelector(':scope > label input[type="checkbox"]');
    if (!cb) return;

    const bar = document.createElement('div');
    bar.className = 'step-done-bar';
    const doneBtn = document.createElement('button');
    doneBtn.type = 'button';
    doneBtn.className = 'step-done-btn';

    const syncUI = () => {
      li.classList.toggle('done', cb.checked);
      doneBtn.classList.toggle('is-done', cb.checked);
      doneBtn.innerHTML = CHECK_SVG + '<span>' + (cb.checked ? 'Step done · Undo' : 'Done with this step') + '</span>';
    };

    const rec = { li, cb, doneBtn, syncUI };
    const idx = stepRecords.length;
    stepRecords.push(rec);
    syncUI();

    const applyRangeFrom = (checked) => {
      if (checked) {
        for (let j = 0; j <= idx; j++) {
          const r = stepRecords[j];
          r.cb.checked = true;
          r.syncUI();
          const sItems = stepMap.get(r.li);
          if (sItems) {
            sItems.forEach((it) => {
              delete it.header.dataset.userToggled;
              it.setCollapsed(true);
            });
          }
        }
        // Auto-expand the immediate next step's files so you can keep flowing straight down
        const nextRec = stepRecords[idx + 1];
        if (nextRec) {
          const nextItems = stepMap.get(nextRec.li);
          if (nextItems) {
            nextItems.forEach((it) => {
              delete it.header.dataset.userToggled;
              it.setCollapsed(false);
            });
          }
        }
      } else {
        for (let j = idx; j < stepRecords.length; j++) {
          const r = stepRecords[j];
          r.cb.checked = false;
          r.syncUI();
          const sItems = stepMap.get(r.li);
          if (sItems) {
            sItems.forEach((it) => {
              delete it.header.dataset.userToggled;
              // Re-expand the step we undid back to; leave later steps collapsed until scrolled into view
              it.setCollapsed(j !== idx);
            });
          }
        }
      }
    };

    cb.addEventListener('change', () => applyRangeFrom(cb.checked));

    doneBtn.addEventListener('click', () => {
      const willCheck = !cb.checked;
      applyRangeFrom(willCheck);
      if (willCheck) {
        setTimeout(() => {
          const rect = li.getBoundingClientRect();
          if (rect.top < 16) {
            li.scrollIntoView({ behavior: 'smooth', block: 'start' });
          }
        }, 120);
      }
    });

    bar.appendChild(doneBtn);
    li.appendChild(bar);
  });

  // Scroll-triggered auto-expand for unchecked steps
  if ('IntersectionObserver' in window) {
    const io = new IntersectionObserver((entries) => {
      entries.forEach((entry) => {
        if (!entry.isIntersecting) return;
        const li = entry.target;
        const cb = li.querySelector(':scope > label input[type="checkbox"]');
        if (cb && cb.checked) return;
        const stepItems = stepMap.get(li);
        if (!stepItems) return;
        stepItems.forEach((it) => {
          if (it.header.dataset.userToggled === '1') return;
          it.setCollapsed(false);
        });
      });
    }, { rootMargin: '-10% 0px -30% 0px', threshold: 0.15 });

    stepMap.forEach((_, li) => io.observe(li));
  }
})();
