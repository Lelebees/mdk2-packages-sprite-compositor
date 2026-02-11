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

### Device Context (SurfaceDc)

Manages sprite frames, coordinate transforms, fonts, and clipping:

```csharp
// Always use CreateWithAspect for production code
var viewport = new RectangleF(0, 0, 512, 512);
var dc = SurfaceDc.CreateWithAspect(surface, AspectMode.Fit, ref viewport, out _);
```

### Virtual Coordinates

Draw using your virtual viewport coordinates - the library handles scaling:

```csharp
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

**Static Paints:**
- `Paint(Color)` - Fixed color
- `BackgroundPaint(surface)` - Surface's background color
- `ForegroundPaint(surface)` - Surface's foreground color

**Procedural Paints** (adapt to theme):
- `AccentPaint(basePaint)` - Complementary accent color
- `BackdropPaint(basePaint)` - Muted background
- `SemanticPaint(basePaint, semanticColor)` - Error/warning/info colors
  - `SemanticPaint.Error`, `SemanticPaint.Warning`, `SemanticPaint.Info`
- `ContrastStepPaint(basePaint, amount)` - Auto-contrast adjustment

### BeginDraw/EndDraw Pattern

```csharp
using (dc.BeginDraw())
{
    dc.Rect(paint, rect);
    dc.Text(font, paint, "Hello", pos, 20f);
} // Frame automatically submitted
```

## Aspect Modes

Controls how virtual viewport maps to physical display. **All modes maintain aspect ratio** - coordinates are never distorted.

**AspectMode.Fit** - Scales uniformly to fit, adds letterboxing if needed
```csharp
var viewport = new RectangleF(0, 0, 512, 512);
var dc = SurfaceDc.CreateWithAspect(surface, AspectMode.Fit, ref viewport, out _);
// Your 512x512 stays 512x512. Adds black bars if display aspect differs.
```

**AspectMode.Fill** - Scales uniformly to fill, crops edges if needed
```csharp
var viewport = new RectangleF(0, 0, 512, 512);
var dc = SurfaceDc.CreateWithAspect(surface, AspectMode.Fill, ref viewport, out _);
// Your 512x512 stays 512x512. Some content may be off-screen if display aspect differs.
```

**AspectMode.Native** - No scaling, 1:1 pixel mapping
```csharp
var viewport = new RectangleF(0, 0, 512, 512);
var dc = SurfaceDc.CreateWithAspect(surface, AspectMode.Native, ref viewport, out _);
// Pixel-perfect, but UI won't adapt to different display sizes
```

**AspectMode.FitAndExpand** - Scales to fit, expands viewport to use available space
```csharp
var viewport = new RectangleF(0, 0, 512, 512);
var dc = SurfaceDc.CreateWithAspect(surface, AspectMode.FitAndExpand, ref viewport, out _);
// viewport dimensions modified to match display's aspect (e.g., 512x384 or 682x512)
// No letterboxing - you get more coordinate space to use
Echo($"Available space: {viewport.Width}x{viewport.Height}");
```

**AspectMode.FillAndContract** - Scales to fill, contracts viewport to visible area
```csharp
var viewport = new RectangleF(0, 0, 512, 512);
var dc = SurfaceDc.CreateWithAspect(surface, AspectMode.FillAndContract, ref viewport, out _);
// viewport dimensions modified to show only visible area (e.g., 384x512 or 512x682)
// No cropping - viewport shows exactly what's on screen
Echo($"Visible area: {viewport.Width}x{viewport.Height}");
```

## Drawing Operations

### Rectangles
```csharp
dc.Rect(paint, new RectangleF(x, y, width, height));
dc.Sprite("CircleHollow", Color.White, new RectangleF(x, y, 50, 50));
```

### Lines
```csharp
dc.Line(paint, from, to, thickness);
dc.Line(paint, from, to, thickness, capped: true);  // With end caps
```

### Text
```csharp
var font = dc.GetFont("Debug");  // or "Monospace"
dc.Text(font, paint, "Hello", new Vector2(x, y), 20f);
dc.Text(font, paint, "Title", new Vector2(x, y), 24f, TextAlignment.CENTER);
dc.Text(font, paint, "Right", new Vector2(x, y), 18f, TextAlignment.RIGHT);
```

### Measuring Text
```csharp
var size = dc.MeasureText(font, "Hello", 20f);  // Returns physical pixel size
```

## Transforms

```csharp
// Translate
dc.Transform = Transform.Identity.Translate(new Vector2(100, 50));
dc.Rect(paint, new RectangleF(0, 0, 50, 50)); // Draws at (100, 50)

