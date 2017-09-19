## Copy/Paste

| Key Combination  | Description           |
| ---------------- | --------------------- |
| Ctrl + Z         | Undo                  |
| Ctrl + Shift + Z | Redo                  |
| Ctrl + X         | Cut (and entire line) |
| Ctrl + C         | Copy                  |
| Ctrl + V         | Paste                 |

## Text Manipulation

| Key Combination | Description                     |
| --------------- | ------------------------------- |
| Ctrl + Click    | Select current word             |
| Alt + Drag      | Select area for multi-line edit |
| Alt + Up/Down   | Move lines                      |

## Search & Navigate

| Key Combination  | Description                       |
| ---------------- | --------------------------------- |
| F12              | Go to reference                   |
| Ctrl + T         | Go to...                          |
| Ctrl + ;         | Search Solution Explorer          |
| Ctrl + Tab       | Cycle open documents              |
| Ctrl + -         | Step backwards in cursor position |
| Ctrl + Shift + - | Step forwards in cursor position  |
| Ctrl + Shift + F | Find All                          |
| Ctrl + K + K     | Set/Remove Bookmark               |
| Ctrl + K + N     | Next Bookmark                     |
| Ctrl + K + P     | Previous Bookmark                 |

## Environment

| Key Combination     | Description |
| ------------------- | ----------- |
| Alt + Shift + Enter | Full Screen |

## Coding

| Key Combination  | Description                       |
| ---------------- | --------------------------------- |
| Ctrl + Shift + B | Build                             |
| Ctrl + .         | Auto-complete suggestions         |
| Ctrl + R + R     | Rename variable/value             |
| Ctrl + K + C     | Comment selected text             |
| Ctrl + K + U     | Uncomment selected text           |
| Ctrl + M + M     | Expand/Collapse code              |
| Ctrl + M + O     | Collapse code to definition level |
| Ctrl + Q         | Quick Launch                      |

## Debugging

| Key Combination | Description   |
| --------------- | ------------- |
| F5              | Build and Run |
| F10             | Step Over     |
| F11             | Step Into     |

## Key Bindings

Install AutoHotKeys from https://autohotkey.com

Press `Win + R` to open Run.. dialog. Type `shell:startup` to open the Startup folder.

Inside that folder create a file called `shortcuts.ahk` and it's contents as follows.

```
#q::
	Send, !{F4}
Return

#w::
	Send, ^{F4}
Return
```

Then this file will run every time your computer starts. As this is the first time, save the file, then double-click it to run it now so your shortcuts start working without having to restart your computer.

Now pressing the following keys will work.

| Key Combination | Description           |
| --------------- | --------------------- |
| Win + W         | Close current window  |
| Win + Q         | Close current program |

## Screenshots

Install LightShot from https://app.prntscr.com/

Right-click the Taskbar Icon then click `Options`. Change the `General Hotkey` to your desired shortcut. In my case I use `Ctrl + Win + S`

## Snippets

Type the snippet below and then double-tab to execute

| Key Combination | Description                 |
| --------------- | --------------------------- |
| ///             | Create Summary comment      |
| ctor            | Create constructor function |
| region          | Create region               |


## Surround Snippets

| Key Combination | Description   |
| --------------- | ------------- |
| Ctrl + K + S    | Surround With |

The only snippet I use is **Region**.

Built in snippets are stored in `C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\VC#\Snippets\1033\Visual C#`. Open notepad as administrator then open the files to edit them as desired.

To add your own, go to your `Documents` folder then into `Visual Studio 2017\Code Snippets\Visual C#` folder and create a sub-folder. Inside place your snippet files. Finally in Visual Studio, go to `Tools -> Code Snippet Manager` and click **Add** then select your folder. Now restart Visual Studio.

For surround by region I change the Title and shortcut to `region` removing the prepended # so I don't have to type `Ctrl + K + S #region` instead I can just now type `Ctrl + K + S T Tab` which looks long but when you get used to typing its quicker to select the area of code and tap out that sequence than it is to type `#region` above the code, scroll below it, add new lines, type `#endregion` then new line again. I also fix the newlines around it for how I typically select the text starting at the line in question, and ending at the line below where I want the region to finish.

My `C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\VC#\Snippets\1033\Visual C#\pp_region.snippet` file now looks like this:

```
<?xml version="1.0" encoding="utf-8" ?>
<CodeSnippets  xmlns="http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet">
	<CodeSnippet Format="1.0.0">
		<Header>
			<Title>region</Title>
			<Shortcut>region</Shortcut>
			<Description>Code snippet for #region</Description>
			<Author>Microsoft Corporation</Author>
			<SnippetTypes>
				<SnippetType>Expansion</SnippetType>
				<SnippetType>SurroundsWith</SnippetType>
			</SnippetTypes>
		</Header>
		<Snippet>
			<Declarations>
				<Literal>
					<ID>name</ID>
					<ToolTip>Region name</ToolTip>
					<Default>MyRegion</Default>
				</Literal>
			</Declarations>
			<Code Language="csharp"><![CDATA[#region $name$

		$selected$ $end$
	#endregion
	]]>
			</Code>
		</Snippet>
	</CodeSnippet>
</CodeSnippets>
```

