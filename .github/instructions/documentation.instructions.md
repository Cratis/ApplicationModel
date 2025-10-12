---
applyTo: "Documentation/**/*.md"
---

# 🧪 How to Write Documentation

- All documentation files live in the `Documentation/` folder.
- Use [Markdown](https://www.markdownguide.org/basic-syntax/) for formatting.
- Use [GitHub Flavored Markdown](https://github.github.com/gfm/) for additional features.
- Use [Mermaid](https://mermaid-js.github.io/mermaid/#/) for diagrams.
- Use [PlantUML](https://plantuml.com/) for UML diagrams.
- Site is build using [DocFX](https://dotnet.github.io/docfx/).
- Follow the [DocFX Markdown](https://dotnet.github.io/docfx/markdown/) guidelines for additional syntax.
- Generate and maintain correct [DocFX TOC files](https://dotnet.github.io/docfx/docs/dotnet-yaml-format.html) for navigation.
- Be consistent with formatting and style.
- Use clear and concise language.
- Use headings, lists, and code blocks to organize content.
- Include links to relevant resources and references.
- Always add an index.md file in each folder to serve as the landing page.
- Maintain the index.md file to include links to all relevant subtopics.
- Use relative links for internal documentation references.
- Ensure all links are valid and up-to-date.
- Use images and diagrams to enhance understanding.
- Emphasize why something is done, not just how.
- Always end the generated markdown with a single empty line inside the file content itself. Never try to add it by running commands like echo or printf — it must be part of the markdown text you output.
- Never use shell commands or external tools to modify files after writing them. Everything, including the trailing newline, must be produced as part of the file’s content.
- Every folder should have its own `toc.yml` file to define the structure of the documentation within that folder.
- When linking to a folder in a `toc.yml` file, link to the `toc.yml` file in that folder, not to an `index.md` file.
- Ensure that documentation is accurate according to the public APIs and features of the project.