using System.Collections.ObjectModel;
using System.Windows.Media;
using Caliburn.Micro;

namespace F1TelemetryUi.ViewModels
{
    public class Drawing
    {
        public Brush Fill { get; set; }
        public System.Windows.Media.Geometry Geometry { get; set; }
        public Brush Stroke { get; set; }
        public double StrokeThickness { get; set; }
        public double ScaleX { get; set; }
        public double ScaleY { get; set; }
    }

    public class NewMapViewModel : PropertyChangedBase
    {
        public NewMapViewModel()
        {
            Geometry tempGeo =
                PathGeometry.Parse("M148 113.333C161.141 110.778 169.316 119.316 178 128C185.749 135.749 193.846 143.639 202 151C217.382 164.886 233.234 178.439 248 193C253.082 198.011 260.673 202.265 263.667 209C266.046 214.354 264.607 219.539 264 225C262.306 240.249 254.585 255.124 258.333 271C261.645 285.026 274.083 299.125 282 311C285.324 315.986 291.982 322.574 291 329C290.285 333.677 285.889 334.415 282 335.333C275.071 336.969 266.469 337.333 260 340.333C251.978 344.054 257.942 351.646 263 354.667C265.561 356.196 268.318 357.567 271 358.833C279.287 362.747 295.161 368.183 304 364.5C308.092 362.795 310.811 358.879 314 356C321.702 349.047 329.29 341.96 337 335C359.765 314.448 382.229 293.557 405 273L427 253C433.085 247.506 439.148 242.761 440 234C440.21 231.839 440.247 229.127 439.833 227C436.585 210.294 411.114 220.701 404.167 207C398.558 195.938 407.389 184.71 419 186C426.771 186.863 435.376 194.878 442 198.833C458.24 208.532 473.371 219.886 480.667 238C484.205 246.786 483 257.681 483 267L483 321C483 340.852 489.281 374.788 467 385C455.22 390.399 440.718 391.764 428 393C411.08 394.645 393.183 396.671 376 395C364.754 393.907 353.352 393 342 393C330.07 393 320.638 403.132 309 402C303.25 401.441 298.942 397.638 294 395.167C287.51 391.922 280.208 389.268 273 388.167C257.216 385.755 247.576 405.867 231 398.5C217.323 392.421 216.776 373.11 206.833 363.167C200.369 356.702 191.584 352.056 184 347L141 318.333C112.938 299.625 83.2475 282.31 57 261.167C46.8842 253.018 34.7189 245.114 29.1667 233C25.1357 224.205 29.1922 210.755 37 205.333C44.6599 200.014 54.6966 198.985 63 194.833C78.6588 187.004 91.888 175.525 106 165.333C111.56 161.318 120.654 157.433 124.833 152C130.51 144.621 114.939 138.456 119.167 130C123.864 120.606 138.35 115.21 148 113.333");

            PathGeometry pathGeo = tempGeo.GetFlattenedPathGeometry();


            tempGeo.GetOutlinedPathGeometry()
                .GetPointAtFractionLength(
                    1.0f, 
                    out System.Windows.Point point, 
                    out System.Windows.Point tangent);

            Drawings.Add(new Drawing
            {
                Geometry = pathGeo,
                Stroke = Brushes.Red,
                StrokeThickness = 5,
                ScaleX = 1.33333,
                ScaleY = 1.33333,
            });
        }

        public ObservableCollection<Drawing> Drawings { get; set; } = new ObservableCollection<Drawing>();
    }
}
