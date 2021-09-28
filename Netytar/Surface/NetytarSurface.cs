using Eyerpheus.Controllers.Graphics;
using NeeqDMIs.Music;
using Netytar.DMIbox;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAnimatedGif;

namespace Netytar
{
    public enum NetytarSurfaceLineModes
    {
        AllLines,
        OnlyScaleLines,
        NoLines
    }

    public enum NetytarSurfaceHighlightModes
    {
        CurrentNote,
        None
    }

    public class NetytarSurface
    {
        private NetytarButton checkedButton;
        private Ellipse highlighter = new Ellipse();
        private NetytarButton lastCheckedButton;
        private Scale scale = ScalesFactory.Cmaj;

        public NetytarSurface(Canvas canvas, NetytarSurfaceLineModes drawMode)
        {
            LoadSettings();
            this.DrawMode = drawMode;

            this.canvas = canvas;
            ResetCanvasDimensions();
        }

        public NetytarButton CheckedButton { get => checkedButton; }
        public NetytarSurfaceLineModes DrawMode { get; set; } = NetytarSurfaceLineModes.OnlyScaleLines;
        public NetytarSurfaceHighlightModes HighLightMode { get; set; }

        public Scale Scale
        {
            get { return scale; }
            set { scale = value; DrawButtons(); }
        }

        #region Settings

        private _SharpNotesModes sharpNotesMode;
        private int ellipseRadius;

        private int generativePitch;

        private int horizontalSpacer;

        private List<Color> keysColorCode = new List<Color>()
        {
            Colors.Red,
            Colors.Orange,
            Colors.Yellow,
            Colors.LightGreen,
            Colors.Blue,
            Colors.Purple,
            Colors.Coral
        };

        private int lineThickness;
        private SolidColorBrush majorBrush;
        private SolidColorBrush minorBrush;
        private int nCols;
        private SolidColorBrush notInScaleBrush;
        private int nRows;
        private int occluderAlpha;
        private int occluderOffset;
        private int startPositionX;
        private int startPositionY;
        private SolidColorBrush transparentBrush = new SolidColorBrush(Colors.Transparent);
        private int verticalSpacer;

        #endregion Settings

        #region Surface components

        private Canvas canvas;

        private List<Ellipse> drawnEllipses = new List<Ellipse>();
        private List<Line> drawnLines = new List<Line>();
        private NetytarButton[,] netytarButtons;

        #endregion Surface components

        public void ClearEllipses()
        {
            for (int i = 0; i < drawnEllipses.Count; i++)
            {
                Ellipse ellipse = drawnEllipses[i];
                canvas.Children.Remove(ellipse);
            }

            drawnEllipses = new List<Ellipse>();
        }

        public void ClearLines()
        {
            for (int i = 0; i < drawnLines.Count; i++)
            {
                Line line = drawnLines[i];
                canvas.Children.Remove(line);
            }

            drawnLines = new List<Line>();
        }

