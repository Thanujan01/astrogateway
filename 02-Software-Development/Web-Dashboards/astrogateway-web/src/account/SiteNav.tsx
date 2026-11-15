import { useEffect, useRef, useState } from 'react'
import type { ReactNode } from 'react'
import { Menu, X } from 'lucide-react'
import './nav.css'

/* ---------------------------------------------------------------------------
 * Header navigation shared by both account apps.
 *
 * Above the mobile breakpoint this renders exactly the inline row it always
 * did. Below it, the links collapse behind a three-bar toggle.
 *
 * Children are whatever each app already passed to its <nav>, so this does not
 * need to know about their differing Link components or sign-out handlers.
 * ------------------------------------------------------------------------- */

const WIDE = '(min-width: 761px)'

export default function SiteNav({ children }: { children: ReactNode }) {
  const [open, setOpen] = useState(false)
  const toggle = useRef<HTMLButtonElement>(null)
  const panel = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (!open) return

    const close = () => setOpen(false)

    const onKey = (event: KeyboardEvent) => {
      if (event.key !== 'Escape') return
      close()
      toggle.current?.focus()
    }

    // Any in-app navigation dispatches popstate, so the panel closes with the route.
    // A click anywhere inside the panel also closes it, which covers every child -
    // links and the sign-out button alike - without enumerating them here. Listening
    // natively rather than with onClick keeps the wrapper a plain, non-interactive div.
    const node = panel.current
    const onPointerDown = (event: MouseEvent) => {
      if (node && !node.contains(event.target as Node)
        && !toggle.current?.contains(event.target as Node)) close()
    }

    window.addEventListener('keydown', onKey)
    window.addEventListener('popstate', close)
    document.addEventListener('pointerdown', onPointerDown)
    node?.addEventListener('click', close)

    // Crossing back to the desktop layout while open would otherwise strand the panel.
    const wide = window.matchMedia(WIDE)
    wide.addEventListener('change', close)

    return () => {
      window.removeEventListener('keydown', onKey)
      window.removeEventListener('popstate', close)
      document.removeEventListener('pointerdown', onPointerDown)
      node?.removeEventListener('click', close)
      wide.removeEventListener('change', close)
    }
  }, [open])

  return (
    <nav aria-label="Main navigation" className="ag-nav" data-open={open || undefined}>
      <button
        ref={toggle}
        type="button"
        className="ag-nav-toggle"
        aria-expanded={open}
        aria-controls="ag-nav-menu"
        aria-label={open ? 'Close menu' : 'Open menu'}
        onClick={() => setOpen(value => !value)}
      >
        {open ? <X size={22} /> : <Menu size={22} />}
        <span className="ag-nav-toggle-text">Menu</span>
      </button>

      <div id="ag-nav-menu" className="ag-nav-items" ref={panel}>
        {children}
      </div>
    </nav>
  )
}
