# Mal.MdkScriptMixin.Graphics

> [!NOTE]
> **Version:** 2.0.3  
> **Authors:** Malware  
> **Package:** `Mal.MdkScriptMixin.Graphics`
> 
> **Description:** A graphics library for Space Engineers providing high-level drawing abstractions, aspect-aware viewports, and an advanced paint system for creating responsive UIs.
>
> **[Installation Instructions →](./Mal.MdkScriptMixin.Graphics-Installation.md)** | **[Release Notes →](./Mal.MdkScriptMixin.Graphics-ReleaseNotes.md)**

---
# Graphics Library for Space Engineers

A comprehensive graphics library for drawing on LCD panels in Space Engineers. Provides high-level drawing abstractions, aspect-aware viewports, coordinate transformations, and an advanced paint system - eliminating the need to manually manage sprites and making it easy to create UIs that work consistently across different display sizes.

## Quickstart

```csharp
// 1. Create a drawing context with virtual coordinates
var viewport = new RectangleF(0, 0, 512, 512);
var dc = SurfaceDc.CreateWithAspect(surface, AspectMode.Fit, ref viewport, out _);

// 2. Create reusable paints
var paint = new Paint(Color.White);
var font = dc.GetFont("Debug");

// 3. Draw with simple method calls - no sprite manipulation!
using (dc.BeginDraw())
{
    dc.Rect(paint, new RectangleF(50, 50, 100, 50));
    dc.Text(font, paint, "Hello!", new Vector2(50, 120), 20f);
}
```

**See the demo project for a complete working example.**

## Why Use This Library?

**The Problem:**
- Manually creating and positioning `MySprite` objects is tedious and error-prone
- Different LCD panels have different resolutions and aspect ratios
- Code that works on one display may look stretched, cropped, or misaligned on another
- Managing sprite lifecycles and frame submission requires boilerplate

**The Solution:**
- **Drawing Abstractions** - Use `Rect()`, `Line()`, `Text()` instead of managing sprites
- **Virtual coordinate systems** - Define your UI once, works on all displays
- **Automatic scaling** - Content scales to fit without distortion  
- **Aspect ratio handling** - Letterboxing/pillarboxing handled automatically
- **Advanced paint system** - Theme-adaptive colors with procedural paints
- **Transform stack** - Nested coordinate systems with push/pop

## Core Concepts

### Virtual Coordinates

Draw using a consistent virtual coordinate system - the library handles scaling to physical displays:

```csharp
var viewport = new RectangleF(0, 0, 512, 512);
var dc = SurfaceDc.CreateWithAspect(surface, AspectMode.Fit, ref viewport, out _);

dc.Rect(paint, new RectangleF(50, 50, 100, 50));  // Always at same relative position
dc.Text(font, paint, "Title", new Vector2(50, 120), 20f);
```

### Paint System

**Create paints once and reuse them:**

```csharp
// In constructor
Paint _whitePaint = new Paint(Color.White);
ForegroundPaint _fgPaint = new ForegroundPaint(surface);
AccentPaint _accentPaint = new AccentPaint(_fgPaint);

// In Main loop - reuse
dc.Rect(_whitePaint, rect);
dc.Text(font, _fgPaint, "Text", pos, size);
```

**Paint Types:**
- **Static:** `Paint(Color)`, `BackgroundPaint(surface)`, `ForegroundPaint(surface)`
- **Procedural:** Adapt to user's theme colors
  - `AccentPaint(basePaint)` - Complementary accent color
  - `BackdropPaint(basePaint)` - Muted background
  - `SemanticPaint(basePaint, semanticColor)` - Error/warning/info (use `SemanticPaint.Error`, `.Warning`, `.Info`)
  - `ContrastStepPaint(basePaint, amount)` - Auto-contrast adjustment

### BeginDraw Pattern

```csharp
using (dc.BeginDraw())
{
    dc.Rect(paint, rect);
    dc.Text(font, paint, "Hello", pos, 20f);
} // Frame automatically submitted
```

## Aspect Modes

Controls how virtual viewport maps to physical display. **All modes maintain aspect ratio** - coordinates are never distorted.

- **Fit** - Scales to fit, adds letterboxing if needed (viewport stays 512x512)
- **Fill** - Scales to fill, crops edges if needed (viewport stays 512x512)
- **Native** - No scaling, 1:1 pixel mapping (viewport stays 512x512)
- **FitAndExpand** - Scales to fit, expands viewport to available space (modifies viewport to e.g., 512x384)
- **FillAndContract** - Scales to fill, contracts viewport to visible area (modifies viewport to e.g., 384x512)