        /// <summary>
        /// Logica di disegno dei pulsanti a schermo
        /// </summary>
        public void DrawButtons()
        {
            ClearButtons();
            LoadSettings();
            ResetCanvasDimensions();

            if (canvas.Children.Contains(highlighter))
            {
                canvas.Children.Remove(highlighter);
            }
            canvas.Children.Add(highlighter);

            // Mi segno un po' di misure
            int halfSpacer = horizontalSpacer / 2;
            int spacer = horizontalSpacer;

            int firstSpacer = 0;

            bool isPairRow;

            for (int row = 0; row < nRows; row++)
            {
                for (int col = 0; col < nCols; col++)
                {
                    #region Is row pair?

                    if (row % 2 != 0)
                    {
                        isPairRow = false;
                    }
                    else
                    {
                        isPairRow = true;
                    }

                    #endregion Is row pair?

                    #region Draw the button on canvas

                    if (isPairRow)
                    {
                        firstSpacer = halfSpacer;
                    }
                    else
                    {
                        firstSpacer = 0;
                    }

                    netytarButtons[row, col] = new NetytarButton(this);

                    #region Define note

                    int calcPitch = generativePitch;
                    calcPitch += col * 2 + row * 2;
                    if (isPairRow)
                    {
                        calcPitch += 1;
                    }
                    netytarButtons[row, col].Note = PitchUtils.PitchToMidiNote(calcPitch);

                    #endregion Define note

                    // Algoritmo che trova la posizione del pulsante
                    int X = startPositionX + firstSpacer + col * horizontalSpacer;
                    int Y = startPositionY + verticalSpacer * row;
                    Canvas.SetLeft(netytarButtons[row, col], X);
                    Canvas.SetTop(netytarButtons[row, col], Y);

                    // Algoritmo che crea l'occluder e ne definisce le dimensioni
                    netytarButtons[row, col].Occluder.Width = occluderOffset * 2;
                    netytarButtons[row, col].Occluder.Height = occluderOffset * 2;
                    netytarButtons[row, col].Occluder.Fill = new SolidColorBrush(Color.FromArgb((byte)occluderAlpha, 0xFF, 0xFF, 0xFF));
                    netytarButtons[row, col].Occluder.Stroke = new SolidColorBrush(Color.FromArgb((byte)occluderAlpha, 0, 0, 0));
                    Canvas.SetLeft(netytarButtons[row, col].Occluder, X - occluderOffset);
                    Canvas.SetTop(netytarButtons[row, col].Occluder, Y - occluderOffset);

                    // Indica le posizioni sull'asse Z
                    Panel.SetZIndex(netytarButtons[row, col], 3);
                    Panel.SetZIndex(netytarButtons[row, col].Occluder, 2000);

                    // Aggiunge gli oggetti al canvas di Netytar
                    canvas.Children.Add(netytarButtons[row, col]);
                    canvas.Children.Add(netytarButtons[row, col].Occluder);
                    if (Rack.UserSettings.SharpNotesMode == _SharpNotesModes.Off && !scale.IsInScale(netytarButtons[row, col].Note))
                    {
                        netytarButtons[row, col].Visibility = Visibility.Hidden;
                        netytarButtons[row, col].Occluder.Visibility = Visibility.Hidden;
                    }

                    #endregion Draw the button on canvas

                    // ===========================================================================
                }
            }
            DrawScale();
        }

        public void DrawEllipses(Scale scale)
        {
            if (netytarButtons != null)
            {
                ClearEllipses();

                List<AbsNotes> scaleNotes = scale.NotesInScale;

                for (int j = 0; j < nCols; j++)
                {
                    for (int k = 0; k < nCols; k++)
                    {
                        AbsNotes note = netytarButtons[j, k].Note.ToAbsNote();

                        Ellipse ellipse = new Ellipse();

                        ellipse.Width = ellipseRadius * 2;
                        ellipse.Height = ellipseRadius * 2;

                        Canvas.SetLeft(ellipse, Canvas.GetLeft(netytarButtons[j, k]) - ellipseRadius);
                        Canvas.SetTop(ellipse, Canvas.GetTop(netytarButtons[j, k]) - ellipseRadius);
                        Canvas.SetZIndex(ellipse, 2);

                        if (!scaleNotes.Contains(note))
                        {
                            if (sharpNotesMode == _SharpNotesModes.On)
                            {
                                ellipse.Stroke = notInScaleBrush;
                                ellipse.Fill = notInScaleBrush;
                            }
                            else if (sharpNotesMode == _SharpNotesModes.Off)
                            {
                                ellipse.Stroke = new SolidColorBrush(Colors.Transparent);
                                ellipse.Fill = new SolidColorBrush(Colors.Transparent);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < scaleNotes.Count; i++)
                            {
                                if (note == scaleNotes[i])
                                {
                                    ellipse.Stroke = new SolidColorBrush(keysColorCode[i]);
                                    ellipse.Fill = new SolidColorBrush(keysColorCode[i]);
                                }
                            }
                        }

                        drawnEllipses.Add(ellipse);
                    }
                }

                foreach (Ellipse ellipse in drawnEllipses)
                {
                    canvas.Children.Add(ellipse);
                }
            }
        }

        public void DrawScale()
        {
            DrawLines(scale);
            DrawEllipses(scale);
        }

