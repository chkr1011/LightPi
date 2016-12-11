using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LightPi.Midi2OrchestratorBridge.Models;
using LightPi.Midi2OrchestratorBridge.ViewModels;

namespace LightPi.Midi2OrchestratorBridge.UI.Views
{
    public partial class EmulatorView
    {
        private readonly string _spritesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sprites");
        private readonly BooleanToVisibilityConverter _booleanToVisibilityConverter = new BooleanToVisibilityConverter();

        private EmulatorViewModel _dataContext;

        public EmulatorView()
        {
            InitializeComponent();

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_dataContext != null)
            {
                throw new InvalidOperationException();
            }

            _dataContext = e.NewValue as EmulatorViewModel;
            if (_dataContext == null)
            {
                return;
            }

            //_dataContext.View = this;
            
            AddSprite("Background", null);
            foreach (var output in _dataContext.Outputs)
            {
                //Surface.AddOutput(output);
                AddSprite(output.Output.Id.ToString(), output);
            }

            //Surface.Update();
        }

        ////public void Update()
        ////{
        ////    //Surface.Update();
        ////}

        private void AddSprite(string id, OutputViewModel output)
        {
            var spriteFilename = Path.Combine(_spritesDirectory, $"{id}.png");
            if (!File.Exists(spriteFilename))
            {
                return;
            }

            var image = new Image
            {
                Source = new BitmapImage(new Uri(spriteFilename, UriKind.Absolute)),
                Stretch = Stretch.Uniform // TODO: Consider setting or UI control > UniformToFill
            };

            image.Source.Freeze();

            if (output != null)
            {
                var binding = new Binding
                {
                    Path = new PropertyPath("IsActive"),
                    Converter = _booleanToVisibilityConverter,
                    Source = output
                };

                image.SetBinding(VisibilityProperty, binding);
            }
            
            SpritesGrid.Children.Add(image);
        }
    }
}