```csharp
var viewport = new RectangleF(0, 0, 512, 512);
var dc = SurfaceDc.CreateWithAspect(surface, AspectMode.FitAndExpand, ref viewport, out _);
Echo($"Available space: {viewport.Width}x{viewport.Height}");
```

## Drawing Operations

```csharp
// Rectangles and sprites
dc.Rect(paint, new RectangleF(x, y, width, height));
dc.Sprite("CircleHollow", Color.White, new RectangleF(x, y, 50, 50));

// Lines with customizable cap texture
dc.Line(paint, from, to, thickness);
dc.Line(paint, from, to, thickness, capped: true);  // With SemiCircle caps
dc.Line(paint, from, to, thickness, capped: true, capTexture: "Circle");  // Custom cap

// Text
var font = dc.GetFont("Debug");  // or "Monospace"
dc.Text(font, paint, "Hello", new Vector2(x, y), 20f);
dc.Text(font, paint, "Title", new Vector2(x, y), 24f, TextAlignment.CENTER);

// Text with measurement (sets sprite.Size for layout calculations)
dc.Text(font, paint, "Measured", new Vector2(x, y), 20f, TextAlignment.LEFT, measure: true);

// Measure text (returns virtual pixel size)
var size = font.MeasureText("Hello", 20f);
```

## Transforms

```csharp
// Translate, scale, rotate
dc.Transform = Transform.Identity.Translate(new Vector2(100, 50));
dc.Transform = Transform.Identity.WithScale(2f);
dc.Transform = Transform.Identity.WithRotation(MathHelper.PiOver4);

// Chain transforms
dc.Transform = Transform.Identity.Translate(center).WithRotation(angle).WithScale(scale);

// Push/Pop for nested transforms
using (dc.Push())
{
    dc.Transform = dc.Transform.Translate(new Vector2(100, 100));
    DrawWidget();
} // Transform and clip automatically restored
```

## Clipping

```csharp
dc.ClipRect = new RectangleF(50, 50, 400, 400);  // In physical pixels
dc.Rect(paint, new RectangleF(0, 0, 500, 500));  // Only visible portion renders
```

## Sprite Caching

Cache static graphics that don't change every frame:

```csharp
Drawing _logo = new Drawing();

void DrawLogo(IDc dc)
{
    dc.Rect(paint1, rect1);
    dc.Line(paint2, from, to, 2f);
    dc.Text(font, paint3, "LOGO", pos, 24f);
}

using (dc.BeginDraw())
{
    _logo.Draw(dc, false, DrawLogo);  // Cached after first draw
}
```

### Using DC Without Rendering

You can use the drawing context for measurements and sprite capture without rendering to the surface:

```csharp
// Capture sprites without rendering to display
var capturedSprites = new List<MySprite>();
using (dc.BeginDraw(render: false))
using (dc.BeginCapture(capturedSprites))
{
    dc.Rect(paint, rect);
    dc.Text(font, paint, "Hidden", pos, 20f, measure: true);
}
// Now you can inspect or manipulate capturedSprites

// Or render the captured sprites later
using (dc.BeginDraw())
{
    dc.AddSprites(capturedSprites);
}
```

## Performance Tips

1. **Reuse paints** - Create once in constructor, reuse in Main loop
2. **Reuse fonts** - Get once and store as field
3. **Use Drawing class** - Cache static graphics
4. **One BeginDraw per frame** - Batch all drawing
5. **Use Update10** - Usually sufficient for UI updates

## API Reference

### SurfaceDc

**Static Factory Methods:**
- `static SurfaceDc Create(IMyTextSurface surface)`
  - Creates a simple drawing context without aspect handling
  - Returns: New SurfaceDc instance

- `static SurfaceDc CreateWithAspect(IMyTextSurface surface, AspectMode mode, ref RectangleF virtualViewport, out RectangleF physicalViewport)`
  - Creates drawing context with aspect ratio handling and coordinate transformation
  - Parameters:
    - `surface` - Text surface to draw on
    - `mode` - How to handle aspect ratio (see AspectMode)
    - `virtualViewport` - Desired virtual coordinate system (may be modified by FitAndExpand/FillAndContract)
    - `physicalViewport` - Output parameter containing physical display viewport
  - Returns: New SurfaceDc instance with transform applied

**Frame Management:**
- `IDisposable BeginDraw(bool render = true)`
  - Starts a drawing context (can be nested)
  - Parameters:
    - `render` - If true (default), renders sprites to surface. If false, allows drawing operations and sprite capture without rendering
  - Returns: IDisposable that calls EndDraw when disposed
  - Usage: Always use with `using` statement for rendering
  - Note: Font measurements (MeasureText) do not require BeginDraw

- `void EndDraw()`
  - Ends drawing context and submits sprites to surface (if render was true)
  - Automatically called when BeginDraw's IDisposable is disposed

