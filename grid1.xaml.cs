
using System;
using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;

namespace Grids2_JAPD;

public partial class grid1 : ContentPage
{
    private string _currentInput = string.Empty;
    private bool _resultadoMostrado = false;

    public grid1()
    {
        InitializeComponent();
    }

    private void OnButtonClicked(object sender, EventArgs e)
    {
        if (sender is not Button boton)
            return;

        string texto = boton.Text;


        if (texto == "AC")
        {
            _currentInput = string.Empty;
            _resultadoMostrado = false;
            ActualizarPantalla();
            return;
        }

        if (texto == "⌫")
        {
            _resultadoMostrado = false;

            if (_currentInput.Length > 0)
                _currentInput = _currentInput[..^1];

            ActualizarPantalla();
            return;
        }

        if (texto == "=")
        {
            CalcularResultado();
            return;
        }

     
        if (texto == ".")
        {
            if (_resultadoMostrado)
            {
                _currentInput = string.Empty;
                _resultadoMostrado = false;
            }

            string numero = ObtenerNumeroActual();

            if (numero.Contains("."))
                return;

            if (_currentInput.Length == 0 ||
                EsOperador(_currentInput[^1]) ||
                _currentInput[^1] == '%')
            {
                _currentInput += "0.";
            }
            else
            {
                _currentInput += ".";
            }

            ActualizarPantalla();
            return;
        }


        if (texto == "%")
        {
            if (_resultadoMostrado)
                return;

            if (_currentInput.Length > 0 &&
                char.IsDigit(_currentInput[^1]))
            {
                _currentInput += "%";
                ActualizarPantalla();
            }

            return;
        }

        if (texto == "+" || texto == "-" ||
            texto == "x" || texto == "/")
        {
            if (_resultadoMostrado)
                _resultadoMostrado = false;

            if (_currentInput.Length == 0)
            {
                if (texto == "-")
                    _currentInput = "-";

                ActualizarPantalla();
                return;
            }

            char ultimo = _currentInput[^1];

            if (EsOperador(ultimo))
            {
                if (texto == "-" && ultimo != '-')
                {
                    _currentInput += texto;
                }
                else
                {
                    _currentInput =
                        _currentInput[..^1] + texto;
                }
            }
            else if (ultimo == '.')
            {
                return;
            }
            else
            {
                _currentInput += texto;
            }

            ActualizarPantalla();
            return;
        }

      
        if (texto.Length == 1 &&
            char.IsDigit(texto[0]))
        {
            if (_resultadoMostrado)
                _currentInput = string.Empty;

            _resultadoMostrado = false;

            if (_currentInput.Length > 0 &&
                _currentInput[^1] == '%')
            {
                return;
            }

            _currentInput += texto;
            ActualizarPantalla();
        }
    }

    private bool EsOperador(char caracter)
    {
        return caracter == '+' ||
               caracter == '-' ||
               caracter == 'x' ||
               caracter == '/';
    }

    private string ObtenerNumeroActual()
    {
        int inicio = _currentInput.Length;

        while (inicio > 0)
        {
            char c = _currentInput[inicio - 1];

            if (EsOperador(c) || c == '%')
                break;

            inicio--;
        }

        return _currentInput[inicio..];
    }

    private void ActualizarPantalla()
    {
        Display.Text = _currentInput;
    }

    private void CalcularResultado()
    {
        if (string.IsNullOrWhiteSpace(_currentInput))
            return;

        try
        {
            string expresion = _currentInput;

            if (EsOperador(expresion[^1]) ||
                expresion[^1] == '.')
            {
                throw new FormatException();
            }

          
            expresion = expresion.Replace("x", "*");

          
            expresion = Regex.Replace(
                expresion,
                @"(\d+(?:\.\d+)?)%",
                "($1/100)");

           
            if (!Regex.IsMatch(
                expresion,
                @"^[0-9+\-*/().\s]+$"))
            {
                throw new FormatException();
            }

            using DataTable tabla = new DataTable();
            tabla.Locale = CultureInfo.InvariantCulture;

            object resultado = tabla.Compute(
                expresion,
                string.Empty);

            if (resultado == null ||
                resultado == DBNull.Value)
            {
                throw new ArithmeticException();
            }

            double numero = Convert.ToDouble(
                resultado,
                CultureInfo.InvariantCulture);

           
            if (double.IsNaN(numero) ||
                double.IsInfinity(numero))
            {
                throw new DivideByZeroException();
            }

            _currentInput = numero.ToString(
                "G15",
                CultureInfo.InvariantCulture);

            _resultadoMostrado = true;
            ActualizarPantalla();
        }
        catch
        {
            Display.Text = "Error";
            _currentInput = string.Empty;
            _resultadoMostrado = false;
        }
    }
}