        public void FlashMovementLine()
        {
            if (lastCheckedButton != null)
            {
                Point point1 = new Point(Canvas.GetLeft(CheckedButton), Canvas.GetTop(CheckedButton));
                Point point2 = new Point(Canvas.GetLeft(lastCheckedButton), Canvas.GetTop(lastCheckedButton));
                LineFlasherTimer timer = new LineFlasherTimer(point1, point2, canvas, Colors.NavajoWhite);
            }
        }

        public void FlashSpark()
        {
            if (checkedButton != null)
            {
                Image image = new Image();
                image.Height = 40;
                image.Width = 40;
                Canvas.SetLeft(image, Canvas.GetLeft(checkedButton) - 10);
                Canvas.SetTop(image, Canvas.GetTop(checkedButton) - 15);
                Canvas.SetZIndex(image, 100);
                canvas.Children.Add(image);

                BitmapImage bitImage = new BitmapImage();
                bitImage.BeginInit();
                bitImage.UriSource = new Uri("pack://application:,,,/Images/Sparks/Spark3.gif");
                bitImage.EndInit();
                ImageBehavior.SetAnimatedSource(image, bitImage);
                ImageBehavior.SetRepeatBehavior(image, new RepeatBehavior(1));
                ImageBehavior.AddAnimationCompletedHandler(image, disposeImage);
            }
        }

        public void NetytarButton_OccluderMouseEnter(NetytarButton sender)
        {
            if (sender != CheckedButton)
            {
                Rack.DMIBox.SelectedNote = sender.Note;

                lastCheckedButton = checkedButton;
                checkedButton = sender;

                FlashMovementLine();

                if (HighLightMode == NetytarSurfaceHighlightModes.CurrentNote)
                {
                    MoveHighlighter(CheckedButton);
                }
            }
        }

        public void LoadSettings()
        {
            ellipseRadius = Rack.UserSettings.EllipseRadius;
            horizontalSpacer = Rack.UserSettings.HorizontalSpacer;
            lineThickness = Rack.UserSettings.LineThickness;
            occluderOffset = Rack.UserSettings.OccluderOffset;
            verticalSpacer = Rack.UserSettings.VerticalSpacer;
            sharpNotesMode = Rack.UserSettings.SharpNotesMode;

            keysColorCode = Rack.ColorCode.KeysColorCode;
            notInScaleBrush = Rack.ColorCode.NotInScaleBrush;
            majorBrush = Rack.ColorCode.MajorBrush;
            minorBrush = Rack.ColorCode.MinorBrush;

            generativePitch = Rack.ButtonsSettings.GenerativeNote;
            nCols = Rack.ButtonsSettings.NCols;
            nRows = Rack.ButtonsSettings.NRows;
            startPositionX = Rack.ButtonsSettings.StartPositionX;
            startPositionY = Rack.ButtonsSettings.StartPositionY;
            occluderAlpha = Rack.ButtonsSettings.OccluderAlpha;

            highlighter.Width = Rack.UserSettings.HighlightRadius;
            highlighter.Height = Rack.UserSettings.HighlightRadius;
            highlighter.StrokeThickness = Rack.UserSettings.HighlightStrokeDim;
            highlighter.Stroke = Rack.ColorCode.HighlightBrush;

            //canvas.Background =
        }

        private void ResetCanvasDimensions()
        {
            canvas.Width = startPositionX * 2 + (horizontalSpacer + 13) * (nCols - 1);
            canvas.Height = startPositionY * 2 + (verticalSpacer + 13) * (nRows - 1);
        }

        private void ClearButtons()
        {
            if (netytarButtons != null)
            {
                foreach (NetytarButton button in netytarButtons)
                {
                    canvas.Children.Remove(button);
                    canvas.Children.Remove(button.Occluder);
                }

                netytarButtons = new NetytarButton[nRows, nCols];
            }
            netytarButtons = new NetytarButton[nRows, nCols];
        }

        private void disposeImage(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(((Image)sender));
        }

