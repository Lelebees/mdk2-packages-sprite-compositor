# ReadMe

The console creates a simple diagnostic display for the PB's primary
display, which is also more or les aesthetic. It is a simple console
that can be used to display information about your script.

## Usage
A script contains (up to) 3 callbacks the game uses to call your script:

- `Program`: The constructor for the script
- `Main`: The main loop of the script
- `Save`: Lets the script save its state

In order to safely use Console you need to wrap the contents of these:

```csharp
public Program()
{
    // Your constructor code here
    Console = new Console(this);
}
