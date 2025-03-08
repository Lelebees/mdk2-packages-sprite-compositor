# IonDisplay Graphics Library


## Warning
Being an experiment and a learning experience, this is a work in progress and may change significantly in
the future.

I will always be looking for ways to optimize and improve the code.

> Note that being a nuget package, you can stick to a specific version of this library if you don't want
> your scripts to break when I make changes.


## Introduction
IonDisplay is a graphics library for Space Engineers designed to simplify the process of drawing text and 
sprites on LCD panels, enabling you to create more complex and visually appealing displays with less effort.


## Features

Easily draw text and sprites on the screen using an MVU-inspired architecture.

```csharp
    public class CoolPage: Page<Program>
    {
        protected override View Render(IMyTextSurface surface, Program model, RectangleF viewport)
        {
            // Creates a view box that is 100x100 pixels in size, but will be scaled to fit the 
            // surface while maintaining the aspect ratio.
            // Centers the text "Hello World" in the view box.
            
            return ViewBox(100, 100)
                .Add(
                    Text("Hello World", Color.White)
                        .CenteredAt(50, 50)
                        .Font("Monospace")
                        .FontSize(2)
                );
        }
    }
```

## Usage
_To be written... Sorry_