# Sprite Compositing for LCD Scripts

An API wrapper that supports composing new sprites from multiple existing ones.

- [Source Code](https://github.com/malforge/mdk2-packages/tree/main/libraries/Lelebees.MdkScriptMixin.SpriteCompositor/Lelebees.MdkScriptMixin.SpriteCompositor/src)
- [Demo Project](https://github.com/malforge/mdk2-packages/blob/main/libraries/Lelebees.MdkScriptMixin.SpriteCompositor/SpriteCompositor.Demo/Program.cs)
- [Tests](https://github.com/malforge/mdk2-packages/tree/main/libraries/Lelebees.MdkScriptMixin.SpriteCompositor/SpriteCompositor.Test)

## Usage

You can instantiate regular sprites using the helper methods of the `Sprites` class. These return a builder (except for
`Sprites.Group()`, which returns a group directly) that can be used to further configure the initial values of the
sprite.

All sprites and sprite groups implement the `Sprite` interface, which you can use to `Translate()`, `Rotate()`, or
`Scale()` a sprite after instantiation, among other things. Individual Sprite types may have additional properties and
methods available, like `TextureSprite`'s `Mirror()` functions.

See the [demo project](https://github.com/malforge/mdk2-packages/blob/main/libraries/Lelebees.MdkScriptMixin.SpriteCompositor/SpriteCompositor.Demo/Program.cs) for a detailed example.

### Grouping Sprites

You can group sprites by calling `Sprites.Group()` or creating a `new SimpleSpriteGroup()`.

If this implementation does
not fit your needs, you can extend the `SpriteGroup` abstract class. If you do, you will be required to implement the
`Clone()` and `GetChildren()` methods.

**Avoid creating a new list in `GetChildren()`**, as the function is invoked
whenever a transformation is applied to the group. I recommend using the following example as a guide:

```csharp
public class CustomSpriteGroup : SpriteGroup
{
    private readonly List<Sprite> backingList;
        
    public Sprite UnspecifiedSprite 
    {
        get { return backingList[0]; }
        set { backingList[0] = value; }
    }
        
    public TextureSprite SpecificSprite 
    {
        get { return (TextureSprite) backingList[1]; }
        set { backingList[1] = value; }
    }
        
    public CustomSpriteGroup(Sprite unspecifiedSprite, TextureSprite specificSprite)
    {
        this.backingList = new List<Sprite> { unspecifiedSprite, specificSprite };
    }
        
    public override Sprite Clone() => new CustomSpriteGroup(backingList.Select(sprite => sprite.Clone()).ToList());
    
    protected override List<Sprite> GetChildren() => backingList;
}
```

If, for some reason, the abstract `SpriteGroup` class also does not suit your needs, you can implement the `Sprite`
interface directly, though if you feel the need to do so, there may be a structural problem with your program.

### Anchors

Some transformation functions take an optional `Anchor` argument. When passed, the transformation will use the anchor as
the center of that transformation. For example, with `Rotate()` transformations, in addition to the regular behavior,
the respective sprite will be rotated around the anchor point, as if it was orbiting it. `Scale()` transformations with
an `Anchor` will also scale the distance to the anchor point.

### Displaying Sprites

In order to draw your composed sprites to an LCD screen, you'll need to call the `Sprite.AsRenderable()` method, which will
return an array of `MySprite` objects that your sprite consists of. You can draw these to the screen in one go
using the `MySpriteDrawFrame.AddRange()` method. `AsRenderable()` takes an optional `RectangleF viewport` as parameter.
Supplying this will move the sprites so that (0,0) is the center of the viewport.

## Legal
`Copyright (c) 2026 Lelebees`

This program is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.

### License
You can find a copy of the [GNU General Public License](https://github.com/malforge/mdk2-packages/blob/main/libraries/Lelebees.MdkScriptMixin.SpriteCompositor/Lelebees.MdkScriptMixin.SpriteCompositor/COPYING) and [GNU Lesser General Public License](https://github.com/malforge/mdk2-packages/blob/main/libraries/Lelebees.MdkScriptMixin.SpriteCompositor/Lelebees.MdkScriptMixin.SpriteCompositor/COPYING.LESSER) next to this source code in COPYING and COPYING.LESSER respectively.

### Reaching out
You can reach me as @lelebees on Discord, or through the project's [Github Repository](https://github.com/Lelebees/mdk2-packages-sprite-compositor). Please note while reaching out on Discord that I generally do not accept random friend requests. @Mention me in the [programmable block channel](https://discord.com/channels/125011928711036928/216219467959500800) of the Keen Software House Discord Server to get a hold of me.