**Drawing Methods:** (All return `IDc` for method chaining)
- `IDc Rect(IPaint paint, RectangleF rect)`
  - Draws a filled rectangle
  - Skips drawing if paint alpha is 0

- `IDc Line(IPaint paint, Vector2 from, Vector2 to, float thickness, bool capped = false, string capTexture = "SemiCircle")`
  - Draws a line between two points
  - Parameters:
    - `capped` - If true, adds rounded end caps
    - `capTexture` - Sprite to use for caps (default: "SemiCircle", alternative: "Circle")
  - Skips drawing if paint alpha is 0 or thickness <= 0

- `IDc Text(IFont font, IPaint paint, string text, Vector2 position, float sizePx, TextAlignment alignment = TextAlignment.LEFT, bool measure = false)`
  - Draws text at specified position
  - Parameters:
    - `font` - Font to use (get from GetFont)
    - `sizePx` - Font size in virtual pixels
    - `alignment` - LEFT, CENTER, or RIGHT
    - `measure` - If true, measures text and sets sprite.Size (useful for layout)
  - Skips drawing if paint alpha is 0 or text is null/empty

- `IDc Sprite(string texture, Color color, RectangleF rect, float rotation = 0f)`
  - Draws a textured sprite
  - Parameters:
    - `texture` - Sprite name (e.g., "Circle", "SquareSimple")
    - `rotation` - Rotation in radians
  - Skips drawing if color alpha is 0

**Font Management:**
- `IFont GetFont(string fontName)`
  - Retrieves font by name
  - Common names: "Debug", "Monospace"
  - Returns: IFont instance or null if not found
  - Fonts are auto-loaded when SurfaceDc is created

**State Management:**
- `IDisposable Push()`
  - Saves current Transform and ClipRect to state stack
  - Returns: IDisposable that calls Pop when disposed
  - Usage: Use with `using` statement for automatic restore

- `void Pop()`
  - Restores Transform and ClipRect from state stack
  - Automatically called when Push's IDisposable is disposed

**Sprite Capture:**
- `IDisposable BeginCapture(List<MySprite> targetList)`
  - Starts capturing sprites to a list
  - Parameters:
    - `targetList` - List to capture sprites into (cleared before capture)
  - Returns: IDisposable that calls EndCapture when disposed
  - Usage: Use with `using` statement
  - Note: Captured sprites are ALSO rendered if BeginDraw(render: true) is active

- `void EndCapture()`
  - Stops capturing sprites
  - Automatically called when BeginCapture's IDisposable is disposed

**Manual Sprite Control:**
- `void AddSprite(ref MySprite sprite)`
  - Manually adds a sprite to the frame and any active captures

- `void AddSprites(List<MySprite> sprites)`
  - Manually adds multiple sprites to the frame and any active captures

**Properties:**
- `Transform Transform { get; set; }`
  - Current coordinate transformation applied to all drawing operations
  - Default: Transform.Identity

- `RectangleF ClipRect { get; set; }`
  - Clipping rectangle in physical pixels
  - Only sprites within this rectangle are rendered
  - Emits clip sprite command when changed

---

### IFont

**Properties:**
- `string Name { get; }`
  - Font name (e.g., "Debug", "Monospace")

**Methods:**
- `Vector2 MeasureText(string text, float sizePx)`
  - Measures text dimensions in virtual pixels
  - Parameters:
    - `text` - String to measure
    - `sizePx` - Font size in virtual pixels
  - Returns: Size in virtual coordinate space

- `Vector2 MeasureText(StringSegment text, float sizePx)`
  - Measures text substring dimensions in virtual pixels
  - Parameters:
    - `text` - String segment to measure
    - `sizePx` - Font size in virtual pixels
  - Returns: Size in virtual coordinate space

- `float ScaleToPx(float scale)`
  - Converts font scale value to pixel size
  - Returns: Pixel size

- `float PxToScale(float px)`
  - Converts pixel size to font scale value
  - Returns: Font scale

---

### Transform

**Struct Properties:**
- `Vector2 Translation` - Translation offset (readonly)
- `float Rotation` - Rotation in radians (readonly)
- `float Scale` - Uniform scale factor (readonly)

**Static Fields:**
- `static readonly Transform Identity`
  - Identity transform (zero translation, zero rotation, scale 1)

**Builder Methods:**
- `Transform Translate(Vector2 offset)`
  - Returns new transform with added translation offset

- `Transform WithTranslation(Vector2 translation)`
  - Returns new transform with absolute translation

- `Transform WithRotation(float rotation)`
  - Returns new transform with absolute rotation in radians

- `Transform WithScale(float scale)`
  - Returns new transform with absolute scale

