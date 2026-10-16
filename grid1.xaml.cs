namespace Grids2_JAPD;

public partial class grid1 : ContentPage
{
    private string _currentInput = string.Empty;

    public grid1()
    {
        InitializeComponent();
        AsignarEventosBotones(this);
    }


    private void AsignarEventosBotones(object contenedor)
    {
        if (contenedor is Microsoft.Maui.Controls.Layout layout)
        {
            foreach (var hijo in layout.Children)
            {
                if (hijo is Button boton)
                {
                    boton.Clicked += OnButtonClicked;
                }
                else
                {
                    AsignarEventosBotones(hijo);
                }
            }
        }
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (sender is Button boton)
        {
            string texto = boton.Text;

        
            if (texto == "AC")
            {
                _currentInput = string.Empty;
                ActualizarPantalla();
            }
        
            else if (texto == "=")
            {
                CalcularResultado();
            }
    
            else
            {
                _currentInput += texto;
                ActualizarPantalla();
            }
        }
    }

    private void ActualizarPantalla()
    {
        
        if (Content is VerticalStackLayout vsl && vsl.Children.Count > 0 && vsl.Children[0] is Entry entry)
        {
            entry.Text = _currentInput;
        }
    }

    private void CalcularResultado()
    {
        try
        {
        
            var dt = new System.Data.DataTable();

           
            string expresionFormateada = _currentInput.Replace("x", "*");

            var resultado = dt.Compute(expresionFormateada, string.Empty);

            if (resultado != null)
            {
                _currentInput = resultado.ToString();
                ActualizarPantalla();
            }
        }
        catch (Exception)
        {
            _currentInput = "Error";
            ActualizarPantalla();
           
            _currentInput = string.Empty;
        }
    }
}