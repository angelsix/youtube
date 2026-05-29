# youtube glossary

The agreed language for the youtube repo. This repo is a grab-bag of code that accompanies AngelSix YouTube videos: each top-level folder is a self-contained Sample for a video or series. There is no single product or shared domain here, so the language is deliberately thin, just enough to talk about the repo's shape.

## Language

**Channel**
The AngelSix YouTube channel (angelsix.com/youtube) that all the code in this repo accompanies.

**Sample**
A single self-contained project in a top-level folder, written for one video or one Series (for example "WPF", "C# Beginners", "Avalonia BatchProcess"). Each Sample stands on its own and is not built together with the others.
Avoid: demo, example, project (use "Sample" for the everyday term).

**Video**
A single AngelSix YouTube video. A Sample exists to go with a Video or a Series of Videos.
Avoid: tutorial, episode.

**Series**
A set of Videos on one topic, backed by a single Sample that grows across them.

**Lab**
A Sample that is an ongoing experiment or playground rather than a finished walkthrough (for example AvaloniaThemeLab, PrototypeTheme). A Lab can pull in external packages and evolve over time.
Avoid: experiment, sandbox.

## Flagged ambiguities

The repo has no shared code-level vocabulary; each Sample brings its own. Define terms inside a Sample's own docs if one ever needs them, not here.
