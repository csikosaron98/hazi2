using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace hazi2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<poz> input = new List<poz>();
        Rectangle porszivo = new Rectangle();
        Rectangle jart = new Rectangle();
        List<Rectangle> jart_teglalapok = new List<Rectangle>();
        const int SOR = 12;
        const int OSZLOP = 12;
        const int H = 50;
        const int W = 50;
        int j = 0;
        const int szobaterulet = (SOR - 2) * (OSZLOP - 2);
        string[,] palya = new string[SOR, OSZLOP];
        const int faldb = 2 * SOR + 2 * OSZLOP + (SOR + 4) + (OSZLOP + 4) + 10;
        public enum irany
        {
            fel,
            le,
            jobbra,
            balra
        }
        public struct poz
        {
            public int x;
            public int y;
            public poz(int x, int y) : this()
            {
                this.x = x;
                this.y = y;
            }
        }
        poz robotpoz;
        List<poz> jartpoz = new List<poz>();  // dinamikus tárolóelem a bejárt területek tárolására
        poz[] falakpoz = new poz[faldb];
        public MainPage()
        {
            this.InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e) //pálya inic gomb event handler -> tömbbe
        {
            int x = 200;
            int y = 50;
            int k = 0;
            StreamReader reader = new StreamReader("robot.txt");
            for (int i = 0; i < SOR; i++)
            {
                string sor = reader.ReadLine();
                for (int j = 0; j < OSZLOP; j++)
                {
                    palya[i, j] = sor[j].ToString();
                }
            }
            for (int i = 0; i < SOR; i++)
            {
                for (int j = 0; j < OSZLOP; j++)
                {
                    if (alakzatvizsgal(palya[i, j]) == 0)
                    {
                        falrajzol(palya, x, y); //x=200tol 1450ig, y=0 tol 700 ig
                        falakpoz[k].x = x;
                        falakpoz[k].y = y;
                        k++;
                    }
                    else if (alakzatvizsgal(palya[i, j]) == 1)
                    {
                       
                        robotrajzol(x, y);
                        robotpoz.x = x;
                        robotpoz.y = y;
                    }
                    x += W;
                }
                if (x == 800)
                    x = 200;
                y += H;
            }
        }

        private void buttonelore_Click(object sender, RoutedEventArgs e)
        {
            robotfel(robotpoz);
        }
        private void buttonbalra1_Click(object sender, RoutedEventArgs e)
        {
            robotbal(robotpoz);
        }
        private void buttonjobbra_Click(object sender, RoutedEventArgs e)
        {
            robotjobb(robotpoz);
        }
        private void buttonhatra_Click(object sender, RoutedEventArgs e)
        {
            robotle(robotpoz);
        }
        private void utkozes()
         {
             for (int i = 0; i < faldb; i++)
             {
                 if (Math.Abs(falakpoz[i].x-robotpoz.x) < W && Math.Abs(falakpoz[i].y - robotpoz.y) < H)
                {
                    porszivo.Fill = new SolidColorBrush(Windows.UI.Colors.Green);
                }
             }
         }
        int progressbarint = 0;
        private void progressbar()
        {
                if (!jartpoz.Contains(robotpoz))
                {
                 progressbarint++;
                 lefedettség.Value = progressbarint;   
                }
        }
     
        private void jart_kirajzol()
        {
            for (int i = j; i < jart_teglalapok.Count; i++)
            {
                jart_teglalapok[i].Fill = new SolidColorBrush(Windows.UI.Colors.LightGray);
                jart_teglalapok[i].Width = W;
                jart_teglalapok[i].Height = H;
                jart_teglalapok[i].Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
                jart_teglalapok[i].StrokeThickness = 1;

                Canvas.Children.Add(jart_teglalapok[i]);

                Canvas.SetLeft(jart_teglalapok[i], jartpoz[i].x);
                Canvas.SetTop(jart_teglalapok[i], jartpoz[i].y);
                j++;
            }
        }

        // 4 irányba történő mozgatás, a bejárt területek folyamatos lementése, a progressbar növelése új terület érintésekor
   
        public void robotfel(poz p)
        {
            Canvas.SetLeft(porszivo, p.x);
            Canvas.SetTop(porszivo, p.y - H);
            string[,] tmp = new string[2, 5];
            VacCleaner VAC = new VacCleaner(palya, robotpoz);
            tmp = VAC.getData(irany.fel);
            jartpoz.Add(new poz(robotpoz.x, robotpoz.y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
            robotpoz.y = robotpoz.y - H;
            progressbar();
            utkozes();
        }
        public void robotle(poz p)
        {
            Canvas.SetLeft(porszivo, p.x);
            Canvas.SetTop(porszivo, p.y + H);
            string[,] tmp = new string[2, 5];
            VacCleaner VAC = new VacCleaner(palya, robotpoz);
            tmp = VAC.getData(irany.le);
            jartpoz.Add(new poz(robotpoz.x, robotpoz.y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
            robotpoz.y = robotpoz.y + H;
            progressbar();
            utkozes();
        }
        public void robotjobb(poz p)
        {
            Canvas.SetLeft(porszivo, p.x + W);
            Canvas.SetTop(porszivo, p.y);
            string[,] tmp = new string[2, 5];
            VacCleaner VAC = new VacCleaner(palya, robotpoz);
            tmp = VAC.getData(irany.jobbra);
            jartpoz.Add(new poz(robotpoz.x, robotpoz.y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
            robotpoz.x = robotpoz.x + W;
            progressbar();
            utkozes();
        }
        public void robotbal(poz p)
        {
            Canvas.SetLeft(porszivo, p.x - W);
            Canvas.SetTop(porszivo, p.y);
            string[,] tmp = new string[2, 5];
            VacCleaner VAC = new VacCleaner(palya, robotpoz);
            tmp = VAC.getData(irany.balra);
            jartpoz.Add(new poz(robotpoz.x, robotpoz.y));
            jart_teglalapok.Add(new Rectangle());
            jart_kirajzol();
            robotpoz.x = robotpoz.x - W;
            progressbar();
            utkozes();
        }

        //falak kirajzolása -> kék négyzet
        private void falrajzol(string[,] tomb, int x, int y)
        {
            Rectangle fal = new Rectangle();
            fal.Fill = new SolidColorBrush(Windows.UI.Colors.Blue);
            fal.Width = W;
            fal.Height = H;
            fal.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            fal.StrokeThickness = 1;

            Canvas.Children.Add(fal);

            Canvas.SetLeft(fal, x);
            Canvas.SetTop(fal, y);
        }

        //robot kirajzolása -> piros négyzet
        private void robotrajzol(int x, int y)
        {
            porszivo.Fill = new SolidColorBrush(Windows.UI.Colors.Red);
            porszivo.Width = W;
            porszivo.Height = H;
            porszivo.Stroke = new SolidColorBrush(Windows.UI.Colors.Black);
            porszivo.StrokeThickness = 1;

            Canvas.Children.Add(porszivo);

            Canvas.SetLeft(porszivo, x);
            Canvas.SetTop(porszivo, y);
            Canvas.SetZIndex(porszivo,1);
        }

        //alakzatvizsgálás a txt-hez -> o: fal, x: robot
        private int alakzatvizsgal(string s)
        {
            if (s == "o")
                return 0;
            else if (s == "x")
                return 1;
            else
                return 2;
        }

        private void lefedettség_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            lefedettség.Maximum = szobaterulet;
            lefedettség.Value = progressbarint;
        }
        
        private void alg1_Click(object sender, RoutedEventArgs e)
        {
            string[,] tmp = new string[2, 5];
            VacCleaner VAC = new VacCleaner(palya, robotpoz);
            tmp = VAC.getData(irany.jobbra);
           
        }
        
    }

}


