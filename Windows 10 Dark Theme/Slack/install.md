# Slack Dark Theme

As many of you who follow what I do will know, I like things dark. I don't like the white glare of a computer screen.

I am sure at least some of you use [Slack](http://www.slack.com) for your work communications? If not I strongly recommend you check it out.

The only down-side is it doesn't yet have a dark theme. So looking around the internet I found plenty of ways to do it by just editing the [Electron](https://electron.atom.io/) app javascript files and injecting a custom css sheet.

So, in order to do this now before Slack officially supports it, do the following:

 - Press <kbd>Ctrl + R</kbd> to open `Run...` in Windows
 - Type `%LOCALAPPDATA%` and press <kbd>Enter</kbd> to open your current users `AppData` folder, such as `C:\Users\luke\AppData\Local`
 - Go into `slack\app-2.x.x\resources\app.asar.unpacked\src\static` and open the **ssb-interop.js** file
 - Add the following to the end of the **ssb-interop.js** file

```
document.addEventListener("DOMContentLoaded", function() {

   // Then get its webviews
   let webviews = document.querySelectorAll(".TeamView webview");

   // Fetch our CSS in parallel ahead of time
   const cssPath = 'https://raw.githubusercontent.com/angelsix/youtube/develop/Windows%2010%20Dark%20Theme/Slack/slack-dark.css';
   let cssPromise = fetch(cssPath).then(response => response.text());

   // Insert a style tag into the wrapper view
   cssPromise.then(css => {
      let s = document.createElement('style');
      s.type = 'text/css';
      s.innerHTML = css;
      document.head.appendChild(s);
   });

   // Wait for each webview to load
   webviews.forEach(webview => {
      webview.addEventListener('ipc-message', message => {
         if (message.channel == 'didFinishLoading')
            // Finally add the CSS into the webview
            cssPromise.then(css => {
               let script = `
                     let s = document.createElement('style');
                     s.type = 'text/css';
                     s.id = 'slack-custom-css';
                     s.innerHTML = \`${css}\`;
                     document.head.appendChild(s);
                     `
               webview.executeJavaScript(script);
            })
      });
   });
});
```

 - Now close Slack (including in the Taskbar)

 ![taskbar-image](images/posts/2017-09-12-slack-dark-theme/close-slack.png "Slack Taskbar")

 - Restart Slack to see the lovely new dark theme :smiley:

 ![slack-dark](images/posts/2017-09-12-slack-dark-theme/dark-slack.png "Slack Dark")
