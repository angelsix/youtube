# Creating a custom GitHub Pages Jekyll

## Prerequisites

First you must install [Ruby](https://rubyinstaller.org/downloads/).

Using command line install jekyll and bundler:

````
gem install jekyll
gem install bundler
````

## Creating new Jekyll Site

Create a new folder on your computer, open the folder in command line, and type `jekyll new .` to create a new Jekyll site in that folder.

Open the created Gemfile and comment out these lines:

```
#gem "jekyll", "~> 3.6.0"
#gem "minima", "~> 2.0"
```

Then comment in this line:

```
gem "github-pages", group: :jekyll_plugins
```

## Building site

Install any dependencies your site may have with:

```
bundle install
```

## Running the site

To run the site type `bundle exec jekyll s`

## Updating Packages

Every now and then, make sure your packages are up-to-date using:

```
bundle update
```

## Configuring basic details

Open the **_config.yml** file and edit the title, email, description with your details.

Set the **baseurl** to the subpath of your site if it is hosted in a subfolder like angelsix.com/blog then **baseurl** would be **blog**. Otherwise leave it as blank.

Set the **url** as the URL where the site will be accessed.

Set your twitter and github details up if needed.

If you are hosting this in GitHub then add this below the github username:

```
repository: angelsix/angelsix.github.io
```

Where `angelsix` is your GitHub username, and `angelsix.github.io` is your repository name.

### Plugins

You can add more plug-ins to the plugin's section. Here are some useful plug-ins I often use:

```
# Add jekyll feed to generate atom feed at /feed.xml
# Add jekyll-avatar for getting GitHub avatar
# Add jekyll-seo-tag to auto add SEO tags into page using {% seo %}
# Add jemoji to allow emoji's via shortnames anywhere in text, like :+1:
plugins:
  - jekyll-feed
  - jekyll-avatar
  - jekyll-seo-tag
  - jekyll-sitemap
  - jemoji
```

## Editing Templates

If you want to customize aa built-in template further, find it on GitHub or wherever it is hosted, for example for **midnight** it is https://github.com/pages-themes/midnight/

Primarily copy the **_layouts** folder to your site, and then copy in any references .css and other files the template uses. Now edit it as you like.

