# Theme philosophy

This theme exists because Avalonia's stock themes are a mess to work with. They run to thousands of files built up over years. Colours and sizes are hard-coded wherever someone needed them. There is no token system, so the same grey is typed out in forty places, slightly differently. Resources are looked up by string keys, so there is no IntelliSense and a typo fails silently at runtime. Changing one corner radius across an app means hunting through dozens of templates.

This theme throws that approach away. Every design decision lives in one C# class, every control theme reads from it, and the compiler checks the lot. `ThemeRules.md` says how to write a control theme. This file says why the rules exist and what they protect, so you can tell what is right when no rule covers the case in front of you.

## What it stands for

### One place for every design decision

Colours, type scale, spacing, radii, control heights and thicknesses are tokens in `DefaultTheme`. A control theme never decides a value for itself. It asks for a token. If you want to know why a button is the size it is, the answer is in one file, not spread across templates.

### A new look is a subclass, not a rewrite

A theme is a single C# class. To make a different look you subclass `DefaultTheme` and change values: a hue, a radius, a font size. Every control picks the change up. A dark variant, a brand theme and a Cupertino theme are all made this way.

So the first question for any new look is "which values change?", never "which templates do I copy?". A control theme changes only when values cannot express the difference, and then it changes by the rules (a new exposed property, a declared variant class, or a token that more than one control needs). That change is made once, in the base, so every theme built on it gains it.

### Few tokens, used well

A token earns its place when it is a real design dimension that several controls share. Before adding one, work with what is there: a colour has a hue and its stages (`Light10`, `Dark3` and so on) and accent roles, so "a lighter tint of the accent" is an existing token, not a new one. A token added for one control, or a near-duplicate of an existing value, is how the old mess starts again.

### The compiler is the first reviewer

Tokens are generated into markup extensions (`{color:...}`, `{size:...}`, `{theme:...}`), so the IDE lists them and a misspelt or removed token is a build error. Anything that sidesteps this (a hex colour, a literal size, a string resource key) hides a mistake until someone notices it on screen. The closed list of exemptions in `ThemeRules.md` is the only place a literal may appear, and a deliberate deviation is marked `Theme Exception:` so nobody "fixes" it later.

### Controls are themable by their users

Every visual property a control has is set by a style setter in its theme, and its template reads those properties with `TemplateBinding`. That way an app can still restyle any control through normal Avalonia styling, with normal precedence. A value written straight into a template locks the user out.

### The theme draws, the page lays out

A view built on this theme places controls with panels, rows, columns and alignment. It does not paint them. No backgrounds, borders, radii, fonts or colours on a page; no helper classes invented to nudge one page into shape; no padding repeated from page to page because the control should have had it. If a page needs a value the control does not provide, the control theme is wrong, and that is where the fix goes.

Style classes on a control are for real variants a user would choose, such as a TextBox's `clearButton` or `revealPasswordButton`, declared by the control's own theme (Rule 17). They are not a place to park one-off fixes.

### As little XAML as possible

Every element, attribute and value is something that can break, so the best XAML is the shortest XAML that does the job. Before adding an element, ask whether an existing one can already do it.

A panel inside a panel that lays out the same way is one panel too many. A Border added only to give its child padding is redundant when the child has Padding of its own. A margin on every child is a Spacing on their panel. A value repeated in several places is a missing setter or token. Nothing is wrapped "just in case", and nothing is set to the value it already has.

This applies to control templates as much as to views. A short template with few named parts is easier to read, cheaper to measure and harder to break, and there are fewer values for a user to fight when they restyle it.

### Small enough to read

A control theme should read top to bottom in one sitting: its template with named parts, then its states in a fixed order (Rule 11). Comments say what a section is for, not what the markup already says. Dead and legacy comments go.

## How a look from elsewhere is made

When the goal is to match another theme, such as Cupertino, that theme is a picture of the target, not a source to copy. Look at it, measure it, then express what you see in this engine's terms: values first, then the smallest rule-abiding control theme change where values cannot reach. Copying its templates, its resources or its page structure brings back exactly the problems this theme was built to remove, and the result would no longer prove anything about this engine.

Matching the picture closely is good. Matching it by breaking the rules is a failure, however close it looks.

## When the engine cannot do something

Say so, and fix the engine properly: expose the property, add the variant to the base control theme, or add a token that genuinely serves more than one control. Write down why. Never patch around it in a page, a subclass-only template copy or a local style, because the next person will copy the patch.