- `Transform WithoutTranslation()`
  - Returns new transform with translation set to zero

- `Transform WithoutRotation()`
  - Returns new transform with rotation set to zero

- `Transform WithoutScale()`
  - Returns new transform with scale set to 1

**Transform Operations:**
- `Vector2 TransformPoint(Vector2 p)`
  - Applies transform to a point (scale, rotate, translate)
  - Returns: Transformed point

- `Vector2 TransformVector(Vector2 v)`
  - Applies transform to a direction vector (scale and rotate, no translation)
  - Returns: Transformed vector

- `RectangleF TransformRectCenter(RectangleF r)`
  - Transforms rectangle by its center point (used for sprite positioning)
  - Returns: Transformed rectangle

- `RectangleF TransformAabb(RectangleF r)`
  - Transforms all four corners and returns axis-aligned bounding box
  - Returns: AABB containing transformed rectangle

- `Vector2 InverseTransformPoint(Vector2 p)`
  - Applies inverse transform to a point (physical to virtual)
  - Throws: InvalidOperationException if Scale is 0
  - Returns: Inverse-transformed point

- `Transform Inverse()`
  - Returns the inverse of this transform
  - Throws: InvalidOperationException if Scale is 0
  - Returns: Inverse transform

**Operators:**
- `static Transform operator *(Transform parent, Transform child)`
  - Combines two transforms: applies child transform, then parent
  - Example: `parentTransform * childTransform`
  - Returns: Combined transform

---

### Drawing

**Properties:**
- `IReadOnlyList<MySprite> Sprites { get; }`
  - Read-only access to cached sprite list

**Methods:**
- `void Draw(IDc dc, bool force, Action<IDc> drawAction)`
  - Draws cached sprites, or regenerates if dirty or forced
  - Parameters:
    - `dc` - Drawing context to draw to
    - `force` - If true, regenerates cache even if not dirty
    - `drawAction` - Action that performs drawing operations
  - Behavior: Captures sprites on first draw or when dirty, replays on subsequent draws

- `void Invalidate()`
  - Marks cache as dirty, will regenerate on next Draw call

---

### Paint Classes

**Static Paints:**

- `Paint(Color color)`
  - Fixed color paint
  - Constructor parameter: `color` - Color to use

- `BackgroundPaint(IMyTextSurface surface)`
  - Uses surface's ScriptBackgroundColor
  - Constructor parameter: `surface` - Text surface to read color from

- `ForegroundPaint(IMyTextSurface surface)`
  - Uses surface's ScriptForegroundColor
  - Constructor parameter: `surface` - Text surface to read color from

**Procedural Paints:** (Colors computed dynamically from base paint)

- `AccentPaint(IPaint basePaint)`
  - Complementary accent color (opposite hue)
  - Constructor parameter: `basePaint` - Base paint to derive color from

- `BackdropPaint(IPaint basePaint)`
  - Muted, desaturated background color
  - Constructor parameter: `basePaint` - Base paint to derive color from

- `SemanticPaint(IPaint basePaint, Color semanticColor)`
  - Error/warning/info colors that adapt to base paint
  - Constructor parameters:
    - `basePaint` - Base paint for context
    - `semanticColor` - Semantic color (use static constants)
  - Static constants:
    - `SemanticPaint.Error` - Red error color
    - `SemanticPaint.Warning` - Yellow warning color
    - `SemanticPaint.Info` - Blue info color

- `ContrastStepPaint(IPaint basePaint, float amount)`
  - Automatically lightens or darkens for contrast
  - Constructor parameters:
    - `basePaint` - Base paint to adjust
    - `amount` - Adjustment amount (positive lightens, negative darkens)

**All Paint Classes Implement:**
- `Color Color { get; }` - Current color value
- `string Texture { get; }` - Texture name (typically "SquareSimple")

---

### AspectMode (Enum)

- `Native`
  - 1:1 pixel mapping, no scaling
  - Virtual viewport dimensions unchanged

- `Fit`
  - Scales uniformly to fit within display
  - Maintains aspect ratio with letterboxing/pillarboxing if needed
  - Virtual viewport dimensions unchanged

- `Fill`
  - Scales uniformly to fill entire display
  - Maintains aspect ratio, may crop edges
  - Virtual viewport dimensions unchanged

- `FitAndExpand`
  - Scales to fit, then expands virtual viewport to use all available space
  - No letterboxing - viewport dimensions modified to match display aspect
  - Virtual viewport parameter is modified

- `FillAndContract`
  - Scales to fill, then contracts virtual viewport to visible area only
  - No cropping - viewport dimensions modified to show only visible area
  - Virtual viewport parameter is modified


---

*Documentation auto-generated from package metadata. Last updated: 2026-02-20*
