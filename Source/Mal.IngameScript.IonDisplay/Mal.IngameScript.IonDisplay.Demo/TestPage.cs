namespace IngameScript
{
    public class TestPage : Page<Program>
    {
        protected override View Render(IIon ion, Program model) =>
            ion.Frame()
                .Add(ion.Text("Hello, World!", ion.Theme.Fg).CenteredAt(50, 50));
    }
}