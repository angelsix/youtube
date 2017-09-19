## Copy/Paste

Ctrl + Z				Undo
Ctrl + Shift + Z		Redo
Ctrl + X				Cut (and entire line)
Ctrl + C				Copy
Ctrl + V				Paste

## Text Manipulation

Ctrl + Click			Select current word
Alt + Drag 			Select area for multi-line edit
Alt + Up/Down		Move lines

## Search & Navigate

F12					Go to reference
Ctrl + T 				Go to... find specific class or type etc...
Ctrl + ; 				Search solution explorer
Ctrl + Tab			Cycle open document
Ctrl + -				Step backwards in cursor position
Ctrl + Shift + -		Step forwards in cursor position
Ctrl + Shift + F		Find all
Ctrl + K + K			Set/remove bookmark
Ctrl + K + N			Next bookmark
Ctrl + K + P			Previous Bookmark

## Environment

Ctrl + Shift + Enter	Full Screen

## Coding

Ctrl + Shift + B		Build
Ctrl + . 				Auto-complete suggestions
Ctrl + R + R			Rename variable/value
Ctrl + K + C 			Comment selection
Ctrl + K + U			Uncomment selection
Ctrl + M + M			Expand/Collapse all code
Ctrl + M + O			Collapse to definition
Ctrl + Q				Quick Launch

## Debugging

F5					Build and Run
F10					Step Over
F11					Step Into

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

Win + W 			Close current window
Win + Q 				Close current program

## Screenshots

Install LightShot from https://app.prntscr.com/

Right-click the Taskbar Icon then click `Options`. Change the `General Hotkey` to your desired shortcut. In my case I use Ctrl + Win + S

## Snippets

Type the snippet below and then double-tab to execute

///				Create summary comment
ctor				Create constructor
region			Creates a new region


## Surround Snippets

Ctrl + K + S		Surround With

The only snippet I use is Region.

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