// Scale
dc.Transform = Transform.Identity.Scale(2f);

// Rotate (radians)
dc.Transform = Transform.Identity.Rotate(MathHelper.PiOver4);

// Chain
dc.Transform = Transform.Identity.Translate(center).Rotate(angle).Scale(scale);

// Push/Pop for nested transforms
using (dc.Push())
{
    dc.Transform = dc.Transform.Translate(new Vector2(100, 100));
    DrawWidget();
} // Transform restored
```

## Clipping

```csharp
dc.ClipRect = new RectangleF(50, 50, 400, 400);
dc.Rect(paint, new RectangleF(0, 0, 500, 500));  // Only visible portion renders

// Clipping also saved with Push/Pop
using (dc.Push())
{
    dc.ClipRect = new RectangleF(100, 100, 200, 200);
}
```

## Sprite Caching

For static graphics that don't change every frame:

```csharp
Drawing _logo = new Drawing();

void DrawLogo(IDc dc)
{
    dc.Rect(paint1, rect1);
    dc.Line(paint2, from, to, 2f);
    dc.Text(font, paint3, "LOGO", pos, 24f);
}

// In Main loop
using (dc.BeginDraw())
{
    _logo.Draw(dc, false, DrawLogo);  // Cached after first draw
}
```

## Performance Tips

1. **Reuse paints** - Create once, reuse in Main loop
2. **Reuse fonts** - Get once and store
3. **Use Drawing class** - Cache static graphics
4. **One BeginDraw per frame** - Batch all drawing
5. **Use Update10** - Usually sufficient for UI

## API Reference

### SurfaceDc
- `Create(IMyTextSurface)` - Simple context
- `CreateWithAspect(surface, mode, ref viewport, out physical)` - With aspect handling
- `BeginDraw()` - Start frame
- `Rect(IPaint, RectangleF)` - Draw rectangle
- `Line(IPaint, Vector2, Vector2, float)` - Draw line
- `Text(IFont, IPaint, string, Vector2, float, TextAlignment)` - Draw text
- `Sprite(string, Color, RectangleF, float)` - Draw sprite
- `MeasureText(IFont, string, float)` - Measure text
- `GetFont(string)` - Get font by name
- `Transform { get; set; }` - Current transform
- `ClipRect { get; set; }` - Clipping rectangle
- `Push()` - Save state
- `Pop()` - Restore state

### Paint Classes
**Static:** `Paint(Color)`, `BackgroundPaint(surface)`, `ForegroundPaint(surface)`

**Procedural:** `AccentPaint(base)`, `BackdropPaint(base)`, `SemanticPaint(base, color)`, `ContrastStepPaint(base, amount)`

### Drawing
- `Draw(IDc, bool force, Action<IDc>)` - Draw cached or regenerate
- `Invalidate()` - Mark for regeneration

### Transform
- `Identity` - Identity transform
- `Translate(Vector2)`, `Scale(float)`, `Rotate(float)` - Add transforms
- `operator *(Transform, Transform)` - Combine
- `TransformPoint(Vector2)`, `TransformRectCenter(RectangleF)` - Apply

### AspectMode
`Native`, `Fit`, `Fill`, `FitAndExpand`, `FillAndContract`
