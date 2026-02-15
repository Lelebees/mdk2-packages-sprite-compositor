# Mal.MdkScriptMixin.Graphics

> [!NOTE]
> **Version:** 1.0.1  
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

// Lines
dc.Line(paint, from, to, thickness);
dc.Line(paint, from, to, thickness, capped: true);  // With rounded end caps

// Text
var font = dc.GetFont("Debug");  // or "Monospace"
dc.Text(font, paint, "Hello", new Vector2(x, y), 20f);
dc.Text(font, paint, "Title", new Vector2(x, y), 24f, TextAlignment.CENTER);
var size = dc.MeasureText(font, "Hello", 20f);  // Returns virtual pixel size
```

## Transforms

```csharp
// Translate, scale, rotate
dc.Transform = Transform.Identity.Translate(new Vector2(100, 50));
dc.Transform = Transform.Identity.Scale(2f);
dc.Transform = Transform.Identity.Rotate(MathHelper.PiOver4);

// Chain transforms
dc.Transform = Transform.Identity.Translate(center).Rotate(angle).Scale(scale);

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

## Performance Tips

1. **Reuse paints** - Create once in constructor, reuse in Main loop
2. **Reuse fonts** - Get once and store as field
3. **Use Drawing class** - Cache static graphics
4. **One BeginDraw per frame** - Batch all drawing
5. **Use Update10** - Usually sufficient for UI updates

## API Reference

### SurfaceDc
- `Create(IMyTextSurface)` - Simple context without aspect handling
- `CreateWithAspect(surface, mode, ref viewport, out physical)` - With aspect handling
- `BeginDraw()` / `EndDraw()` - Frame lifecycle (use `using` pattern)
- `Rect(paint, rect)` - Draw filled rectangle
- `Line(paint, from, to, thickness, capped)` - Draw line
- `Text(font, paint, text, pos, size, alignment)` - Draw text
- `Sprite(texture, color, rect, rotation)` - Draw sprite
- `MeasureText(font, text, size)` - Measure text in virtual pixels
- `GetFont(name)` - Get font ("Debug" or "Monospace")
- `Transform` - Current transform applied to all drawing
- `ClipRect` - Clipping rectangle in physical pixels
- `Push()` / `Pop()` - Save/restore state
- `BeginCapture()` / `EndCapture()` - Capture sprites for replay

### Transform
- `Identity` - No transformation
- `Translate(offset)`, `Scale(scale)`, `Rotate(radians)` - Build transforms
- `operator *` - Combine transforms
- `TransformPoint(point)` - Apply to point
- `TransformVector(vector)` - Apply to direction (no translation)
- `TransformRectCenter(rect)` - Transform rectangle by center
- `InverseTransformPoint(point)` - Reverse transform

### Drawing
- `Draw(dc, force, drawAction)` - Draw cached or regenerate
- `Invalidate()` - Mark for regeneration


---

*Documentation auto-generated from package metadata. Last updated: 2026-02-11*