        private void DrawLines(Scale scale)
        {
            if (netytarButtons != null)
            {
                ClearLines();
                if (DrawMode != NetytarSurfaceLineModes.NoLines)
                {
                    Brush inScaleBrush = new SolidColorBrush();

                    #region Determine inScale brush

                    switch (scale.ScaleCode)
                    {
                        case ScaleCodes.maj:
                            inScaleBrush = majorBrush;
                            break;

                        case ScaleCodes.min:
                            inScaleBrush = minorBrush;
                            break;

                        default:
                            inScaleBrush = majorBrush;
                            break;
                    }

                    #endregion Determine inScale brush

                    bool isPairRow;

                    Point realCenter1;
                    Point realCenter2;

                    for (int row = 0; row < nRows; row++)
                    {
                        for (int col = 0; col < nCols; col++)
                        {
                            #region Is row pair?

                            if (row % 2 != 0)
                            {
                                isPairRow = false;
                            }
                            else
                            {
                                isPairRow = true;
                            }

                            #endregion Is row pair?

                            #region Draw horizontal lines

                            if (col != 0)
                            {
                                Brush brush;
                                if (scale.AreConsequent(netytarButtons[row, col].Note, netytarButtons[row, col - 1].Note))
                                {
                                    brush = inScaleBrush;
                                }
                                else
                                {
                                    if (DrawMode == NetytarSurfaceLineModes.OnlyScaleLines)
                                    {
                                        brush = transparentBrush;
                                    }
                                    else
                                    {
                                        brush = notInScaleBrush;
                                    }
                                }
                                realCenter1 = new Point(Canvas.GetLeft(netytarButtons[row, col]), Canvas.GetTop(netytarButtons[row, col]));
                                realCenter2 = new Point(Canvas.GetLeft(netytarButtons[row, col - 1]), Canvas.GetTop(netytarButtons[row, col - 1]));
                                Line myLine = new Line();

                                myLine.Stroke = brush;
                                myLine.X1 = realCenter1.X;
                                myLine.X2 = realCenter2.X;
                                myLine.Y1 = realCenter1.Y;
                                myLine.Y2 = realCenter2.Y;
                                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                                myLine.VerticalAlignment = VerticalAlignment.Center;
                                myLine.StrokeThickness = lineThickness;
                                Panel.SetZIndex(myLine, 1);
                                drawnLines.Add(myLine);

                                netytarButtons[row, col - 1].L_p2 = myLine;
                                netytarButtons[row, col].L_m2 = myLine;
                            }

                            #endregion Draw horizontal lines

                            #region Draw diagonal lines

                            // Diagonale A: se riga pari p+1, se dispari p+3
                            if (row != 0)
                            {
                                Brush brush;
                                if (scale.AreConsequent(netytarButtons[row, col].Note, netytarButtons[row - 1, col].Note))
                                {
                                    brush = inScaleBrush;
                                }
                                else
                                {
                                    if (DrawMode == NetytarSurfaceLineModes.OnlyScaleLines)
                                    {
                                        brush = transparentBrush;
                                    }
                                    else
                                    {
                                        brush = notInScaleBrush;
                                    }
                                }

                                if (isPairRow)
                                {
                                    realCenter1 = new Point(Canvas.GetLeft(netytarButtons[row, col]) + 2, Canvas.GetTop(netytarButtons[row, col]) + 2);
                                    realCenter2 = new Point(Canvas.GetLeft(netytarButtons[row - 1, col]) + 11, Canvas.GetTop(netytarButtons[row - 1, col]) + 11);
                                }
                                else
                                {
                                    realCenter1 = new Point(Canvas.GetLeft(netytarButtons[row, col]) + 11, Canvas.GetTop(netytarButtons[row, col]) + 2);
                                    realCenter2 = new Point(Canvas.GetLeft(netytarButtons[row - 1, col]) + 2, Canvas.GetTop(netytarButtons[row - 1, col]) + 11);
                                }

                                Line myLine = new Line();

                                myLine.Stroke = brush;
                                myLine.X1 = realCenter1.X;
                                myLine.X2 = realCenter2.X;
                                myLine.Y1 = realCenter1.Y;
                                myLine.Y2 = realCenter2.Y;
                                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                                myLine.VerticalAlignment = VerticalAlignment.Center;
                                myLine.StrokeThickness = lineThickness;
                                Panel.SetZIndex(myLine, 1);
                                drawnLines.Add(myLine);

                                if (isPairRow)
                                {
                                    netytarButtons[row - 1, col].L_p3 = myLine;
                                    netytarButtons[row, col].L_m3 = myLine;
                                }
                                else
                                {
                                    netytarButtons[row - 1, col].L_p1 = myLine;
                                    netytarButtons[row, col].L_m1 = myLine;
                                }

                                // Diagonale B: se riga pari p+3, se dispari p+1

                                if (isPairRow)
                                {
                                    if (col < nCols - 1)
                                    {
                                        if (scale.AreConsequent(netytarButtons[row, col].Note, netytarButtons[row - 1, col + 1].Note))
                                        {
                                            brush = inScaleBrush;
                                        }
                                        else
                                        {
                                            if (DrawMode == NetytarSurfaceLineModes.OnlyScaleLines)
                                            {
                                                brush = transparentBrush;
                                            }
                                            else
                                            {
                                                brush = notInScaleBrush;
                                            }
                                        }
                                        realCenter1 = new Point(Canvas.GetLeft(netytarButtons[row, col]) + 11, Canvas.GetTop(netytarButtons[row, col]) + 2);
                                        realCenter2 = new Point(Canvas.GetLeft(netytarButtons[row - 1, col + 1]) + 2, Canvas.GetTop(netytarButtons[row - 1, col + 1]) + 11);

                                        Line diaLine = new Line();

                                        diaLine.Stroke = brush;
                                        diaLine.X1 = realCenter1.X;
                                        diaLine.X2 = realCenter2.X;
                                        diaLine.Y1 = realCenter1.Y;
                                        diaLine.Y2 = realCenter2.Y;
                                        diaLine.HorizontalAlignment = HorizontalAlignment.Left;
                                        diaLine.VerticalAlignment = VerticalAlignment.Center;
                                        diaLine.StrokeThickness = lineThickness;
                                        Panel.SetZIndex(diaLine, 1);
                                        drawnLines.Add(diaLine);

                                        netytarButtons[row - 1, col + 1].L_p1 = diaLine;
                                        netytarButtons[row, col].L_m1 = diaLine;
                                    }
                                }
                                else
                                {
                                    if (col > 0)
                                    {
                                        if (scale.AreConsequent(netytarButtons[row, col].Note, netytarButtons[row - 1, col - 1].Note))
                                        {
                                            brush = inScaleBrush;
                                        }
                                        else
                                        {
                                            if (DrawMode == NetytarSurfaceLineModes.OnlyScaleLines)
                                            {
                                                brush = transparentBrush;
                                            }
                                            else
                                            {
                                                brush = notInScaleBrush;
                                            }
                                        }
                                        realCenter1 = new Point(Canvas.GetLeft(netytarButtons[row, col]) + 2, Canvas.GetTop(netytarButtons[row, col]) + 2);
                                        realCenter2 = new Point(Canvas.GetLeft(netytarButtons[row - 1, col - 1]) + 11, Canvas.GetTop(netytarButtons[row - 1, col - 1]) + 11);

                                        Line diaLine = new Line();

                                        diaLine.Stroke = brush;
                                        diaLine.X1 = realCenter1.X;
                                        diaLine.X2 = realCenter2.X;
                                        diaLine.Y1 = realCenter1.Y;
                                        diaLine.Y2 = realCenter2.Y;
                                        diaLine.HorizontalAlignment = HorizontalAlignment.Left;
                                        diaLine.VerticalAlignment = VerticalAlignment.Center;
                                        diaLine.StrokeThickness = lineThickness;
                                        Panel.SetZIndex(diaLine, 1);
                                        drawnLines.Add(diaLine);

                                        netytarButtons[row - 1, col - 1].L_p3 = diaLine;
                                        netytarButtons[row, col].L_m3 = diaLine;
                                    }
                                }
                            }

                            #endregion Draw diagonal lines
                        }
                    }

                    foreach (Line line in drawnLines)
                    {
                        canvas.Children.Add(line);
                    }
                }
            }
        }

        private void MoveHighlighter(NetytarButton checkedButton)
        {
            Canvas.SetLeft(highlighter, Canvas.GetLeft(checkedButton) - highlighter.ActualWidth / 2);
            Canvas.SetTop(highlighter, Canvas.GetTop(checkedButton) - highlighter.ActualHeight / 2);
        }
    }
}