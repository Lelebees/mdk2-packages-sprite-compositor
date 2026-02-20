# Release Notes - Mal.MdkScriptMixin.Graphics

````````text
2.0.3
- Fixed AlphaPaint using the wrong data type for alpha value

2.0.2
- Added AlphaPaint for altering the alpha value of an existing paint

2.0.1
- Fixed AddSprites taking a list instead of an IReadOnlyList

2.0.0
- BREAKING CHANGES: MeasureText moved from IDc to IFont interface (call font.MeasureText() instead of dc.MeasureText())
- Added BeginDraw(bool render) parameter to control whether sprites are rendered to surface
- Added measure parameter to Text() method to set sprite.Size for convenience
- Added StringSegment overload to IFont.MeasureText()
- Added Sprites property to Drawing class for read-only access to cached sprite list
- Comprehensive API documentation in readme

1.0.1
- Fixed MeasureText to return virtual pixels instead of physical pixels
- Removed XML documentation comments to reduce script size
- Updated demo to use CreateWithAspect
- Improved readme documentation

1.0.0
- Initial release
````````

---

*Release notes auto-generated from `_releasenotes` file. Last updated: 2026-02-20*
